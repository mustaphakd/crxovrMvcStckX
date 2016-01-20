using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using StockExchange.Models;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq.Expressions;
using StockExchange.Controllers;
using Moq.Protected;
using System.Web.Mvc;
using System.Security.Principal;
using System.Security.Claims;

namespace StockExchange.Tests
{
    /// <summary>
    /// Test cases for the StockController
    /// </summary>
    public class StockControllerFacts
    {

        [Fact]
        public void AddQuotes_toUser()
        {
            //Arrange
            var crlUsrId = Guid.NewGuid().ToString();
            var appDbCntxt = new Mock<ApplicationDbContext>();
            var stckLst = new List<Stock> { new Stock { Code = "MSFT", StockId = 1 }, new Stock { Code = "Orng", StockId = 2 }, new Stock { Code = "APPL", StockId = 3 } };
            var UsrLst = new List<ApplicationUser>() { 
            
            new ApplicationUser
            {
                Id = crlUsrId,

                Email = "koko@gmail.com",
                UserName = "Musmus",
                Stocks = stckLst
            }
            };
           var dbSetMck = new Mock<DbSet<ApplicationUser>>();
           dbSetMck.MockQueryableInterface(UsrLst);

           var dbSetStcksMck = new Mock<DbSet<Stock>>();
           dbSetStcksMck.MockQueryableInterface(stckLst);
           dbSetStcksMck.Setup(set => set.Add(It.IsAny<Stock>())).Callback((Stock val) =>
           {
               stckLst.Add(val);
           });

            appDbCntxt.Setup(cntxt => cntxt.Users).Returns(
             dbSetMck.Object
            );
            appDbCntxt.Setup(cntxt => cntxt.Stocks).Returns(
                dbSetStcksMck.Object
            );

            var dbCntxtMck = new Mock<ApplicationDbContext>();
            dbCntxtMck.Setup(cntxt => cntxt.Users).Returns(() => dbSetMck.Object);

            var claimsIdenty = new ClaimsIdentity(new GenericIdentity(UsrLst[0].UserName), new []{new Claim(ClaimTypes.NameIdentifier, crlUsrId) });
            
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMck =  new Mock<ApplicationUserManager>(userStoreMock.Object);
            userManagerMck.Setup(mck => mck.Users).Returns(dbSetMck.Object.AsQueryable());
            //Microsoft.Owin.OwinContext
            /*
            var dicMck = new Mock<IDictionary<string, object>>();
            dicMck.Setup( mck => mck[It.IsAny<string>()]).Returns(userManagerMck.Object);*/

            var cntxtDix = new Dictionary<string, Object>() { 
                { 
                    "owin.Environment", 
                    new Dictionary<string, Object>()
                    {
                        {String.Format("AspNet.Identity.Owin:{0}", typeof(ApplicationUserManager).AssemblyQualifiedName),  userManagerMck.Object}
                    }
                }            
            };
            var controllerContext = Mock.Of<ControllerContext>(cntxt => 

                cntxt.HttpContext.Items == cntxtDix &&

                cntxt.HttpContext.User == new ClaimsPrincipal(new[] { claimsIdenty }) 
                
                
                );
            var stckController = new StocksController(appDbCntxt.Object) { ControllerContext = controllerContext };
            

            //Act
            var usrLstVw = stckController.Create(new Stock { Code = "AAAA" }) as  RedirectToRouteResult;

            //Assert
            Assert.NotNull(usrLstVw);
            Assert.Collection<Stock>(stckLst, stck => { }, stck => { }, stck => { }, stck => { });

        }

        [Fact]
        public void List_UserQuotes_Succeed()
        {
            //Arrange
            var crlUsrId = Guid.NewGuid().ToString();
            var appDbCntxt = new Mock<ApplicationDbContext>();
            var stckLst = new List<Stock> { new Stock { Code = "MSFT", StockId = 1 }, new Stock { Code = "Orng", StockId = 2 }, new Stock { Code = "APPL", StockId = 3 } };
            var UsrLst = new List<ApplicationUser>() { 
            
            new ApplicationUser
            {
                Id = crlUsrId,

                Email = "koko@gmail.com",
                UserName = "Musmus",
                Stocks = stckLst
            }
            };
            var dbSetMck = new Mock<DbSet<ApplicationUser>>();
            dbSetMck.MockQueryableInterface(UsrLst);

            var dbSetStcksMck = new Mock<DbSet<Stock>>();
            dbSetStcksMck.MockQueryableInterface(stckLst);
            

            appDbCntxt.Setup(cntxt => cntxt.Users).Returns(
             dbSetMck.Object
            );
            appDbCntxt.Setup(cntxt => cntxt.Stocks).Returns(
                dbSetStcksMck.Object
            );

            var dbCntxtMck = new Mock<ApplicationDbContext>();
            dbCntxtMck.Setup(cntxt => cntxt.Users).Returns(() => dbSetMck.Object);

            var claimsIdenty = new ClaimsIdentity(new GenericIdentity(UsrLst[0].UserName), new[] { new Claim(ClaimTypes.NameIdentifier, crlUsrId) });

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMck = new Mock<ApplicationUserManager>(userStoreMock.Object);
            userManagerMck.Setup(mck => mck.Users).Returns(dbSetMck.Object.AsQueryable());
            //Microsoft.Owin.OwinContext
            /*
            var dicMck = new Mock<IDictionary<string, object>>();
            dicMck.Setup( mck => mck[It.IsAny<string>()]).Returns(userManagerMck.Object);*/

            var cntxtDix = new Dictionary<string, Object>() { 
                { 
                    "owin.Environment", 
                    new Dictionary<string, Object>()
                    {
                        {String.Format("AspNet.Identity.Owin:{0}", typeof(ApplicationUserManager).AssemblyQualifiedName),  userManagerMck.Object}
                    }
                }            
            };
            var controllerContext = Mock.Of<ControllerContext>(cntxt =>

                cntxt.HttpContext.Items == cntxtDix &&

                cntxt.HttpContext.User == new ClaimsPrincipal(new[] { claimsIdenty })


                );
            var stckController = new StocksController(appDbCntxt.Object) { ControllerContext = controllerContext };


            //Act
            var usrLstVw = stckController.Index() as ViewResult;

            //Assert
            Assert.NotNull(usrLstVw);
            Assert.Collection<Stock>(stckLst, stck => { }, stck => { }, stck => { });
    
        }

