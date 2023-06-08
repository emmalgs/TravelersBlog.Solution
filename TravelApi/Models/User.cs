using Microsoft.AspNetCore.Identity;

namespace TravelApi.Models;

public class User : IdentityUser
{
  public List<Review> Reviews { get; set; }
}