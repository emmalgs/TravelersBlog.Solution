namespace TravelApi.Models;

public class User
{
  public int UserId { get; set; }
  public string UserName { get; set; }
  public List<Review> Reviews { get; set; }
}