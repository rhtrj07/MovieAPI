using System;
using System.Collections.Generic;

#nullable disable

namespace MovieAPI.Models
{
    public class Actor
    {
        public Actor()
        {
            MovieActorLinks = new HashSet<MovieActorLink>();
        }

        public long Id { get; set; }

        public string Aname { get; set; }
        public string Username { get; set; }
        public string Photo { get; set; }
        
        public long? Age { get; set; }
        public long? Experience { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public long? Phone { get; set; }

        public virtual ICollection<MovieActorLink> MovieActorLinks { get; set; }
    }
}
