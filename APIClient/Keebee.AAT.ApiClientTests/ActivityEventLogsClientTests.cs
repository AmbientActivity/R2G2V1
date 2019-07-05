using Keebee.AAT.ApiClient.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Keebee.AAT.ApiClientTests
{
    [TestClass]
    public class ActivityEventLogsClientTests
    {
        [TestMethod]
        public void GetActivityEventLogsForDate()
        {
            var client = new ActivityEventLogsClient();
            const string date ="07/22/2017";

            // Act
            var eventLogs = client.GetForDate(date);

            // Assert
            Assert.IsNotNull(eventLogs);
        }
    }
}
