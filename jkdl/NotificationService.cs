using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace jkdl
{
    class NotificationService : INotificationService
    {
        private readonly ITextProvider _textProvider;

        public NotificationService(ITextProvider textProvider)
        {
            _textProvider = textProvider;
        }

        public async Task ProccessError(ILogger logger, string msg)
        {
            await _textProvider.Writer.WriteLineAsync($"Error: {msg}");
            logger.LogError(msg);
        }
    }
}
