using Microsoft.EntityFrameworkCore;

namespace TravelApi.Models
{
  public class TravelApiContext : DbContext
  {
    public DbSet<Country> Countries { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<User> Users { get; set; }

    public TravelApiContext(DbContextOptions<TravelApiContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      builder.Entity<Country>()
        .HasData(
          new Country { CountryId = 1, Name = "Striped Umbrella", Language = "Shade", Climate = "Shady and Sandy", Population = 4999999 },
          new Country { CountryId = 2, Name = "Mint", Language = "Peppermint", Climate = "Cool and Sweet", Population = 46001 },
          new Country { CountryId = 3, Name = "Agitated Badger", Language = "Chomp", Climate = "Harsh", Population = 450 },
          new Country { CountryId = 4, Name = "Western Democratic Coalition of Dragons", Language = "Dragonian", Climate = "Sweltering and Lava", Population = 3 },
          new Country { CountryId = 5, Name = "Cheese Island", Language = "Swiss", Climate = "Gooey", Population = 72 }, 
          new Country { CountryId = 6, Name = "Pants", Language = "Flubber", Climate = "Subtropical", Population = 7200 },
          new Country { CountryId = 7, Name = "Sporkonia", Language = "Sporkian", Climate = "Hot and Damp", Population = 840 },
          new Country { CountryId = 8, Name = "Coconut Land", Language = "Nut", Climate = "Desert Island Vibe", Population = 2 },
          new Country { CountryId = 9, Name = "Magnet Calendar", Language = "Sharpie", Climate = "Glossy", Population = 7600 }
        );
    }
  }
}