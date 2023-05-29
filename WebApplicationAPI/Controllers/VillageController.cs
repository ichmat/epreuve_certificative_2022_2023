using AppCore.Context;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class VillageController : FreshTechController
    {
        public VillageController(FTDbContext dbContext) : base(dbContext)
        {

        }

        [HttpPost(APIRoute.GET_ENTIRE_VILLAGE)]
        public async Task<IActionResult> GetStatByUserId(FTMessageClient message)
        {
            return await ProcessAndCheckToken<EPGetEntireVillage>(message, (args) =>
            {
                
            });
        }
    }
}
