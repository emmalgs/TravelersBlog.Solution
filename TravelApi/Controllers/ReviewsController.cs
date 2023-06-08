using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApi.Models;

namespace TravelApi.Controllers.v1
{
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiController]
  [ApiVersion("1.0")]
  [ApiVersion("2.0")]
  public class ReviewsController : ControllerBase
  {
    private readonly TravelApiContext _db;

    public ReviewsController(TravelApiContext db)
    {
      _db = db;
    }

    [HttpGet]
    public async Task<List<Review>> Get(int pageNumber, int pageSize, string text, int countryId, int userId, string countryName, string userName, bool random = false)
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

      if (userId > 0)
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
        User thisUser = await _db.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        query = query.Where(entry => entry.UserId == thisUser.UserId);
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

    [MapToApiVersion("1.0")]
    [HttpPost]
    public async Task<ActionResult<Review>> Post([FromBody] Review review)
    {
      Country thisCountry = await _db.Countries
                                        .Include(country => country.Reviews)
                                        .FirstOrDefaultAsync(country => country.CountryId == review.CountryId);
      User thisUser = await _db.Users
                              .Include(user => user.Reviews)
                              .FirstOrDefaultAsync(user => user.UserId == review.UserId);
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
        // review.CountryId = thisCountry.CountryId;
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();
        thisUser.Reviews.Add(review);
        thisCountry.Reviews.Add(review);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, review);
      }
    }
  }
}