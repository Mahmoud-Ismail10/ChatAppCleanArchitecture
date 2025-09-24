using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Authentication.Commands.Models
{
    public record VerifyOtpCommand(string Otp, string PhoneNumber) : IRequest<ApiResponse<string>>;
}
