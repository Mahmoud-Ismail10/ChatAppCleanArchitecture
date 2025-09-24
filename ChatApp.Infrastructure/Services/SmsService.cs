using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Helpers;
using Serilog;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ChatApp.Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        #region Fields
        private readonly TwilioSettings _twilio;
        #endregion

        #region Constructors
        public SmsService(TwilioSettings twilio)
        {
            _twilio = twilio;
            TwilioClient.Init(_twilio!.AccountSID, _twilio.AuthToken);
        }
        #endregion

        #region Functions
        public async Task<string> SendSmsAsync(string phoneNumber, string body)
        {
            try
            {
                var result = await MessageResource.CreateAsync(
                body: body,
                from: new Twilio.Types.PhoneNumber(_twilio.PhoneNumber),
                to: new Twilio.Types.PhoneNumber(phoneNumber));

                if (result.ErrorCode != null)
                    return "Failed";
                return "Success";
            }
            catch (Exception ex)
            {
                Log.Error("Failed to send SMS: {Message}", ex.InnerException?.Message ?? ex.Message);
                return "Failed";
            }
        }
        #endregion
    }
}
