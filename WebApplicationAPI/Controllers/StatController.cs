using AppCore.Context;
using AppCore.Models;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                Guid? utilisateurId = GetUutilisateurIdByUserGuid(message.UserGuid);
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
                return Ok(Json(dbContext.Stats.FirstOrDefault(x => x.UtilisateurId == args.UtilisateurId)));
            });
        }
    }
}
