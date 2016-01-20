using Moq;
using StockExchange.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Xunit;

namespace StockExchange.Tests
{
    public class IndexPageFacts
    {
       
        public void Unauthorized_accessToIndexPage_Denied()
        {
            //Arrange

            //Act
            var view = (new HomeController()).Index();

            //Assert
            Assert.IsType<ViewResult>(view);
            Assert.Equal(((ViewResult)view).ViewName, "Login");
        }

         [Fact]
        public void Authorized_accessToIndexPage_Granted()
        {
            //Arrange

            var userMock = Mock.Of<ClaimsPrincipal>(clmPr => clmPr.Identity == new ClaimsIdentity(new GenericIdentity("Payu","ASPNET")));

            var controllerCntxt = Mock.Of<ControllerContext>(cntrlr => 
                cntrlr.HttpContext.User == userMock && 
                cntrlr.HttpContext.Request.IsAuthenticated == true
                
                );
             //cntrlr.HttpContext == Mock.Of<HttpContextBase>(cntxt =>
                    //cntxt.User == userMock &&
                    //cntxt.Request.IsAuthenticated == true
             //
            //Act
            var view = (new HomeController() { ControllerContext = controllerCntxt }).Index();

            //Assert
            Assert.IsType<RedirectToRouteResult>(view);
            Assert.Equal(((RedirectToRouteResult)view).RouteValues["Action"], "Index");
            Assert.Equal(((RedirectToRouteResult)view).RouteValues["controller"], "Stocks");

        }
    }
}
