using System;
using System.Collections.Generic;

namespace CoreCodeCamp.Data.Models
{
    public class CampModel
    {
        public string Name { get; set; }
        public string Moniker { get; set; }
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        public int Length { get; set; } = 1;
        
        public string Venue { get; set; }

        public ICollection<TalkModel> Talks { get; set; }
    }
}