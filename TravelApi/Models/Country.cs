namespace TravelApi.Models
{
  public class Country
  {
    public int CountryId { get; set; }
    public string Name { get; set; }
    public string Language { get; set; }
    public int Population { get; set; }
    public string Climate { get; set; }
    public List<Review> Reviews { get; set; }
  }
}