using System;
using System.Net;
using TaskManager.Database;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Mappers;
using TaskManager.Infrastructure;

namespace TaskManagerAPI.Utils
{
    public static class Utils
    {
        public static void SaveLogError(Exception ex)
        {
            string logFilePath = $"logs/{DateTime.Now.ToString("dd-MM-yyyy")}/errorApi_log.txt";

            var logDirectory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            string logMessage = $"{DateTime.UtcNow}: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}\n\n";

            try
            {
                File.AppendAllText(logFilePath, logMessage);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}