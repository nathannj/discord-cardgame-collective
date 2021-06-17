using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CardCollective.Services
{
    public class TestService
    {
        private readonly ILogger _logger;
        public TestService(ILogger<TestService> logger)
        {
            _logger = logger;
        }

        public void LogAsync(string message)
        {
            _logger.LogInformation("test");
        }
    }
}
