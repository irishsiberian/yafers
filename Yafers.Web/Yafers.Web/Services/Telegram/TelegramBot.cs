using Microsoft.Extensions.Options;
using System.Net;
using Yafers.Web.Middleware;

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

        public async Task SendException(Exception ex, HttpContext? ctx)
        {
            if (string.IsNullOrEmpty(_botToken) || string.IsNullOrEmpty(_chatId))
                return;

            var urlMessage = ctx == null ? "" : $"*URL:* {ctx.Request.Method} {ctx.Request.Path}";
            var message = $@"
🚨 <b>Ошибка Yafers</b> 🚨
{urlMessage}
<b>Время:</b> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
<b>Сообщение:</b> <code>{ex.Message}</code>
<b>Тип:</b> <code>{ex.GetType().Name}</code>
<pre>{WebUtility.HtmlEncode(ex.StackTrace)}</pre>
";

            var content = GetSendContent(message);

            try
            {
                var results = await _http.PostAsync(_sendUrl, content);
                var response = await results.Content.ReadAsStringAsync();
            }
            catch (Exception sendEx)
            {
                logger.LogError(sendEx, "Failed to send error to Telegram");
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
