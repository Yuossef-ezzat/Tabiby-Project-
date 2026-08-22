using BLL;
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
using DAL.Repository;
using Microsoft.Extensions.Options;
using ServiceAbstractionLayer;

namespace PL.Extention
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add your custom services here
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<DataSeeder, DataSeeder>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IMedicationService, MedicationService>();
            services.AddScoped<INursingService, NursingService>();
            services.AddScoped<IConsultationService, ConsultationService>();
            services.AddScoped<IConsultationChatService, ConsultationChatService>();
            services.AddScoped<IConsultationReviewService, ConsultationReviewService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IProfileUserService, ProfileUserService>();
            services.Configure<PaymobSettings>(configuration.GetSection("Paymob"));
            services.AddHttpClient<IPaymobClient, PaymobClient>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<PaymobSettings>>().Value;

                client.BaseAddress = new Uri(settings.BaseUrl);
            });

            services.AddAutoMapper((x) => { }, typeof(DomainProfile).Assembly);
            services.AddSignalR();
            return services;
        }
    }
}
