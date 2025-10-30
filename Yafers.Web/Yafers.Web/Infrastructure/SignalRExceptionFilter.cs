using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Yafers.Web.Services.Telegram;

namespace Yafers.Web.Infrastructure
{
    public sealed class SignalRExceptionFilter : IHubFilter
    {
        private readonly ILogger<SignalRExceptionFilter> _logger;
        private readonly TelegramBot _telegramBot;

        public SignalRExceptionFilter(ILogger<SignalRExceptionFilter> logger, TelegramBot telegramBot)
        {
            _logger = logger;
            _telegramBot = telegramBot;
        }

        public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
        {
            try
            {
                return await next(invocationContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in SignalR invocation. Hub: {Hub}, Method: {Method}", invocationContext.Hub.GetType().FullName, invocationContext.HubMethodName);
                // Optionally rethrow to let SignalR propagate to client
                await _telegramBot.SendException(ex, null, invocationContext);
                throw;
            }
        }

        // Optional: implement other IHubFilter methods if needed
        public ValueTask OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, ValueTask> next) => next(context);
        public ValueTask OnDisconnectedAsync(HubLifetimeContext context, Exception? exception, Func<HubLifetimeContext, Exception?, ValueTask> next) => next(context, exception);
    }
}