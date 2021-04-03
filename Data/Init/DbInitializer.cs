using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepairApp.Data.Init;
using RepairApp.Models;
using RepairApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairApp.Data.Init
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
     

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
           
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }
            if (_db.Roles.Any(r => r.Name == SD.Admin)) return;

            _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.User)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new IdentityUser
            {
                UserName = "admin@admin.pl",
                Email = "admin@admin.pl",
                EmailConfirmed = true,              
            }, "Admin1234!").GetAwaiter().GetResult();


            _userManager.CreateAsync(new IdentityUser
            {
                UserName = "user@user.pl",
                Email = "user@user.pl",
                EmailConfirmed = true,
            }, "User1234!").GetAwaiter().GetResult();

            IdentityUser admin = _db.Users.Where(u => u.Email == "admin@admin.pl").FirstOrDefault();
            _userManager.AddToRoleAsync(admin, SD.Admin).GetAwaiter().GetResult();


            IdentityUser user = _db.Users.Where(u => u.Email == "user@user.pl").FirstOrDefault();
            _userManager.AddToRoleAsync(user, SD.User).GetAwaiter().GetResult();
        }
    }   
}
