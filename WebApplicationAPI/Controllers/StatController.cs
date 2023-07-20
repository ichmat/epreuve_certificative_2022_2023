using AppCore.Context;
using AppCore.Models;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class StatController : FreshTechController
    {
        public StatController(FTDbContext dbContext) : base(dbContext)
        {
        }

        [HttpPost(APIRoute.SAVE_STAT)]
        public async Task<IActionResult> SaveStat(FTMessageClient message)
        {
            return await ProcessAndCheckToken<EPSaveStat>(message, (args) =>
            {
                Guid? utilisateurId = GetUtilisateurIdByUserGuid(message.UserGuid);
                if(utilisateurId == null)
                {
                    return BadRequest(APIError.USER_ID_NOT_EXIST);
                }

                if(args.Stat.UtilisateurId != utilisateurId)
                {
                    return Unauthorized();
                }

                Stat? stat = dbContext.Stats.FirstOrDefault(x => x.UtilisateurId == utilisateurId);
                if(stat != null)
                {
                    dbContext.Stats.Remove(stat);
                }

                dbContext.Stats.Add(args.Stat);
                dbContext.SaveChanges();

                return Ok();
            });
        }

        [HttpPost(APIRoute.GET_STAT_BY_USER_ID)]
        public async Task<IActionResult> GetStatByUserId(FTMessageClient message)
        {
            return await ProcessAndCheckToken<EPGetStatByUserId>(message, (args) =>
            {
                Guid? userId = GetUtilisateurIdByUserGuid(message.UserGuid);
                if (userId == null)
                {
                    return BadRequest(APIError.BAD_USER_TOKEN);
                }

                var stat = dbContext.Stats.FirstOrDefault(x => x.UtilisateurId == userId.Value);
                if(stat != null)
                {
                    return Json(Message(message.UserGuid, stat));
                }
                return NotFound();
            });
        }

        [HttpPost(APIRoute.GENERATE_ACTIVITY_ENGINE)]
        public async Task<IActionResult> GenerateActivityEngine(FTMessageClient message)
        {
            return await ProcessAndCheckToken<EPGenerateActivityEngine>(message, (args) =>
            {
                Guid? userId = GetUtilisateurIdByUserGuid(message.UserGuid);

                if (userId == null)
                {
                    return BadRequest(APIError.BAD_USER_TOKEN);
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
                catch(NoNullAllowedException)
                {
                    return BadRequest(APIError.NON_COMPLETE_USER_STAT);
                }
                
                return Json(Message(message, engine));
            });
        }
    }
}
