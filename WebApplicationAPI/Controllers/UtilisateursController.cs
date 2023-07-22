using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppCore.Context;
using AppCore.Models;
using AppCore.Services.APIMessages;
using AppCore.Services;
using AppCore.Services.GeneralMessage.Args;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class UtilisateursController : FreshTechController
    {
        public UtilisateursController(FTDbContext context) : base(context)
        {
        }

        [HttpPost(APIRoute.CREATE_USER)]
        public async Task<IActionResult> Create(FTMessageClient message)
        {
            return await ProcessResponse<EPCreateUser>(message, (args) =>
            {
                if(string.IsNullOrWhiteSpace(args.Pseudo) || 
                string.IsNullOrWhiteSpace(args.Mail) ||
                string.IsNullOrWhiteSpace(args.MotDePasse) ||
                string.IsNullOrWhiteSpace(args.Sel))
                {
                    return BadRequest(APIError.BAD_ARGS);
                }

                if(dbContext.Utilisateurs.Any(x => x.Pseudo == args.Pseudo || x.Mail == args.Mail))
                {
                    return BadRequest(APIError.BAD_ARGS);
                }

                Utilisateur u = new Utilisateur();
                u.UtilisateurId = Guid.NewGuid();
                u.Pseudo = args.Pseudo;
                u.Mail = args.Mail;
                u.MotDePasse = args.MotDePasse;
                u.Sel = args.Sel;
                u.PoidKg = args.PoidKg;
                u.TailleCm = args.TailleCm;
                dbContext.Utilisateurs.Add(u);
                dbContext.SaveChanges();

                return Json(Message(message, u.UtilisateurId));
            });
        }

        [HttpPost(APIRoute.UPDATE_USER)]
        public async Task<IActionResult> Update(FTMessageClient message)
        {
            return await ProcessAndCheckToken<EPUpdateUser>(message, (args) =>
            {
                Utilisateur? u = GetUserByUserGuid(message.UserGuid);

                if(u == null) { return BadRequest(APIError.USER_ID_NOT_EXIST); }

                bool edit = false;

                if (args.MotDePasse != null && args.Sel != null) 
                {
                    u.MotDePasse = args.MotDePasse; 
                    u.Sel = args.Sel;
                    edit = true;
                }

                if (args.Mail != null)
                {
                    u.Mail = args.Mail;
                    edit = true;
                }

                if (args.TailleCm != null)
                {
                    u.TailleCm = args.TailleCm;
                    edit = true;
                }

                if(args.PoidKg != null)
                {
                    u.PoidKg = args.PoidKg;
                    edit = true;
                }

                if(args.Pseudo != null)
                {
                    u.Pseudo = args.Pseudo;
                    edit = true;
                }

                if (edit)
                {
                    dbContext.Update(u);
                    dbContext.SaveChanges();
                    return Ok();
                }
                else {
                    return BadRequest(APIError.NOTHING_TO_UPDATE);
                }

            });
        }

        [HttpPost(APIRoute.GET_USER_BY_TOKEN)]
        public async Task<IActionResult> GetUserByToken(FTMessageClient message)
        {
            return await ProcessAndCheckToken<EPGetUserByToken>(message, (args) =>
            {
                Guid? UtilisateurId = Program.serverManager.GetUserGuidByUserId(message.UserGuid);
                if(UtilisateurId != null)
                {
                    Utilisateur? user = dbContext.Utilisateurs.FirstOrDefault( x => x.UtilisateurId == UtilisateurId);
                    if(user != null)
                    {
                        return Json(Message(message, user));
                    }
                }

                return BadRequest(APIError.CANCELED_REQUEST);
            });
        }
    }
}
