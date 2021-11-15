using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieAPI.Models
{
    public class UserInfo
    {
        public string Role { get; set; }
        public string Username { get; set; }
        public bool isAuthenticated { get; set; }
    }
}