        [Fact]
        public void RemoveQuotes_FromUser_succeed()
        {
            //Arrange
            var crlUsrId = Guid.NewGuid().ToString();
            var appDbCntxt = new Mock<ApplicationDbContext>();
            var stckLst = new List<Stock> { new Stock { Code = "MSFT", StockId = 1 }, new Stock { Code = "Orng", StockId = 2 }, new Stock { Code = "APPL", StockId = 3 } };
            var UsrLst = new List<ApplicationUser>() { 
            
            new ApplicationUser
            {
                Id = crlUsrId,

                Email = "koko@gmail.com",
                UserName = "Musmus",
                Stocks = stckLst
            }
            };
            var dbSetMck = new Mock<DbSet<ApplicationUser>>();
            dbSetMck.MockQueryableInterface(UsrLst);

            var dbSetStcksMck = new Mock<DbSet<Stock>>();
            dbSetStcksMck.MockQueryableInterface(stckLst);
            dbSetStcksMck.Setup(set => set.Remove(It.IsAny<Stock>())).Callback((Stock val) =>
            {
                stckLst.Remove(val);
            });

            appDbCntxt.Setup(cntxt => cntxt.Users).Returns(
             dbSetMck.Object
            );
            appDbCntxt.Setup(cntxt => cntxt.Stocks).Returns(
                dbSetStcksMck.Object
            );
            #region OwinContext related
            var dbCntxtMck = new Mock<ApplicationDbContext>();
            dbCntxtMck.Setup(cntxt => cntxt.Users).Returns(() => dbSetMck.Object);

            var claimsIdenty = new ClaimsIdentity(new GenericIdentity(UsrLst[0].UserName), new[] { new Claim(ClaimTypes.NameIdentifier, crlUsrId) });

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMck = new Mock<ApplicationUserManager>(userStoreMock.Object);
            userManagerMck.Setup(mck => mck.Users).Returns(dbSetMck.Object.AsQueryable());
          
            var cntxtDix = new Dictionary<string, Object>() { 
                { 
                    "owin.Environment", 
                    new Dictionary<string, Object>()
                    {
                        {String.Format("AspNet.Identity.Owin:{0}", typeof(ApplicationUserManager).AssemblyQualifiedName),  userManagerMck.Object}
                    }
                }            
            };
#endregion
            var controllerContext = Mock.Of<ControllerContext>(cntxt =>

                cntxt.HttpContext.Items == cntxtDix &&

                cntxt.HttpContext.User == new ClaimsPrincipal(new[] { claimsIdenty })


                );
            var stckController = new StocksController(appDbCntxt.Object) { ControllerContext = controllerContext };


            //Act
            var usrLstVw = stckController.Delete(stckLst[0].StockId) as ViewResult;

            //Assert
            Assert.NotNull(usrLstVw);
            Assert.Collection<Stock>(stckLst, stck => { }, stck => { });
            Assert.Collection<Stock>(UsrLst[0].Stocks, stck => { }, stck => { });
        }

       
    }

    public static class ExtentionClasses
    {
        public static void MockQueryableInterface<T>(this Mock<DbSet<T>> mockedDbSet, List<T> set) where T : class
        {
            mockedDbSet.As<IQueryable<T>>().Setup(st => st.Provider).Returns(set.AsQueryable().Provider);
            mockedDbSet.As<IQueryable<T>>().Setup(st => st.ElementType).Returns(set.AsQueryable().ElementType);
            mockedDbSet.As<IQueryable<T>>().Setup(st => st.Expression).Returns(set.AsQueryable().Expression);
            mockedDbSet.As<IQueryable<T>>().Setup(st => st.GetEnumerator()).Returns(() => set.AsQueryable().GetEnumerator());
        }
    }
}
