using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace jkdl
{
    internal interface INotificationService
    {
        Task ProccessError(ILogger logger, Exception ex);
        Task ProccessError(ILogger logger, string msg);
    }
}