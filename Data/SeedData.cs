using Atelier.Authorizations;
using Atelier.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Atelier.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@admin.com");
                await EnsureRole(serviceProvider, adminID, AuthorizationConstants.VetementAdministratorsRole);


                SeedDB(context, adminID);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            //if (userManager == null)
            //{
            //    throw new Exception("userManager is null");
            //}

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }

        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
            if (context.Vetement.Any())
            {
                return;
            }
            context.Vetement.AddRange(
                new Vetement { Nom = "T-shirt bleu", ProprietaireId = adminID, DateObtention = DateTime.Now, Description = "Allo", Type = "Haut", Image = "Allo" },
                new Vetement { Nom = "Camisole verte", ProprietaireId = adminID, DateObtention = DateTime.Now, Description = "Allo", Type = "Haut", Image = "Allo" },
                new Vetement { Nom = "Chienne de travail", ProprietaireId = adminID, DateObtention = DateTime.Now, Description = "Allo", Type = "Haut", Image = "Allo" },
                new Vetement { Nom = "Mom Jeans", ProprietaireId = adminID, DateObtention = DateTime.Now, Description = "Allo", Type = "Haut", Image = "Allo" },
                new Vetement { Nom = "Espadrilles", ProprietaireId = adminID, DateObtention = DateTime.Now, Description = "Allo", Type = "Haut", Image = "Allo" });
            context.SaveChanges();
        }
    }

}