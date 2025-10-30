using Microsoft.Extensions.Options;
using System.Net;
using Microsoft.AspNetCore.SignalR;
using Yafers.Web.Middleware;
using Yafers.Web.Utils;

namespace Yafers.Web.Services.Telegram
{
    public class TelegramBot(ILogger<ExceptionMiddleware> logger, IOptions<TelegramBotOptions> optionsAccessor)
    {
        private readonly string _botToken = optionsAccessor.Value.TelegramBotToken;
        private readonly string _chatId = optionsAccessor.Value.TelegramChatId;
        private static readonly HttpClient _http = new HttpClient();
        private readonly string _sendUrl = $"https://api.telegram.org/bot{optionsAccessor.Value.TelegramBotToken}/sendMessage";

        public async Task SendMessage(string message)
        {
            var content = GetSendContent(message);
            try
            {
                await _http.PostAsync(_sendUrl, content);
            }
            catch (Exception sendEx)
            {
                logger.LogError(sendEx, "Failed to send error to Telegram");
            }
        }

        public async Task SendException(Exception ex, HttpContext? ctx, HubInvocationContext hubCtx)
        {
            if (string.IsNullOrEmpty(_botToken) || string.IsNullOrEmpty(_chatId))
                return;

            var urlMessage = ctx == null ? "" : $"*URL:* {ctx.Request.Method} {ctx.Request.Path}";
            if (hubCtx != null)
            {
                urlMessage += $"\n*SignalR Hub:* {hubCtx.Hub.GetType().FullName}\n*Method:* {hubCtx.HubMethodName}";
            }
            var stackTraceParts = ex.StackTrace.SplitInPartsByWords();
            foreach (var part in stackTraceParts)
            {
                var message = $@"
🚨 <b>Ошибка Yafers</b> 🚨
{urlMessage}
<b>Время:</b> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
<b>Сообщение:</b> <code>{ex.Message}</code>
<b>Тип:</b> <code>{ex.GetType().Name}</code>
<pre>{WebUtility.HtmlEncode(part)}</pre>";

                var content = GetSendContent(message);

                try
                {
                    var results = await _http.PostAsync(_sendUrl, content);
                    var response = await results.Content.ReadAsStringAsync();
                    var re = response;
                }
                catch (Exception sendEx)
                {
                    logger.LogError(sendEx, "Failed to send error to Telegram");
                }
            }
        }

        private FormUrlEncodedContent GetSendContent(string message)
        {
            var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("chat_id", _chatId),
                new KeyValuePair<string, string>("text", message),
                new KeyValuePair<string, string>("parse_mode", "HTML")
            ]);
            return content;
        }

    }
}
