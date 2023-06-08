using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApi.Models;

namespace TravelApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class UsersController : ControllerBase
  {
    private readonly TravelApiContext _db;

    public UsersController(TravelApiContext db)
    {
      _db = db;
    }

    [HttpGet]
    public async Task<List<User>> Get(string username)
    {
      IQueryable<User> query = _db.Users 
                                  .Include(user => user.Reviews)
                                  .AsQueryable();
      if (username != null)
      {
        query = query.Where(entry => entry.UserName == username);
      }

      return await query.ToListAsync();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
      User user = await _db.Users
                                  .Include(user => user.Reviews)
                                  .FirstOrDefaultAsync(user => user.UserId == id);

      if (user == null)
      {
        return NotFound();
      }

      return user;
    }

    

    [HttpPost]
    public async Task<ActionResult<User>> Post([FromBody] User user)
    {
      _db.Users.Add(user);
      await _db.SaveChangesAsync();
      return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, User user)
    {
      if (id != user.UserId)
      {
        return BadRequest();
      }

      _db.Users.Update(user);

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!UserExists(id))
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
      User user = await _db.Users.FindAsync(id);
      if (user == null)
      {
        return NotFound();
      }

      _db.Users.Remove(user);
      await _db.SaveChangesAsync();

      return NoContent();
    }
    private bool UserExists(int id)
    {
      return _db.Users.Any(e => e.UserId == id);
    }

    [HttpPut("{userId}/reviews/{reviewId}")]
    public async Task<IActionResult> Put(int userId, int reviewId, Review review)
    {
      if (reviewId != review.ReviewId)
      {
        return BadRequest();
      }
      else if (userId != review.UserId)
      {
        return Unauthorized();
      }
      else
      {
        _db.Reviews.Update(review);
      }

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!ReviewExists(reviewId))
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
    [HttpDelete("{userId}/reviews/{reviewId}")]
    public async Task<IActionResult> DeleteReview(int reviewId, int userId)
    {
      Review review = await _db.Reviews.FindAsync(reviewId);
      if (review == null)
      {
        return NotFound();
      }
      else if (userId != review.UserId)
      {
        return Unauthorized("You are not the owner of this review");
      }

      _db.Reviews.Remove(review);
      await _db.SaveChangesAsync();

      return NoContent();
    }

    private bool ReviewExists(int id)
    {
      return _db.Reviews.Any(e => e.ReviewId == id);
    }
  }
}