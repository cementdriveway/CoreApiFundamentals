using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreCodeCamp.Data.Models
{
    public class CampModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Moniker { get; set; }

        public DateTime EventDate { get; set; } = DateTime.MinValue;

        [Range(1, 100)]
        public int Length { get; set; } = 1;

        public string Venue { get; set; }

        public ICollection<TalkModel> Talks { get; set; }
    }
}