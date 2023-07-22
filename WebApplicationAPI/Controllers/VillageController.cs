using AppCore.Context;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services;
using Microsoft.AspNetCore.Mvc;
using AppCore.Models;
using AppCore.Services.GeneralMessage.Response;
using AppCore.Property;
using System;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class VillageController : FreshTechController
    {
        public VillageController(FTDbContext dbContext) : base(dbContext)
        {

        }

        [HttpPost(APIRoute.CREATE_USER_VILLAGE)]
        public async Task<IActionResult> CreateUserVillage(FTMessageClient message)
        {
            return await ProcessAndCheckToken<EPCreateUserVillage>(message, (args) =>
            {
                Guid? userId = GetUtilisateurIdByUserGuid(message.UserGuid);
                if (userId == null)
                {
                    return BadRequest(APIError.BAD_USER_TOKEN);
                }

                Village? town = dbContext.Villages.FirstOrDefault(x => x.UtilisateurId == userId);

                if (town != null)
                {
                    return BadRequest(APIError.VILLAGE_ALREADY_SET);
                }

                // création du village
                Village village = new Village();
                village.UtilisateurId = userId.Value;
                dbContext.Villages.Add(village);

                //Add the event construction and define the finish time 
                Attaque attaque = new Attaque();
                attaque.DateApparition = DateTime.Now + new TimeSpan(2, 0, 0);
                attaque.Puissance = 300;
                attaque.Village = village;

                dbContext.Attaques.Add(attaque);
                dbContext.SaveChanges();
                return Ok();
            });
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
                var response = new ResponseGetEntireVillage(
                        town,
                        dbContext.ConstructionDefs.Where(x => x.VillageId == town.VillageId).ToArray(),
                        dbContext.ConstructionProds.Where(x => x.VillageId == town.VillageId).ToArray(),
                        dbContext.ConstructionAutres.Where(x => x.VillageId == town.VillageId).ToArray(),
                        dbContext.Attaques.Where(x => x.VillageId == town.VillageId).ToArray(),
                        dbContext.Coordonnees.Where(x => x.VillageId == town.VillageId).ToArray(),
                        dbContext.RessourcePossedes.Where(x => x.VillageId == town.VillageId).ToArray(),
                        dbContext.ObjetsPossedes.Where(x => x.VillageId == town.VillageId).ToArray()
                    );
                return Json(Message(message.UserGuid, response));
            });
        }

        [HttpPost(APIRoute.GET_NECESSARY_DATA_VILLAGE)]
        public async Task<IActionResult> GetNecessaryDataVillage(FTMessageClient message)
        {
            return await ProcessAndCheckToken<EPGetNecessaryDataVillage>(message, (args) =>
            {
                return Json(
                    Message(message.UserGuid, new ResponseGetNecessaryDataVillage(
                        dbContext.ConstructionInfos.ToArray(),
                        dbContext.Ressources.ToArray(),
                        dbContext.Objets.ToArray(),
                        dbContext.CreationRessources.ToArray(),
                        dbContext.AmeliorationRessources.ToArray(),
                        dbContext.ReparationRessources.ToArray(),
                        dbContext.CreationObjets.ToArray(),
                        dbContext.AmeliorationObjets.ToArray()
                        )
                    ));
            });
        }
    }
}
