using AutoMapper;
using BLL.DTOs;
using DAL.Entities;

namespace BLL.Profiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<Author, AuthorDTO>().ReverseMap();
        }
    }
}
