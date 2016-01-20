namespace StockExchange.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using StockExchange.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<StockExchange.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            
        }

        protected override void Seed(ApplicationDbContext context)
        {
            SeedRoles(context);
            var anyUsr = context.Users.Any(usr => usr.UserName.Equals("Pierre@yahoo.com"));
            if (!anyUsr)
            {
                AddRegularUsers(context, new[] { "Pierre@yahoo.com", "Francis@yahoo.com", "Anne@yahoo.com" }, "hello");
            }

            //TODO: auto create quotes for Pierre.
            //first make sure that he exist, retrieve him from the database. and then add the quotes and save.
        }

        private void AddRegularUsers(ApplicationDbContext context, IEnumerable<string> userNames, string pwd)
        {
            foreach (var userName in userNames)
            {
                if (!context.Users.Any(usr => usr.UserName.Equals(userName)))
                {
                    AddUser(context, userName, pwd);
                }
            }
        }

        private void AddUser(ApplicationDbContext context, string userName, string pwd)
        {
            var usrStore = new UserStore<ApplicationUser>(context);
            var usrManager = new UserManager<ApplicationUser>(usrStore);
            usrManager.PasswordValidator = new MinimumLengthValidator(3);


            var userTask = usrManager.CreateAsync(new ApplicationUser
            {
                UserName = userName,
                Email = userName,
                EmailConfirmed = true
            }, pwd);
            userTask.Wait();

            var result = userTask.Result;
        }
        private void SeedRoles(ApplicationDbContext context)
        {
            var anyRoles = context.Roles.Any(rl => rl.Name.Equals("Admin"));
            if (!anyRoles)
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManger = new RoleManager<IdentityRole>(roleStore);

                var adminRole = new IdentityRole("Admin");

                var tsk = roleManger.CreateAsync(adminRole);
                tsk.Wait();

                var result = tsk.Result;

                if (!result.Succeeded)
                    throw new InvalidOperationException();

                /* var awaiter = roleManger.CreateAsync(adminRole).GetAwaiter();

                 awaiter.OnCompleted(() =>
                 {
                     var result = awaiter.GetResult();

                     if (!result.Succeeded)
                         throw new InvalidOperationException();
                 }
                     );*/
            }
        }
    }
}
