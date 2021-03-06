using MovieAPI.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using MovieAPI.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MovieAPI.Controllers
{
    //[EnableCors("AllowAllHeaders")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        [Route("login")]
       // public async Task<IActionResult> Login(LoginModel model)
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMinutes(60),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    userRoles = await userManager.GetRolesAsync(user)


            });
            }
            return Unauthorized();
        }

        //[HttpPost]
        //[Route("register")]
        //public async Task<IActionResult> Register([FromBody] RegisterModel model)
        //{
        //    var userExists = await userManager.FindByNameAsync(model.Username);
        //    if (userExists != null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

        //    ApplicationUser user = new ApplicationUser()
        //    {
        //        Email = model.Email,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.Username
        //    };
        //    var result = await userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        //    return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        //}

        [HttpGet]
        [Route("user")]
        public UserInfo Username()
        {
            UserInfo info = new UserInfo();
            ApplicationUser users;
            String UserRole;

            var user = User.Identity.Name;

            if(User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    UserRole = "Admin";
                else
                    UserRole = "Actor";
            }
            else
            {
                UserRole = null;
            }

           

            info.Role = UserRole;
            info.Username = user;
            info.isAuthenticated = User.Identity.IsAuthenticated;

            return info;

        }

        //public async string GetUserRole(string EmailID, string Password)
        //{
        //    var user = await userManager.FindByEmailAsync(EmailID);
        //    string rolename = await userManager.GetRolesAsync(user.I).FirstOrDefault();
        //    return rolename;
        //}

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin( RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.Actor))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Actor));

            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            string usernam = _context.Actors.Where(x => x.Id == id).Select(x => x.Username).ToList().FirstOrDefault();

            var actor = await _context.Actors.FindAsync(id);
            var user = await userManager.FindByNameAsync(usernam);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User Not found" });
            }
            else
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    var lst = _context.MovieActorLinks.Where(x => x.Actorid == id);
                    foreach (var link in lst)
                    {
                        _context.MovieActorLinks.Remove(link);
                    }
                    await _context.SaveChangesAsync();
                    _context.Actors.Remove(actor);
                    await _context.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                
            }
        }
    }
}
