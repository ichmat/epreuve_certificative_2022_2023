using AppCore.Context;
using AppCore.Models;
using AppCore.Services;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage.Args;
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
        [HttpPost(APIRoute.CREATE_BUILDING)]
        public async Task<IActionResult> CreateBuilding(FTMessageClient message)
        {
            return await ProcessAndCheckToken<EpBuyBuilding>(message, (args) =>
            {
                Guid? userId = GetUtilisateurIdByUserGuid(message.UserGuid);
                if (userId == null)
                {
                    return BadRequest(APIError.BAD_USER_TOKEN);
                }

                Village? town = dbContext.Villages.FirstOrDefault(x => x.UtilisateurId == userId);

                if (town == null)
                {
                    return BadRequest(APIError.VILLAGE_NOT_SET);
                }

                var constr = new Construction();
                dbContext.Co.Add(constr);
                // création du village
                
                //village.UtilisateurId = userId.Value;
                //dbContext.Villages.Add(village);
                dbContext.SaveChanges();
                return Ok();
            });
        }
    } 
}
