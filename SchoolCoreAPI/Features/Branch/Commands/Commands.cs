using MediatR;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Features.Branch.Commands
{
    public record SaveBranchCommand(Models.BranchDTO Model, int UserId) : IRequest<ServiceResult>;
    public record DeleteBranchCommand(int Id, int UserId) : IRequest<ServiceResult>;
}
