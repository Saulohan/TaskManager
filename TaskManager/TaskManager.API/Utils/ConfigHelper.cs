using Microsoft.Extensions.Configuration;

namespace TaskManager.Infrastructure
{
    public class ConfigHelper
    {
        private readonly IConfiguration _configuration;

        public ConfigHelper() { }

        public ConfigHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T GetValue<T>(string key)
        {
            return _configuration.GetValue<T>($"AppSettings:{key}");
        }

        public long DefaultUserId
        {
            get
            {
                try
                {
                    return GetValue<long>("DefaultUserId");
                }
                catch
                {
                    return 1;
                }
            }
        }

        public virtual int NumberOfDaysToReport
        {
            get
            {
                try
                {
                    return _configuration.GetValue<int>("AppSettings:NumberOfDaysToReport");
                }
                catch
                {
                    return 123;
                }
            }
        }

        public int MaxTasksPerProject
        {
            get
            {
                try
                {
                    return _configuration.GetValue<int>("AppSettings:MaxTasksPerProject");
                }
                catch
                {
                    return 20;
                }
            }
        }

    }
}
