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
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("user")]
        public string Username()
        {
           // if(User.Identity.Actor != null )

            var user = User.Identity.Name;
            //var users = new ApplicationUser { UserName = model.Email, Email = model.Email }; 
            //var userRoles = await _userManager.

            return user;

        }


        // GET: api/Movies
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            return await _context.Movies.ToListAsync();
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(long id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<ActionResult<Movie>> GetMovieEdit(long id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }

        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {

            var recDate = movie.MovieActorLinks;
            var new_actor = movie.MovieActorLinks.Select(o => o.Actorid).ToList();
            var MovieActor = _context.MovieActorLinks;
            var old_actor = MovieActor.Where(p => p.Movieid == id).Select(q => q.Actorid).ToList();
            var newDate = recDate.Where(y => (old_actor.All(p2 => p2 != y.Actorid))).ToList();
            var removeList = MovieActor.Where(x => (new_actor.All(p2 => p2 != x.Actorid)) && (x.Movieid == id));


            var num = removeList.ToList();

            foreach (var remove in removeList)
            {
                _context.MovieActorLinks.Remove(remove);
            }

            foreach (var link in newDate)
            {
                _context.MovieActorLinks.Add(link);
            }
            await _context.SaveChangesAsync();

            movie.Id = id;

            _context.Entry(movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
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

        // POST: api/Movies
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie (Movie movie )
        {
            
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();


            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovie(long id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            var lst = _context.MovieActorLinks.Where(x => x.Movieid == id);

            foreach (var link in lst)
            {
                _context.MovieActorLinks.Remove(link);
            }
            await _context.SaveChangesAsync();
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return movie;
        }
       // [Route("user")]
        

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}