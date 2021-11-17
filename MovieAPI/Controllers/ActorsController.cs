using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MovieAPI.Models;
using MovieAPI.Authentication;

namespace MovieAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;



        public ActorsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }

        

        // GET: api/Actors
        [HttpGet]
       
        public async Task<ActionResult<IEnumerable<Actor>>> GetMovies()
        {
            return await _context.Actors.ToListAsync();
        }

        // GET: api/Actors/5

        [HttpGet("{id}")]
        public async Task<ActionResult<Actor>> GetActor(long id)
        {
            var actor = await _context.Actors.FindAsync(id);

            if (actor == null)
            {
                return NotFound();
            }

            return actor;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("Edit")]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActorEdit()
        {
            return await _context.Actors.ToListAsync();
        }

        [HttpGet("curruser")]
        public async Task<ActionResult<Actor>> GetActorByUsername()
        {
            string user = User.Identity.Name;

            var id = _context.Actors.Where(x => x.Username == user).Select(x => x.Id).ToList().FirstOrDefault();

            var actor = await _context.Actors.FindAsync(id);

            if (actor == null)
            {
                return NotFound();
            }

            return actor;
        }




        [HttpGet]
        [Route("profile")]
        public async Task<ActionResult<Actor>> GetActorProfile()
        {
            var user = User.Identity.Name;
            var ids = _context.Actors.Where(st => st.Username == user)
                                    .Select(st => new {Id = st.Id }).ToList().FirstOrDefault();
            long id = ids.Id;

            var actor = await _context.Actors.FindAsync(id);

            if (actor == null)
            {
                return NotFound();
            }

            return actor;
        }

        // PUT: api/actors/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActor(int id, Actor actor)
        {
            if (id != actor.Id)
            {
                return BadRequest();
            }

            _context.Entry(actor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Actors
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("addnew")]
        public async Task<IActionResult> AddNew( RegisterModel model)
        {
            Actor actor = new Actor();

            actor.Username = model.Username;
            actor.Aname = model.Aname;
            actor.Email = model.Email;
            actor.Photo = model.Username;
            actor.Age = model.Age;
            actor.Experience = model.Experience;
            actor.Gender = model.Gender;
            actor.Phone = model.Phone;


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

            if (await roleManager.RoleExistsAsync(UserRoles.Actor))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Actor);
            }

            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });

        }
        

        public async Task<ActionResult<Movie>> PostActor(Actor actor)
        {

            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = actor.Id }, actor);
        }

        //[Authorize(Roles = UserRoles.Admin)]
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Movie>> DeleteActor(int id)
        //{
        //    var movie = await _context.Movies.FindAsync(id);
        //    if (movie == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Movies.Remove(movie);
        //    await _context.SaveChangesAsync();

        //    return movie;
        //}
        //[Route("user")]


        private bool ActorExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
