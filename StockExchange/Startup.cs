using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StockExchange.Startup))]
namespace StockExchange
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
