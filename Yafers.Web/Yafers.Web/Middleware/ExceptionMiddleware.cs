using Yafers.Web.Services.Telegram;

namespace Yafers.Web.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, TelegramBot telegramBot)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await telegramBot.SendException(ex, context, null);
        }
    }
}