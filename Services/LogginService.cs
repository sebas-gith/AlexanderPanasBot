using Discord;
using System;

namespace Alexander.Services
{
    public class LoggingService
    {
        public Task LogAsync(LogMessage message)
        {
            Console.WriteLine($"[{DateTime.Now,-19}] [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
            return Task.CompletedTask;
        }
    }
}