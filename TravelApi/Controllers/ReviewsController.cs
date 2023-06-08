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
  public class ReviewsController : ControllerBase
  {
    private readonly TravelApiContext _db;
    private readonly UserManager<User> _userManager;

    public ReviewsController(UserManager<User> userManager, TravelApiContext db)
    {
      _db = db;
      _userManager = userManager;
    }

    [HttpPost]
    public async Task<ActionResult<Review>> PostReview(Review review)
    {
      Country thisCountry = await _db.Countries
                                        .Include(country => country.Reviews)
                                        .FirstOrDefaultAsync(country => country.CountryId == review.CountryId);
      User thisUser = await _userManager.FindByIdAsync(review.UserId);
      if (thisCountry == null)
      {
        return NotFound("this country doesn't exist");
      }
      else if (thisUser == null)
      {
        return NotFound("this user does not exist");
      }
      else
      {
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();
        thisUser.Reviews.Add(review);
        thisCountry.Reviews.Add(review);
        await _db.SaveChangesAsync();
        return Ok();
      }
    }

    [HttpGet]
    public async Task<List<Review>> GetReview(int pageNumber, int pageSize, string text, int countryId, string userId, string countryName, string userName, bool random = false)
    {
      IQueryable<Review> query = _db.Reviews.AsQueryable();

      if (text != null)
      {
        query = query.Where(entry => entry.Text == text);
      }

      if (countryId > 0)
      {
        query = query.Where(entry => entry.CountryId == countryId);
      }

      if (userId == null)
      {
        query = query.Where(entry => entry.UserId == userId);
      }

      if (countryName != null)
      {
        Country thisCountry = await _db.Countries.FirstOrDefaultAsync(c => c.Name == countryName);
        query = query.Where(entry => entry.CountryId == thisCountry.CountryId);
      }

      if (userName != null)
      {
        User thisUser = await _userManager.FindByNameAsync(userName);
        query = query.Where(entry => entry.UserId == thisUser.Id);
      }
      if (random)
      {
        Random randomInt = new Random();
        int id = randomInt.Next(1, _db.Reviews.ToList().Count);
        query = query.Where(r => r.ReviewId == id);
      }
      if (pageNumber > 0 && pageSize > 0)
      {
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize); 
      }
      return await query.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Review>> GetReview(int id)
    {
      Review review = await _db.Reviews.FindAsync(id);

      if (review == null)
      {
        return NotFound();
      }

      return review;
    }
  }
}