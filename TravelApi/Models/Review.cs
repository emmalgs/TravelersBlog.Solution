namespace TravelApi.Models
{
  public class Review
  {
    public int ReviewId { get; set; }
    public string Text { get; set; }
    public int CountryId { get; set; }
    public int UserId { get; set; }
  }
}