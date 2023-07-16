using AppCore.Context;
using AppCore.Models;
using AppCore.Property;
using AppCore.Services;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services.GeneralMessage.Response;
using AppCore.Services.NecessaryDataClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            return await ProcessAndCheckToken<EPBuyBuilding>(message, (args) =>
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
                // vérifie les ressources et objets 
                if (!CheckAndUpdateUserObjetAndRessources(args.ConsInfoId, out List<RessourcePossede> ressourcePossedesUpdated, out List<ObjetsPossede> objetsPossedesUpdated))
                {
                    return BadRequest(APIError.NOT_ENOUGHT_RESSOURCE);
                }

                // créer temporairement le bâtiment
                if (!CheckAndMakeBuilding(args.ConsInfoId, town.VillageId, out Construction? constructionCreated) || constructionCreated == null)
                {
                    return BadRequest(APIError.ERROR_SCHEMA_NOT_FOUND_OR_INCOMPLET);
                }

                // enregistre les modifications 
                if (!ApplyModification(in constructionCreated, in ressourcePossedesUpdated, in objetsPossedesUpdated))
                {
                    return Problem("Construction type not supported or error saving", null, 500);
                }

                // envoie les données modifiés
                return Json(Message(message.UserGuid, new ResponseBuyBuilding(ressourcePossedesUpdated, objetsPossedesUpdated, constructionCreated)));
            });
        }

        [Obsolete("oublie de vérifier si l'utilisateur possédait les objets nécessaire et on souhaite renvoyer les données modifier pour que l'utilisateur les reçoivent (cela évite que l'utilisateur redemande les derniers info modifier)")]
        private bool CheckAndUpdateUserRessources(int consInfoId)
        {
            var creationRessourceList = dbContext.CreationRessources.Where(x => x.ConsInfoId == consInfoId);
            var ressourcePossedeList = dbContext.RessourcePossedes.ToList();
            var resourcesToRemove = new List<Tuple<RessourcePossede, int>>();

            foreach (var creationRessource in creationRessourceList)
            {
                var ressourcePossedeItem = ressourcePossedeList.FirstOrDefault(x => x.RessourceId == creationRessource.RessourceId);

                if (ressourcePossedeItem == null || ressourcePossedeItem.Nombre < creationRessource.Nombre)
                {
                    return false;
                }

                resourcesToRemove.Add(Tuple.Create(ressourcePossedeItem!, creationRessource.Nombre));
            }

            // ATTENTION : foreach vérifie si les données ont été modifier, or vue que tu les modifie ici
            // il va renvoyer une erreur
            for(int i = 0; i < resourcesToRemove.Count; ++i)
            {
                var tuple = resourcesToRemove[i];
                var ressourcePossedeItem = tuple.Item1;
                var quantity = tuple.Item2;

                try
                {
                    // ATTENTION : si tu ne met pas le mot clé 'ref',
                    // l'objet que tu donnes ne sera pas modifier en sortie de fonction
                    RemoveRessources(ref ressourcePossedeItem,in quantity);
                }
                catch (ArgumentException ex)
                {
                    // Gérer l'erreur de quantité insuffisante
                    Console.WriteLine($"Erreur : {ex.Message}");
                    return false;
                }
            }

            // Mettre à jour les ressources possédées en une seule opération après la boucle
            dbContext.RessourcePossedes.UpdateRange(ressourcePossedeList);
            
            // Ne SOURTOUT PAS mettre à jour, il faut que cela se fasse UNE fois seulement
            //dbContext.SaveChanges();

            return true;
        }

        private bool CheckAndUpdateUserObjetAndRessources(int consInfoId, out List<RessourcePossede> ressourcePossedesUpdated, out List<ObjetsPossede> objetsPossedesUpdated)
        {
            if (FTDbContext.NecessaryData != null)
            {
                ressourcePossedesUpdated = new List<RessourcePossede>();
                objetsPossedesUpdated = new List<ObjetsPossede>();

                // récupération du schéma nécessaire
                ConstructionInfoSchema? schema = FTDbContext.NecessaryData.TryGetConstructionInfoSchemaByConsInfoId(consInfoId);
                if (schema == null) return false; // schéma non trouvé

                // vérification des ressources
                // key => id de la ressource
                // value => quantité nécessaire
                foreach(KeyValuePair<int, int> ressourcesNecessaire in schema.CreationsRessources)
                {
                    // vérifie si l'utilisateur possède les ressources adéquat
                    RessourcePossede? ressourcePossede = 
                        dbContext.RessourcePossedes.FirstOrDefault(x => x.RessourceId == ressourcesNecessaire.Key);

                    if(ressourcePossede != null && ressourcePossede.Nombre >= ressourcesNecessaire.Value)
                    {
                        // appliquer la modification temporairement (pas encore en BDD)
                        ressourcePossede.Nombre -= ressourcesNecessaire.Value;
                        ressourcePossedesUpdated.Add(ressourcePossede);
                    }
                    else
                    {
                        return false;
                    }
                }

                // vérification des objets
                // key => id de l'objet
                // value => quantité nécessaire
                foreach (KeyValuePair<int, int> objetsNecessaire in schema.CreationsObjet)
                {
                    // vérifie si l'utilisateur possède les objets adéquat
                    ObjetsPossede? objetsPossede =
                        dbContext.ObjetsPossedes.FirstOrDefault(x => x.ObjetId == objetsNecessaire.Key);

                    if(objetsPossede != null && objetsPossede.Nombre >= objetsNecessaire.Value)
                    {
                        // appliquer la modification temporairement (pas encore en BDD)
                        objetsPossede.Nombre -= objetsNecessaire.Value;
                        objetsPossedesUpdated.Add(objetsPossede);
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                throw new NullReferenceException("'FTDbContext.NecessaryData' est null, problème d'initialisation. 'FTDbContext.TriggerConfigureFinish()' n'a pas été appelé ou à été modifier");
            }
        }

        private bool ApplyModification(in Construction constructionCreated, in List<RessourcePossede> ressourcePossedesUpdated, in List<ObjetsPossede> objetsPossedesUpdated)
        {
            switch(constructionCreated)
            {
                case ConstructionDef constructionDef:
                    dbContext.ConstructionDefs.Add(constructionDef);
                    break;
                case ConstructionAutre constructionAutre:
                    dbContext.ConstructionAutres.Add(constructionAutre);
                    break;
                case ConstructionProd constructionProd:
                    dbContext.ConstructionProds.Add(constructionProd);
                    break;
                default:
                    return false;
            }
            dbContext.RessourcePossedes.UpdateRange(ressourcePossedesUpdated);
            dbContext.ObjetsPossedes.UpdateRange(objetsPossedesUpdated);
            dbContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// Créer une construction avec l'id du schéma donnée
        /// </summary>
        /// <param name="consInfoId">id de construction à créer</param>
        /// <param name="townId">l'id du village où le bâtiment sera créé</param>
        /// <param name="constructionCreated">la construction créer</param>
        /// <returns>
        /// <b>True</b> => bâtiment créer et insérer en base (pas encore sauvegardé) <br></br>
        /// <b>False</b> => bâtiment non créer : id inexistant ou schéma ne possédant pas un <see cref="ConstructionInfoSchema.ConsTypeSchema"/> correct
        /// </returns>
        /// <exception cref="NullReferenceException">'FTDbContext.NecessaryData' est null</exception>
        /// <exception cref="Exception">Problème de paramétrage des schémas dans 'NecessaryData'</exception>
        private bool CheckAndMakeBuilding(in int consInfoId, in int townId, out Construction? constructionCreated)
        {
            if(FTDbContext.NecessaryData != null)
            {
                // récupération du schéma nécessaire
                ConstructionInfoSchema? schema = FTDbContext.NecessaryData.TryGetConstructionInfoSchemaByConsInfoId(consInfoId);
                constructionCreated = null;
                if (schema == null) return false; // schéma non trouvé

                try
                {
                    // construction du bâtiment
                    switch (schema.ConsTypeSchema)
                    {
                        case TypeSchema.Prod:
                            ConstructionProd constructionProd = new ConstructionProd()
                            {
                                ConsInfoId = consInfoId,
                                Type = schema.Type,
                                VillageId = townId,
                                Vie = schema.VieMax,
                                Niveau = 1,
                                Production = schema.Production!.Value,
                                MultParNiveau = schema.MultParNiveau!.Value,
                                RessourceId = schema.RessourceProduit!.RessourceId
                            };
                            constructionCreated = constructionProd;
                            return true;
                        case TypeSchema.Def:
                            ConstructionDef constructionDef = new ConstructionDef()
                            {
                                ConsInfoId = consInfoId,
                                Type = schema.Type,
                                VillageId = townId,
                                Vie = schema.VieMax,
                                Niveau = 1,
                                Puissance = schema.Puissance!.Value,
                                MultParNiveau = schema.MultParNiveau!.Value,
                            };
                            constructionCreated = constructionDef;
                            return true;
                        case TypeSchema.Other:
                            ConstructionAutre constructionAutre = new ConstructionAutre()
                            {
                                ConsInfoId = consInfoId,
                                Type = schema.Type,
                                VillageId = townId,
                                Vie = schema.VieMax,
                                Niveau = 1
                            };
                            constructionCreated = constructionAutre;
                            return true;
                        default:
                            return false;
                    }
                }
                catch
                {
                    throw new Exception("Problème de paramétrage des schémas dans 'NecessaryData'.");
                }
            }
            else
            {
                throw new NullReferenceException("'FTDbContext.NecessaryData' est null, problème d'initialisation. 'FTDbContext.TriggerConfigureFinish()' n'a pas été appelé ou à été modifier") ;
            }
        }


        [Obsolete("pas utilisée ici")]
        private void RemoveRessources(ref RessourcePossede ressourcePossede,in int quantity)
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
