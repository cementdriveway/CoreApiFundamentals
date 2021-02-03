using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Data.Models;

namespace CoreCodeCamp
{
    public class SpeakerProfile : Profile
    {
        public SpeakerProfile()
        {
            CreateMap<Speaker, SpeakerModel>();
        }
    }
}