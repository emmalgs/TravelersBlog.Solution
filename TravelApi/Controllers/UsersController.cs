using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApi.Models;
using Microsoft.AspNetCore.Identity;


namespace TravelApi.Controllers
{
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiController]
  [ApiVersion("1.0")]
  [ApiVersion("2.0")]

  public class UsersController : ControllerBase
  {
    private readonly TravelApiContext _db;
    private readonly UserManager<User> _userManager;

    public UsersController(UserManager<User> userManager, TravelApiContext db)
    {
      _db = db;
      _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
      if (ModelState.IsValid)
      {
        User user = new Models.User { UserName = model.Username, Email = model.Email };
        IdentityResult result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          return Ok(new { Message = "Registration successful" });
        }

        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }
      }

      return BadRequest(ModelState);
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
      var users = _userManager.Users.Include(u => u.Reviews);

      return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(string id)
    {
      User user = await _db.Users
                                  .Include(user => user.Reviews)
                                  .FirstOrDefaultAsync(user => user.Id == id);

      if (user == null)
      {
        return NotFound();
      }

      return user;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, User user)
    {
      if (id != user.Id)
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
    public async Task<IActionResult> DeleteUser(string id)
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

    [HttpPut("{userId}/reviews/{reviewId}")]
    public async Task<IActionResult> Put(string userId, int reviewId, Review review)
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
    public async Task<IActionResult> DeleteReview(int reviewId, string userId)
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

    private bool UserExists(string id)
    {
      return _db.Users.Any(e => e.Id == id);
    }

    private bool ReviewExists(int id)
    {
      return _db.Reviews.Any(e => e.ReviewId == id);
    }
  }
}