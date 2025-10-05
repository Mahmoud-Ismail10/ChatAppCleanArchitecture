using ChatApp.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Features.Users.Commands.Models
{
    public record UpdateProfileImageCommand(IFormFile ProfileImage) : IRequest<ApiResponse<string>>;
}
