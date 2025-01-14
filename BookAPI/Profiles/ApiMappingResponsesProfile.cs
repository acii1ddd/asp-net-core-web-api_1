using AutoMapper;
using BLL.DTOs;
using BookAPI.Contracts.Responses;

namespace BookAPI.Profiles
{
    public class ApiMappingResponsesProfile : Profile
    {
        public ApiMappingResponsesProfile()
        {
            CreateMap<AuthorDTO, GetAuthorResponse>();
            CreateMap<AuthorDTO, GetAuthorWithBooksResponse>();
            CreateMap<BookDTO, GetBookResponse>();
        }
    }
}
