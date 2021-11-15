using System;
using System.Collections.Generic;

#nullable disable

namespace MovieAPI.Models
{
    public class Movie
    {
        public Movie()
        {
            MovieActorLinks = new HashSet<MovieActorLink>();
        }

        public long Id { get; set; }
        public string Mname { get; set; }
        public double? Duration { get; set; }
        public string Genre { get; set; }
        public DateTime? Releasedate { get; set; }
        public double? Rating { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }


        public virtual ICollection<MovieActorLink> MovieActorLinks { get; set; }
    }
}
