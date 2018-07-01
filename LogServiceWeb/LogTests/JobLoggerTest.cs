using System;
using Xunit;

namespace LogTests
{
    public class JobLoggerTest
    {
        [Fact]
        public void LogWarningToFile()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var file = System.IO.Path.Combine(documents, "LogFile" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
            string message = " First warning message  ";

            JobLogger.LogAMessage(true, false, false, true, true, true, message, false, true, false);

            //Verify that the file has this new message
            Assert.True(System.IO.File.ReadAllText(file).Contains(message));
        }

        [Fact]
        public void LogErrorToConsole()
        {
            string message = " First error message  ";

            JobLogger.LogAMessage(false, true, false, true, true, true, message, false, false, true);
        }

        [Fact]
        public void LogMessageToDatabase()
        {
            string message = " First message  ";

            JobLogger.LogAMessage(false, false, true, true, true, true, message, true, false, false);
        }
    }
}