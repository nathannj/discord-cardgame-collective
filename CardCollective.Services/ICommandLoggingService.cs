using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CardCollectiveBot.Common
{
    public interface ICommandLoggingService
    {
        Task LogAsync(LogMessage message);
    }
}
