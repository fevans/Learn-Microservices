using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Identity.Service.Models;

[CollectionName("users")]
public class ApplicationUser : MongoIdentityUser<Guid>
{
    public decimal Gil { get; set; }
    public  decimal GilSpent { get; set; }
}