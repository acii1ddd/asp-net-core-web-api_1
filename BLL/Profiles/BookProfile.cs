using AutoMapper;
using BLL.DTOs;
using DAL.Entities;

namespace BLL.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDTO>().ReverseMap();
        }
    }
}
