using BLL.Abstractions;
using BLL.Abstractions.Errors;
using BLL.Abstractions.Errrors;
using BLL.Dtos.Payment.paymob;
using BLL.Services.AbstractServices.PaymobModule;
using DAL.Models.PaymentModule;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace BLL.Services.ImplementationService.PaymobModule
{
    public class PaymobClient: IPaymobClient
    {
        private readonly HttpClient _httpClient;
        private readonly PaymobSettings _settings;

        public PaymobClient(HttpClient httpClient, IOptions<PaymobSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Token", _settings.SecretKey);
        }
        public async Task<Result<CreateIntentionResponse>> CreateIntentionAsync(Payment payment)
        {
            if(payment.IntegrationId is null)
                return Result<CreateIntentionResponse>.Failure(PaymentError.InvalidIntegrationId);
            var request = new CreateIntentionRequest
            {
                Amount = (int)(payment.Amount * 100),
                Currency = payment.Currency,
                PaymentMethods = new List<int> { payment.IntegrationId.Value },
                BillingData = new BillingData
                {
                    FullName = payment.Order.Patient.Fullname,
                    Email = payment.Order.Patient.Email,
                    PhoneNumber = payment.Order.Patient.PhoneNumber,
                    Apartment = payment.Order.Address.Apartment,
                    Floor = payment.Order.Address.Floor,
                    Building = payment.Order.Address.Building,
                    Street = payment.Order.Address.Street,
                    City = payment.Order.Address.City,
                    Country = payment.Order.Address.Country,

                }};
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("v1/intention/", request);
            if(!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return Result<CreateIntentionResponse>.Failure(new Errror(PaymentError.FailedToCreateIntention.Code, error));
            }
            var body = await response.Content.ReadFromJsonAsync<CreateIntentionResponse>();
            if (body is null)
                return Result<CreateIntentionResponse>.Failure(new Errror(PaymentError.FailedToCreateIntention.Code, "Paymob returned an empty CreateIntention response."));
            return Result<CreateIntentionResponse>.Success(new CreateIntentionResponse
            {
                Id = body.Id,
                ClientSecret = body.ClientSecret
            });
        }

        public Task<Result<RefundResponse>> RefundAsync(string transactionId, decimal amount)
        {
            throw new NotImplementedException();
        }
    }
}
