using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApi.Models;

namespace TravelApi.Controllers.v2
{
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiController]
  [ApiVersion("2.0")]
  public class ReviewsController : ControllerBase
  {
    private readonly TravelApiContext _db;

    public ReviewsController(TravelApiContext db)
    {
      _db = db;
    }

    [MapToApiVersion("2.0")]
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

    [MapToApiVersion("2.0")]
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