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

                dbContext.SaveChanges();

                return Json(Message(message, u.UtilisateurId));
            });
        }
        
    }
}
