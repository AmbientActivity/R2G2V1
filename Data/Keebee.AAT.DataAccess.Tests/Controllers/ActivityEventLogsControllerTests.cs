using System;
using System.Collections.Generic;
using Keebee.AAT.DataAccess.Tests.Keebee.AAT.DataAccess.Models;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Keebee.AAT.DataAccess.Tests.KeebeeAAT;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Simple.OData.Client;

namespace Keebee.AAT.DataAccess.Tests.Controllers
{
    [TestClass]
    public class ActivityEventLogsControllerTests
    {
        private const string OdataUri = "http://localhost/Keebee.AAT.DataAccess/odata/";

        public class ActivityEventLogs
        {
            public IEnumerable<ActivityEventLog> value { get; set; }
        }

        // ---- Container ----

        [TestMethod]
        public void OData_Container_GetActivityEventLogsConfigDetails()
        {
            // Arrange
            var container = new Container(new Uri(OdataUri));

            // Act
            var activityEventLogs = container
              .Execute<ActivityEventLogs>(
                  new Uri(
                      "/ActivityEventLogs?$apply=groupby((ConfigDetail/ConfigId, ConfigDetailId), aggregate(Id with sum as Total))",
                      UriKind.Relative))
                      ;


            // Assert
            Assert.IsNotNull(activityEventLogs);
        }

        // --- HttpRequest ---

        [TestMethod]
        public void OData_HttpRequest_GetActivityEventLogsConfigDetails()
        {
            // Arrange
            var url = $"{OdataUri}/ActivityEventLogs?$apply=groupby((ConfigDetail/ConfigId, ConfigDetailId), aggregate(Id with sum as Total))";
            var request = (HttpWebRequest)WebRequest.Create(url);

            ActivityEventLogs activityEventLogs;

            // Act
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    var data = new StreamReader(stream);
                    var result = data.ReadToEnd();
                    activityEventLogs = JsonConvert.DeserializeObject<ActivityEventLogs>(result);
                }
            }

            // Assert
            Assert.IsNotNull(activityEventLogs);
        }
    }
}
