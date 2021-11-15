using System;
using System.Collections.Generic;

#nullable disable

namespace MovieAPI.Models
{
    public class MovieActorLink
    {
        public long Id { get; set; }
        public long? Actorid { get; set; }
        public long? Movieid { get; set; }

        public virtual Actor Actor { get; set; }
        public virtual Movie Movie { get; set; }
    }
}
