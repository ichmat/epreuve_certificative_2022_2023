using AppCore.Context;
using AppCore.Models;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class CoursesController : FreshTechController
    {
        public const int MAX_LENGHT_COURSES = 50;

        public CoursesController(FTDbContext dbContext) : base(dbContext)
        {
            
        }

        [HttpPost(APIRoute.GET_COURSES_ORDERED_BY_DATE_DESC)]
        public async Task<IActionResult> GetCourses(FTMessageClient message)
        {
            return await ProcessAndCheckToken<EPGetCourses>(message, (args) =>
            {
                Guid? userId = GetUtilisateurIdByUserGuid(message.UserGuid);

                if (userId == null)
                {
                    return BadRequest(APIError.BAD_USER_TOKEN);
                }

                if(args.EndIndex - args.StartIndex > MAX_LENGHT_COURSES)
                {
                    return BadRequest(APIError.TOO_MUCH_DATA);
                }

                return Json(Message(message.UserGuid,
                    dbContext.Courses
                    .Where(x => x.UtilisateurId == userId) // récupère uniquement les Courses de l'utilisateur
                    .OrderByDescending(x => x.DateDebut) // les tries par date 
                    .Skip(args.StartIndex) // index de début
                    .Take(args.EndIndex - args.StartIndex) // index de fin
                    .ToArray()
                ));
            });
        }

        [HttpPost(APIRoute.PUBLISH_COURSE)]
        public async Task<IActionResult> PublishCourses(FTMessageClient message)
        {
            return await ProcessAndCheckToken<EPPublishCourse>(message, (args) =>
            {
                // appliquer toute les vérifications
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

                Stat? stat = dbContext.Stats.FirstOrDefault(x => x.UtilisateurId == userId);
                if (stat == null)
                {
                    return BadRequest(APIError.NO_STAT_SET);
                }

                ActivityEngine engine;
                try
                {
                    engine = new ActivityEngine(stat);
                }
                catch (NoNullAllowedException)
                {
                    return BadRequest(APIError.NON_COMPLETE_USER_STAT);
                }

                // création de l'instance de la course
                Courses courses = new Courses()
                {
                    VitesseMoyenKmH = (float)Math.Round(args.MeanSpeedKmH, 4),
                    DistanceKm = (float)Math.Round(args.DistanceKm, 4),
                    TempsSec = (long)Math.Round(args.TotalActivitySec),
                    TempsSecPause = (long)Math.Round(args.TotalPauseSec),
                    NiveauDifficulte = args.Difficulty,
                    DateDebut = args.DateStartActivity,
                    UtilisateurId = userId.Value
                };

                // génération des gains par le système 
                ResultAward award = engine.GenerateAward(courses);

                // récupération des id de ressources BOIS et FERRAILLE
                int idWood = NecessaryData.GetRessource(RESSOURCE.BOIS).RessourceId;
                int idScrapMetal = NecessaryData.GetRessource(RESSOURCE.FERRAILLE).RessourceId;

                // fonction d'ajout de ressource
                Action<int, int> applyAwardRessource = (idRes, nbReward) =>
                {
                    RessourcePossede? ressource = dbContext.RessourcePossedes.FirstOrDefault(x => x.VillageId == town.VillageId && x.RessourceId == idRes);
                    if (ressource != null)
                    {
                        ressource.Nombre += nbReward;
                        dbContext.RessourcePossedes.Update(ressource);
                    }
                    else
                    {
                        ressource = new RessourcePossede()
                        {
                            VillageId = town.VillageId,
                            RessourceId = idRes,
                            Nombre = nbReward
                        };
                        dbContext.RessourcePossedes.Add(ressource);
                    }
                };

                // application des gains des ressources
                applyAwardRessource.Invoke(idWood, award.RealAwardWood);
                applyAwardRessource.Invoke(idScrapMetal, award.RealAwardScrapMetal);

                // application des gains des objets
                Objet obj;
                int numberGain;
                foreach(var objAward in award.RealAwardObjets)
                {
                    obj = NecessaryData.GetObjetById(objAward.Key);
                    numberGain = objAward.Value;
                    ObjetsPossede? objets = dbContext.ObjetsPossedes.FirstOrDefault(x => x.VillageId == town.VillageId && x.ObjetId == obj.ObjetId);

                    if (objets != null)
                    {
                        objets.Nombre += numberGain;
                        dbContext.ObjetsPossedes.Update(objets);
                    }
                    else
                    {
                        objets = new ObjetsPossede()
                        {
                            VillageId = town.VillageId,
                            ObjetId = obj.ObjetId,
                            Nombre = numberGain
                        };
                        dbContext.ObjetsPossedes.Add(objets);
                    }
                }

                // ajout de la course
                dbContext.Courses.Add(courses);

                dbContext.SaveChanges();

                return Json(Message(message.UserGuid, award));
            });
        }

    }
}
