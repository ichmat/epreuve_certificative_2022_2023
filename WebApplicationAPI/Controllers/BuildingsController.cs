using AppCore.Context;
using AppCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class BuildingsController : FreshTechController
    {
        public BuildingsController(FTDbContext dbContext) : base(dbContext)
        {
        }
    }
}
