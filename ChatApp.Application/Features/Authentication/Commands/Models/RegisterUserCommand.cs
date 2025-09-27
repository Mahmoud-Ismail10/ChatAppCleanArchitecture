using ChatApp.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Features.Authentication.Commands.Models
{
    public record RegisterUserCommand(
        string? Name,
        string? PhoneNumber,
        string? Email,
        IFormFile? ProfileImageUrl) : IRequest<ApiResponse<string>>;
}
