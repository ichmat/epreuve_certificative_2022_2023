using AppCore.Context;
using AppCore.Models;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage.Args;
using AppCore.Services;
using Microsoft.AspNetCore.Mvc;

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
                    .Take(args.EndIndex) // index de fin
                    .ToArray()
                ));
            });
        }
    }
}
