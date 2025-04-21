using MediatR;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Features.Branch.Queries
{
    public record GetAllBranchesQuery : IRequest<List<vbranches>>;
    public record ViewBranchQuery(int Id) : IRequest<vbranches>;
}
