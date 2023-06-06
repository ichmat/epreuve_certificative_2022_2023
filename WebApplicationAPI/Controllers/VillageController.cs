using AppCore.Context;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services;
using Microsoft.AspNetCore.Mvc;
using AppCore.Models;
using AppCore.Services.GeneralMessage.Response;

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
        public async Task<IActionResult> GetEntireVillageOfUser(FTMessageClient message)
        {
            return await ProcessAndCheckToken<EPGetEntireVillage>(message, (args) =>
            {
                Guid? userId = GetUtilisateurIdByUserGuid(message.UserGuid);
                if(userId == null)
                {
                    return BadRequest(APIError.BAD_USER_TOKEN);
                }
                Village? town = dbContext.Villages.FirstOrDefault(x => x.UtilisateurId == userId);

                if(town == null)
                {
                    return BadRequest(APIError.VILLAGE_NOT_SET);
                }

                return Json(
                    Message(message.UserGuid, new ResponseGetEntireVillage(
                        town,
                        dbContext.ConstructionDefs.Where(x => x.VillageId == town.VillageId).ToArray(),
                        dbContext.ConstructionProds.Where(x => x.VillageId == town.VillageId).ToArray(),
                        dbContext.ConstructionAutres.Where(x => x.VillageId == town.VillageId).ToArray(),
                        dbContext.Attaques.Where(x => x.VillageId == town.VillageId).ToArray(),
                        dbContext.Coordonnees.Where(x => x.VillageId == town.VillageId).ToArray(),
                        dbContext.RessourcePossedes.Where(x => x.VillageId == town.VillageId).ToArray(),
                        dbContext.ObjetsPossedes.Where(x => x.VillageId == town.VillageId).ToArray()
                        ))
                    );
            });
        }
    }
}
