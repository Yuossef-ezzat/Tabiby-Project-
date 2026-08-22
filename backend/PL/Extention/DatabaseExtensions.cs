using DAL.Data;
using DAL.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace PL.Extention
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection  AddDatabase(this IServiceCollection services , IConfiguration configuration)
        {


            services.AddDbContext<TabibyDbContext>(
                options => {
                    options.UseSqlServer(configuration.GetConnectionString("TabibyDbContext"));
                }
            );
            services.AddIdentityCore<ApplicationUser>(
                options =>
                {

                })
                .AddRoles<IdentityRole<int>>()
                .AddRoleManager<RoleManager<IdentityRole<int>>>()
                .AddEntityFrameworkStores<TabibyDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
