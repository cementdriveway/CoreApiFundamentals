using AutoMapper;
using CoreCodeCamp.Data.Models;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            CreateMap<Camp, CampModel>()
                .ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName));
            CreateMap<CampModel, Camp>();
        }
    }
}