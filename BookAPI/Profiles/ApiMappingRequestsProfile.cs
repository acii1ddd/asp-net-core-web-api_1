using AutoMapper;
using BLL.DTOs;
using BookAPI.Contracts.Requests;

namespace BookAPI.Profiles
{
    public class ApiMappingRequestsProfile : Profile
    {
        public ApiMappingRequestsProfile()
        {
            CreateMap<AddAuthorRequest, AuthorDTO>();
            CreateMap<FullUpdateAuthorRequest, AuthorDTO>();
            CreateMap<PartialUpdateAuthorRequest, AuthorDTO>();
        }
    }
}
