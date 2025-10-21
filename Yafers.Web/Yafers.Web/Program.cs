using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Swashbuckle.AspNetCore.Filters;
using Yafers.Web;
using Yafers.Web.Components;
using Yafers.Web.Components.Account;
using Yafers.Web.Data;
using Yafers.Web.Extensions;
using Yafers.Web.Middleware;
using Yafers.Web.Services.EmailSender;
using Yafers.Web.Telegram;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithExceptionDetails()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Async(a => a.File("logs/log.txt", rollingInterval: RollingInterval.Day))
    .WriteTo.Async(a => a.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {puVersion} {TraceId} {NewLine}{Exception}"))
    .CreateLogger();
Log.Verbose("Logger created");

try
{
    builder.Services.AddCommonConfiguration(builder.Configuration);

    //сервисы для аутентификации и Identity
    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddScoped<IdentityUserAccessor>();
    builder.Services.AddScoped<IdentityRedirectManager>();
    builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // путь для перенаправления при неавторизованном доступе
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // время жизни куки
        options.SlidingExpiration = true; // обновление куки при активности пользователя
    });

    builder.Services.AddSwaggerGen(option =>
    {
        option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
        option.OperationFilter<SecurityRequirementsOperationFilter>();
    });

//Строка подключения к БД и настройка DbContext
    builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
    {
        // Берём DatabaseOptions из DI
        var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

        if (string.IsNullOrEmpty(dbOptions.YafersConnectionString))
            throw new InvalidOperationException("Database connection string is not configured.");

        options.UseSqlServer(dbOptions.YafersConnectionString);
    });
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    //Добавляем IdentityApi и IdentityCore
    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true; // опционально
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    //builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    //    .AddIdentityCookies();

    //builder.Services.AddAuthorization();

    //Конфигурация для отправки почты
    builder.Services.AddSingleton<IEmailSender<ApplicationUser>, EmailSender>();
    builder.Services.AddTransient<IEmailSender, EmailSender>();
    builder.Services.Configure<EmailSenderOptions>(builder.Configuration);

    builder.Services.Configure<TelegramBotOptions>(builder.Configuration);
    builder.Services.Configure<DatabaseOptions>(builder.Configuration);
    builder.Services.AddScoped<TelegramBot>();

    builder.Services.AddHostedService<MigrationRunner>();

    var app = builder.Build();

    app.MapIdentityApi<ApplicationUser>();

    app.UseMiddleware<ExceptionMiddleware>();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
        app.UseMigrationsEndPoint();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseAntiforgery();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode()
        .AddInteractiveWebAssemblyRenderMode()
        .AddAdditionalAssemblies(typeof(Yafers.Web.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
    app.MapAdditionalIdentityEndpoints();

//применяем миграции и создаём БД, если её нет
//раскомментировать только при самом первом запуске
    using (var scope = app.Services.CreateScope())
    {
        //var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //db.Database.Migrate(); // применяет миграции и создает базу, если ее нет
        
        var dbOptions = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        Log.Information("Database connection: " + dbOptions.YafersConnectionString);

        // Seed roles and admin user
        IdentitySeeder.SeedRolesAndAdminAsync(scope.ServiceProvider).GetAwaiter().GetResult();
    }

    app.Run();

}
catch (Exception ex)
{
    Log.Error(ex, "Fatal error");
    try
    {
        using var scope = builder.Services.BuildServiceProvider().CreateScope();
        var bot = scope.ServiceProvider.GetService<TelegramBot>();

        if (bot != null)
        {
            bot.SendException(ex, null).GetAwaiter().GetResult();
        }
        else
        {
            Log.Warning("TelegramBot is not available in DI — cannot send fatal error");
        }
    }
    catch (Exception sendEx)
    {
        Log.Error(sendEx, "Failed to send fatal error to Telegram");
    }
}