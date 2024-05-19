using AutoMapper;
using FrisörenSörenAPI.Dto;
using FrisörenSörenModels;

namespace FrisörenSörenAPI.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Customer, CustomerDto>();
        }
    }
}
