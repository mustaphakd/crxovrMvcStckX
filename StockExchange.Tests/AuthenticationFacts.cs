using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using StockExchange.Controllers;
using StockExchange.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace StockExchange.Tests
{
    /// <summary>
    /// Test cases validating User registration and login
    /// </summary>
    public class AuthenticationFacts
    {

        [Fact]
        public void AccountController_regristration_succeed()
        {
            //Arrange
            string _email = "haha@kolo.com";

            var registerViewModel = new RegisterViewModel() { Email = _email, Password = _email };
            var appUser = new ApplicationUser{ UserName = registerViewModel.Email, Email = registerViewModel.Email  };

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

            Mock<ApplicationUserManager> userManagerMock = new Mock<ApplicationUserManager>(userStoreMock.Object);
            userManagerMock.Setup(mck => mck.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns((ApplicationUser apUsr, string pwd) => {

                if (appUser != null && appUser.UserName == _email && appUser.Email == _email && !String.IsNullOrEmpty(pwd) && pwd == _email)
                    return Task.FromResult<IdentityResult>( IdentityResult.Success);

                return Task<IdentityResult>.Run<IdentityResult>(() => IdentityResult.Failed());
            
            });


            var authManagerMck = new Mock<IAuthenticationManager>();
            var  signInManagerMck = new Mock<ApplicationSignInManager>(userManagerMock.Object, authManagerMck.Object);


            var accountController = new AccountController(userManagerMock.Object, signInManagerMck.Object);

            //Act

           var tsk =  accountController.Register(registerViewModel);
           tsk.Wait();
           var reslt = tsk.Result;


            //Assert
           userManagerMock.Verify(mck => mck.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()));
            Assert.IsType<RedirectToRouteResult>(reslt);

        }


        [Fact]
        public void AccountController_Login_Denied()
        {
            //Arrange
            string _email = "haha@kolo.com";

            var registerViewModel = new RegisterViewModel() { Email = _email, Password = _email };
            var appUser = new ApplicationUser { UserName = registerViewModel.Email, Email = registerViewModel.Email };

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();

            Mock<ApplicationUserManager> userManagerMock = new Mock<ApplicationUserManager>(userStoreMock.Object);
            userManagerMock.Setup(mck => mck.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns((ApplicationUser apUsr, string pwd) =>
            {
                return Task<IdentityResult>.Run<IdentityResult>(() => IdentityResult.Failed());

            });

            var authManagerMck = new Mock<IAuthenticationManager>();

            //ApplicationSignInManager signInManager = Mock.Of<ApplicationSignInManager>();
            var signInManagerMck = new Mock<ApplicationSignInManager>(userManagerMock.Object, authManagerMck.Object);
            var accountController = new AccountController(userManagerMock.Object, signInManagerMck.Object);

            //Act

            var tsk = accountController.Register(registerViewModel);
            tsk.Wait();
            var reslt = tsk.Result;


            //Assert
            userManagerMock.Verify(mck => mck.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()));
            Assert.IsType<ViewResult>(reslt);
            Assert.IsType<RegisterViewModel>(((ViewResult)reslt).Model);

        }

        [Fact]
        public void AccountController_Login_Succeeded()
        {
            //Arrange
            var _email = "musmus@hello.com";
            var lgnVm = new LoginViewModel { Email = _email, Password = _email, RememberMe = false };

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMck =  new Mock<ApplicationUserManager>(userStoreMock.Object);


            var authManagerMck = new Mock<IAuthenticationManager>();

            Mock<ApplicationSignInManager> signInManagerMck = new Mock<ApplicationSignInManager>(userManagerMck.Object, authManagerMck.Object);

            signInManagerMck.Setup(mck => mck.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(
                (string str1, string str2, bool bl1, bool bl2) => Task.FromResult<SignInStatus>(SignInStatus.Success));

            var urlHlprMck = new Mock<UrlHelper>();
            urlHlprMck.Setup(hlpr => hlpr.IsLocalUrl(It.IsAny<string>())).Returns(false);

            var accountController = new AccountController(userManagerMck.Object, signInManagerMck.Object) { Url = urlHlprMck.Object };

            //Act
            var tsk = accountController.Login(lgnVm, "");
            tsk.Wait();
            var rslt = tsk.Result;

            //Assert
            Mock.Get(signInManagerMck.Object).Verify(mck => mck.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()));
            Assert.IsType<RedirectToRouteResult>(rslt);
            Assert.Equal("Index", ((RedirectToRouteResult)rslt).RouteValues["Action"]);
            Assert.Equal("Home", ((RedirectToRouteResult)rslt).RouteValues["Controller"]);
        }
    }

}
