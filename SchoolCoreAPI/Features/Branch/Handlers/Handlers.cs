using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SchoolCoreAPI.Features.Branch.Commands;
using SchoolCoreAPI.Features.Branch.Queries;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Features.Branch.Handlers
{
    public class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, List<vbranches>>
    {
        private readonly APIContext _context;
        private readonly IDistributedCache _cache;
        private const string CacheKey = "GetAllBranches";
        public GetAllBranchesQueryHandler(APIContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<vbranches>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
        {
            // Try to retrieve data from Redis cache
            var cachedData = await _cache.GetStringAsync(CacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // Deserialize and return cached data
                return JsonConvert.DeserializeObject<List<vbranches>>(cachedData);
            }
            // If not cached, retrieve data from the database
            var branches = _context.vbranches.OrderByDescending(x => x.Id).ToList();
            if (branches != null && branches.Any())
            {
                // Serialize and store data in Redis
                var serializedData = JsonConvert.SerializeObject(branches);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10), // Cache expires after 10 minutes
                    SlidingExpiration = TimeSpan.FromMinutes(5)                // Reset expiration if accessed
                };
                await _cache.SetStringAsync(CacheKey, serializedData, cacheOptions);
            }

            return branches;
           // return Task.FromResult(branches);
            
        }
    }

    public class ViewBranchQueryHandler : IRequestHandler<ViewBranchQuery, vbranches>
    {
        private readonly APIContext _context;

        public ViewBranchQueryHandler(APIContext context)
        {
            _context = context;
        }
        
        public async Task<vbranches> Handle(ViewBranchQuery request, CancellationToken cancellationToken)
        {
            var objBranch = await _context.vbranches
             .AsNoTracking()
             .FirstOrDefaultAsync(m => m.Id == request.Id);
            return objBranch; // Returns null if no matching record is found
        }
    }

    public class SaveBranchCommandHandler : IRequestHandler<SaveBranchCommand, ServiceResult>
    {
        private readonly APIContext _context;
        private readonly IDistributedCache _cache;
        private const string CacheKey = "GetAllBranches";
        private readonly IMapper _mapper;
        public SaveBranchCommandHandler(APIContext context, IDistributedCache cache, IMapper mapper)
        {
            _context = context;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<ServiceResult> Handle(SaveBranchCommand request, CancellationToken cancellationToken)
        {
            ServiceResult result = new ServiceResult();

            try
            {
                Models.Branch branch;

                if (request.Model.Id > 0)
                {
                    // Update Branch
                    branch = await _context.Branch                       
                        .FirstOrDefaultAsync(m => m.Id == request.Model.Id && m.IsActive == ((int)ActiveState.Active).ToString());

                    if (branch == null)
                    {
                        //result.StatusId = (int)ServiceResultStatus.NotFound;
                        //result.Message = "Branch not found.";
                        return result;
                    }
                    branch = _mapper.Map<Models.Branch>(request.Model);
                    branch.LastModifiedDate = DateTime.Now;
                    // branchEntity.IsActive = branchEntity.IsActive;
                    branch.LastModifiedByUserId = request.UserId;
                    //_context.Branch.Update(branch);
                    _context.Entry(branch).State = EntityState.Modified;
                }
                else
                {
                    // Add Branch
                    branch = _mapper.Map<Models.Branch>(request.Model);
                    branch.AdminId = request.Model.AdminId > 0 ? request.Model.AdminId : 0;
                    branch.IsActive = ((int)ActiveState.Active).ToString();
                    branch.CreatedDate = DateTime.Now;
                    branch.CreatedByUserId = request.UserId;
                    branch.LastModifiedDate = DateTime.Now;
                    branch.LastModifiedByUserId = request.UserId;

                    _context.Branch.Add(branch);
                }

                var changes = await _context.SaveChangesAsync(cancellationToken);

                if (changes > 0)
                {
                    result.StatusId = request.Model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
                    result.RecordId = branch.Id;
                    await _cache.RemoveAsync(CacheKey);
                }
                else
                {
                    //result.StatusId = (int)ServiceResultStatus.NoChanges;
                    //result.Message = "No changes were made.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                //result.StatusId = (int)ServiceResultStatus.Error;
                //result.Message = "An error occurred while processing the request.";
            }

            return result;
        }

    }
}
