using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Data.Models;

namespace CoreCodeCamp
{
    public class TalkProfile : Profile
    {
        public TalkProfile()
        {
            CreateMap<Talk, TalkModel>().ReverseMap();
        }
    }
}