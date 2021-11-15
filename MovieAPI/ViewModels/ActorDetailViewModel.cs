using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieAPI.ViewModels
{
    public class ActorDetailViewModel 
    {
        public string Aname { get; set; }
        public long? Age { get; set; }
        public long? Experience { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public long? Phone { get; set; }
    }
}
