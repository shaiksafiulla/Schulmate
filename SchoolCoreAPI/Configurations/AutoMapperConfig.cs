using AutoMapper;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig() { 
            CreateMap<Branch, BranchDTO>().ReverseMap();
        }
    }
}
