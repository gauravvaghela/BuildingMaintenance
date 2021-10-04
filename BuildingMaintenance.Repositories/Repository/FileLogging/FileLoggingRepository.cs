using BuildingMaintenance.Repositories.IRepository.IFileLogging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingMaintenance.Repositories.Repository.FileLogging
{
    public class FileLoggingRepository : IFileLoggingRepository
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        public FileLoggingRepository()
        {

        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Information(string message)
        {
            _logger.Info(message);
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }
    }

}