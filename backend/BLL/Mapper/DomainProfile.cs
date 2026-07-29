using AutoMapper;
using BLL.Dtos.Appointment;
using BLL.Dtos.Consultion;
using BLL.Dtos.Doctor;
using BLL.Dtos.Medication;
using BLL.Dtos.Order;
using BLL.Dtos.Schedule;
using DAL.Models.AppointmentModule;
using DAL.Models.Consultation;
using DAL.Models.OrderModule;
using DAL.Models.Users;
using DomainLayer.Models.BasketModule;
using Shared.DTOs;
using BLL.Dtos.Nursing;
using DAL.Models.NursingModule;

namespace BLL.Mapper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Fullname : string.Empty))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Fullname : string.Empty))
                .ForMember(dest => dest.StatusText, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<CreateAppointmentDto, Appointment>()
                .ForMember(dest => dest.AppointmentTime, opt => opt.Ignore());
            CreateMap<UpdateAppointmentDto, Appointment>()
                .ForMember(dest => dest.ScheduleId, opt =>
                {
                    opt.PreCondition(src => src.ScheduleId.HasValue);
                    opt.MapFrom(src => src.ScheduleId!.Value);
                })
                .ForMember(dest => dest.AppointmentDate, opt =>
                {
                    opt.PreCondition(src => src.AppointmentDate.HasValue);
                    opt.MapFrom(src => src.AppointmentDate!.Value);
                })
                .ForMember(dest => dest.Notes, opt => opt.Condition(src => src.Notes != null));

            
            CreateMap<DoctorSchedule, DoctorScheduleDto>().ReverseMap();
            CreateMap<DoctorSchedule, CreateDoctorScheduleDto>().ReverseMap();
            CreateMap<DoctorSchedule, UpdateDoctorScheduleDto>().ReverseMap().ForMember(dest=>dest.Id,opt=>opt.Ignore());

            CreateMap<DoctorSchedule, AvailableDoctorSlotDto>()
                    .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => true));

            
            CreateMap<Medication, MedicationDto>().ReverseMap();
            CreateMap<Medication, CreateMedicationDto>()
                        .ForMember(dest => dest.Image, opt => opt.Ignore())
                        .ReverseMap();
            CreateMap<Medication, UpdateMedicationDto>()
            .ForMember(dest => dest.Image, opt => opt.Ignore())
            .ReverseMap();
            CreateMap<Medication, AllMedicationDto>().ReverseMap();

            
            CreateMap<Consultation, ConsultationDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Fullname : string.Empty))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Fullname : string.Empty))
                .ReverseMap();
            CreateMap<Consultation, CreateConsultationDto>().ReverseMap();

            CreateMap<ConsultationMessage, ConsultationMessageDto>().ReverseMap();
            CreateMap<ConsultationMessage, SendMessageDto>().ReverseMap();

            CreateMap<ConsultationReview, ConsultationReviewDto>().ReverseMap();
            CreateMap<ConsultationReview, CreateConsultationReviewDto>().ReverseMap();

            
            CreateMap<Doctor, DoctorInfoDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Fullname))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber));

            
            CreateMap<Nurse, BLL.Dtos.Nursing.NurseInfoDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Fullname))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber));

            
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItem))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Fullname : string.Empty));
            CreateMap<Order, CreateOrderDto>().ReverseMap();

            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<OrderItem, CreateOrderItemDto>().ReverseMap();

            
            CreateMap<OrderAddress, OrderAddressDto>().ReverseMap();


            CreateMap<BasketItem, BasketItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Medication.Name))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.Medication.PictureUrl))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.Price * src.Quantity));

            CreateMap<CustomerBasket, BasketDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.BasketItems))
                .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src =>
                    src.BasketItems.Sum(i => i.Price * i.Quantity)))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src =>
                    src.BasketItems.Sum(i => i.Price * i.Quantity) + src.ShippingPrice));

            
            CreateMap<NursingRequest, NursingRequestDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Fullname : string.Empty))
                .ForMember(dest => dest.NurseName, opt => opt.MapFrom(src => src.Nurse != null ? src.Nurse.Fullname : string.Empty))
                .ReverseMap();
            CreateMap<NursingReview, NursingReviewDto>().ReverseMap();
            CreateMap<NursingReview, CreateNursingReviewDto>().ReverseMap();

            
            CreateMap<ApplicationUser, BLL.Dtos.Admin.AdminUserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Fullname))
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ForMember(dest => dest.Specialty, opt => opt.Ignore())
                .ForMember(dest => dest.Location, opt => opt.Ignore())
                .ForMember(dest => dest.Specialization, opt => opt.Ignore())
                .ForMember(dest => dest.PharmacyName, opt => opt.Ignore());
        }
    }
}