using AppCore.Context;
using AppCore.Models;
using AppCore.Property;
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
            bool response;
            IActionResult result = await ProcessAndCheckToken<EpBuyBuilding>(message, (args) =>
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
                if (CheckAndUpdateUserRessources(args.ConsInfoId))
                {

                    var constr = new Construction()
                    {
                        ConsInfoId = args.ConsInfoId,
                        Type = (TypeConstruction)args.Type,
                        VillageId = town.VillageId,
                        Vie = args.Vie,
                    };

                    dbContext.Construction.Add(constr);
                    dbContext.SaveChanges();
                    return Ok();
                }else
                {
                    return BadRequest("L'utilisateur n'as pas les ressources suffisantes");
                }

            });

            return result;
        }

        private bool CheckAndUpdateUserRessources(int consInfoId)
        {
            var creationRessourceList = dbContext.CreationRessources.Where(x => x.ConsInfoId == consInfoId);
            var ressourcePossedeList = dbContext.RessourcePossedes.ToList();
            var resourcesToRemove = new List<Tuple<RessourcePossede, int>>();

            foreach (var creationRessource in creationRessourceList)
            {
                var ressourcePossedeItem = ressourcePossedeList.FirstOrDefault(x => x.RessourceId == creationRessource.RessourceId);

                if (ressourcePossedeItem != null && ressourcePossedeItem.Nombre < creationRessource.Nombre)
                {
                    return false;
                }

                resourcesToRemove.Add(Tuple.Create(ressourcePossedeItem, creationRessource.Nombre));
            }

            foreach (var tuple in resourcesToRemove)
            {
                var ressourcePossedeItem = tuple.Item1;
                var quantity = tuple.Item2;

                try
                {
                    RemoveRessources(ressourcePossedeItem, quantity);
                }
                catch (ArgumentException ex)
                {
                    // Gérer l'erreur de quantité insuffisante
                    Console.WriteLine($"Erreur : {ex.Message}");
                    return false;
                }
            }

            // Mettre à jour les ressources possédées en une seule opération après la boucle
            foreach (var ressourcePossedeItem in ressourcePossedeList)
            {
                dbContext.RessourcePossedes.Update(ressourcePossedeItem);
            }
            dbContext.SaveChanges();

            return true;
        }


        private void RemoveRessources(RessourcePossede ressourcePossede, int quantity)
        {
            if (ressourcePossede.Nombre >= quantity)
            {
                ressourcePossede.Nombre -= quantity;

            }
            else
            {
                throw new ArgumentException("La quantité de ressources à retirer dépasse la quantité disponible.");
            }
        }
    }
}
