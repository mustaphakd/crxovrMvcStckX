using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using StockExchange.Controllers;
using System.Web.Http;
using System.Net.Http;
using System.Threading;
using System.Net;

namespace StockExchange.Tests
{
    /// <summary>
    /// Test cases for the Secured Restful Web API endpoint
    /// </summary>
    public class SecureExchangeWebApiFacts
    {
        [Fact]
        public void Unauthorized_Access_Denied()
        {
            //Arrange
            HttpStatusCode statusCode = HttpStatusCode.OK;
            var configuration = new HttpConfiguration();

            WebApiConfig.Register(configuration);
            /*((NetworkCredential)CredentialCache.DefaultCredentials).UserName = " ";
            ((NetworkCredential)CredentialCache.DefaultNetworkCredentials).UserName = "";

            ((NetworkCredential)CredentialCache.DefaultCredentials).Domain = " dd";
            ((NetworkCredential)CredentialCache.DefaultNetworkCredentials).Domain = "dd";*/

            var principalCache = Thread.CurrentPrincipal; //may need it down there somewhere
            Thread.CurrentPrincipal = null;

            using(var server = new HttpServer(configuration))
            {
                //var client = new HttpClient { Credentials = null };
                //server.InnerHandler
                //or httpmessageinvoker
                using (var invoker = new HttpClient(server) )
                {
                    
                    using(var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/SecureStockExchange/msft"))
                    {
                        using(var response = invoker.SendAsync(request, CancellationToken.None).Result)
                        {
                            //Act
                            statusCode = response.StatusCode;
                        }
                    }
                }
            }
            Thread.CurrentPrincipal = principalCache; //just in case :)
            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode);

        }

    }
}
