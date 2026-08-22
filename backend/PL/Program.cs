using BLL;
using BLL.Hubs;
using BLL.Mapper;
using BLL.Services.AbstractServices;
using BLL.Services.AbstractServices.Admin;
using BLL.Services.AbstractServices.AppointmentModule;
using BLL.Services.AbstractServices.ConsultationModule;
using BLL.Services.AbstractServices.MedicationModule;
using BLL.Services.AbstractServices.PaymobModule;
using BLL.Services.AbstractServices.Users;
using BLL.Services.ImplementationService;
using BLL.Services.ImplementationService.Admin;
using BLL.Services.ImplementationService.AppointmentModule;
using BLL.Services.ImplementationService.ConsultationModule;
using BLL.Services.ImplementationService.MedicationModule;
using BLL.Services.ImplementationService.NursingModule;
using BLL.Services.ImplementationService.PaymobModule;
using DAL.Data;
using DAL.Models.Users;
using DAL.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PL.Extention;
using PL.Utilites;
using ServiceAbstractionLayer;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using TalabatDemo.CustomMiddleWares;

namespace PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationServices(builder.Configuration)
                .AddEndpointsApiExplorer()
                .AddSwagger()
                .AddAuthentication(builder.Configuration)
                .AddDatabase(builder.Configuration)
                .AddCors(options =>
                {
                    options.AddPolicy("AllowLocalhost", builder =>
                    {
                        builder
                            .WithOrigins("http://localhost:3000", "http://localhost:3001")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
                })
                .AddCustomRateLimiting()
                .AddControllers();

            var app = builder.Build();

            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.ConfigObject = new ConfigObject()
                    {
                        DisplayRequestDuration = true
                    };
                    options.DocumentTitle = "Tabiby APP";
                    options.DocExpansion(DocExpansion.None);
                    options.EnableFilter();
                    options.EnablePersistAuthorization();
                });
            }
            
            using var scope = app.Services.CreateScope();
            var seed = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            await seed.SeedDatabaseAsync();

            EmailSettings.Initialize(app.Configuration);

            app.UseHttpsRedirection();
            app.UseMiddleware<CustomExceptionHandlerMiddleWare>();
            app.UseCors("AllowLocalhost");
            app.UseRouting();
            app.UseAuthentication();
            app.UseRateLimiter();
            app.UseAuthorization();


            app.UseStaticFiles();
            app.MapHub<NotificationHub>("/notificationHub");
            app.MapHub<ChatHub>("/chatHub");
            app.MapControllers();

            app.Run();
        }
    }
}
