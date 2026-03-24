using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Identity.Service.Models;

[CollectionName("roles")]
public class ApplicationRole : MongoIdentityRole<Guid>
{

}