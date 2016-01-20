using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StockExchange.Controllers
{
    //The ASMX is secured.  only authorized clients can call it.
    [Authorize]
    [RoutePrefix("api/SecureStockExchange")]
    public class SecureStockExchangeController : ApiController
    {
        public SecureStockExchangeController()
        {
            var test = "";
        }
        /// <summary>
        /// returns stock price in ranfom between 1 and 1000
        /// </summary>
       
        private static Random _random = new Random();
        // GET: api/SecureStockExchange/MSFT

         [Route("{code}")]
        public double Get(string code)
        {
            if (Request == null || (Request.GetRequestContext() == null || Request.GetRequestContext().Principal == null) || User == null)
                throw new UnauthorizedAccessException();
            return _random.Next(1, 1000);
        }

        // GET: api/SecureStockExchange/fdsf&das
        public IEnumerable<Double> Get([FromUri]IEnumerable<string> codes)
        {
            if(codes != null)
            foreach (var code in codes)
                yield return Get(code);

            yield break;
        }

    }
}
