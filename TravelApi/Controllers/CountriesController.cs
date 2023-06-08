using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApi.Models;

namespace TravelApi.Controllers
{   
  [Route("api/[controller]")]
  [ApiController]
  public class CountriesController : ControllerBase
  {
    private readonly TravelApiContext _db;

    public CountriesController(TravelApiContext db)
    {
      _db = db;
    }

    // GET api/countries
    [HttpGet]
    public async Task<List<Country>> Get(int pageNumber, int pageSize, string name, string language, int population, string climate, string sortBy, bool random=false)
    {
      IQueryable<Country> query = _db.Countries
                                      .Include(country => country.Reviews)
                                      .AsQueryable();

      if (name != null)
      {
        query = query.Where(entry => entry.Name == name);
      }

      if (language != null)
      {
        query = query.Where(entry => entry.Language == language);
      }

      if (population > 0)
      {
        query = query.Where(entry => entry.Population >= population);
      }

      if (climate != null)
      {
        query = query.Where(entry => entry.Climate == climate);
      }

      if (sortBy != null)
      {
        if (sortBy.ToLower() == "popular")
        {
          query = query.OrderByDescending(c => c.Reviews.Count);
        }
        else if (sortBy.ToLower() == "unpopular")
        {
          query = query.OrderBy(c => c.Reviews.Count);
        }      }

      if (random)
      {
        Random randomInt = new Random();
        int id = randomInt.Next(1, _db.Countries.ToList().Count);
        query = query.Where(c => c.CountryId == id);
      }
      if (pageNumber > 0 && pageSize > 0)
      {
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize); 
      }
      return await query.ToListAsync();
    }

    // GET: api/Countries/7
    [HttpGet("{id}")]
    public async Task<ActionResult<Country>> GetCountry(int id)
    {
      Country country = await _db.Countries
                                  .Include(country => country.Reviews)
                                  .FirstOrDefaultAsync(country => country.CountryId == id);

      if (country == null)
      {
        return NotFound();
      }

      return country;
    }

    // POST api/countries
    [HttpPost]
    public async Task<ActionResult<Country>> Post([FromBody] Country country)
    {
      _db.Countries.Add(country);
      await _db.SaveChangesAsync();
      return CreatedAtAction(nameof(GetCountry), new { id = country.CountryId }, country);
    }

    // PUT: api/Countries/7
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Country country)
    {
      if (id != country.CountryId)
      {
        return BadRequest();
      }

      _db.Countries.Update(country);

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!CountryExists(id))
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

    // DELETE: api/Countries/7
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
      Country country = await _db.Countries.FindAsync(id);
      if (country == null)
      {
        return NotFound();
      }

      _db.Countries.Remove(country);
      await _db.SaveChangesAsync();

      return NoContent();
    }

    private bool CountryExists(int id)
    {
      return _db.Countries.Any(e => e.CountryId == id);
    }

  }
}

