using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
//using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieAPI.Models;
using MovieAPI.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace MovieAPI.Controllers
{
    [ApiController]
    [Route("api/link")]
    public class ActorMovieLinkController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorMovieLinkController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieActorLink>>> ShowAllLink()
        {
            return await _context.MovieActorLinks.ToListAsync();
        }

        [HttpPost]
        [Route("AddCast")]
        public async Task<IActionResult> AddNewLink(List<MovieActorLink> Links)
        {
            

            foreach (var link in Links)
            {
                _context.MovieActorLinks.Add(link);
            }
            await _context.SaveChangesAsync();
            return Ok(new Response { Status = "Success", Message = "Cast Added successfully!" });

        }

        [HttpGet]
        [Route("cast/{id}")]
        public async Task<ActionResult<IEnumerable<Actor>>> Casts(long id)
        {
            var li = _context.MovieActorLinks.Where(x => x.Movieid == id).Select(st => st.Actorid).ToList();
            return await _context.Actors.Where(h => li.Contains(h.Id)).ToListAsync();
        }

        [HttpGet]
        [Route("movies/{id}")]
        public async Task<ActionResult<IEnumerable<Movie>>> Movies(long id)
        {
            
            var li = _context.MovieActorLinks.Where(x => x.Actorid == id).Select(st => st.Movieid).ToList();
            var lu = await _context.Movies.Where(h => li.Contains(h.Id)).ToListAsync();
            return lu;
        }

        [HttpGet]
        [Route("moviesbyuser/{user}")]
        public async Task<ActionResult<IEnumerable<Movie>>> MoviesByUsername(string user)
        {
            var id = _context.Actors.Where(x => x.Username == user).Select(st => st.Id).ToList().FirstOrDefault();
            var li = _context.MovieActorLinks.Where(x => x.Actorid == id).Select(st => st.Movieid).ToList();
            var lu = await _context.Movies.Where(h => li.Contains(h.Id)).ToListAsync();
            return lu;
        }
    }
}
