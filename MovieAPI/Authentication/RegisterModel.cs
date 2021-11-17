using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Authentication
{
    public class RegisterModel
    {


       
        
        
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string Aname { get; set; }

        public string Photo { get; set; }

        public long? Age { get; set; }

        public double? Experience { get; set; }

        public string Gender { get; set; }

        public long? Phone { get; set; }

    }
}
