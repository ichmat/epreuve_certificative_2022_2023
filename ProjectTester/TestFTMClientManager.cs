using AppCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTester
{
    /// <summary>
    /// ⚠ pour ce test : il faut lancer le projet : WebApplicationAPI avec
    /// </summary>
    [TestClass]
    public class TestFTMClientManager
    {
        
        private static async Task<bool> PingWithHttpClient()
        {
            string hostUrl = "https://www.code4it.dev/";

            var httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri(hostUrl),
                Method = HttpMethod.Head
            };
            var result = await httpClient.SendAsync(request);
            return result.IsSuccessStatusCode;
        }

        private void CheckConnexion()
        {
            var t = PingWithHttpClient();
            t.Wait();
            Assert.IsTrue(t.Result, "not connected to internet");
        }

        [TestMethod]
        public async void TestConnexion()
        {
            CheckConnexion();
            await Task.Delay(5000); // wait for API to start
            FTMClientManager clientManager = new FTMClientManager();
            var t = clientManager.EstablishConnection();
            t.Wait();
            Assert.IsTrue(t.Result, "fail to get info");
        }
    }
}
