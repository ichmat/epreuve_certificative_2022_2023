using AppCore.Context;
using AppCore.Models;
using AppCore.Services;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class ConnexionController : FreshTechController
    {
        public ConnexionController(FTDbContext dbContext) : base(dbContext)
        {
        }

        [HttpPost(APIRoute.ESTABLISH_CONNECTION)]
        public async Task<IActionResult> EstablishConnection(FTMessageClient message)
        {
            await FTMServerManager.WaitLock();
            IActionResult res;
            try
            {
                Program.serverManager.AddOrClearUser(message.UserGuid);
                FTMessageServer response = Program.serverManager
                    .SetAsyncKeyAndReturnSyncKey(message.UserGuid, message.Message);
                res = Json(response);
            }
            catch(Exception ex) 
            {
                res = BadRequest(APIError.CANCELED_REQUEST);
            }
            finally
            {
                FTMServerManager.Release();
            }

            return res;
        }

        [HttpPost(APIRoute.SIGN_KEY)]
        public async Task<IActionResult> SetPublicSignKey(FTMessageClient message)
        {
            await FTMServerManager.WaitLock();
            IActionResult res;
            try
            {
                bool exist = Program.serverManager.UserExist(message.UserGuid);
                if (exist)
                {
                    FTMessageServer response = Program.serverManager.
                        SetPublicSignKeyAndReturnServerPkSignKey(message.UserGuid, message.Message);
                    res = Json(response);
                }
                else
                {
                    res = BadRequest(APIError.USER_ID_NOT_EXIST);
                }
            }
            catch (Exception ex)
            {
                res = BadRequest(APIError.CANCELED_REQUEST);
            }
            finally
            {
                FTMServerManager.Release();
            }

            return res;

        }

        [HttpPost(APIRoute.RESET_TIME_OUT)]
        public async Task<IActionResult> ResetTimeOut(FTMessageClient message)
        {
            return await ProcessResponse(message, Ok);
        }

        [HttpPost(APIRoute.ATTEMPT_CONNECTION)]
        public async Task<IActionResult> AttemptConnection(FTMessageClient message)
        {
            return await ProcessResponseWithoutCheckToken<Credentials>(message, (cred) =>
            {
                Utilisateur? user = dbContext.Utilisateurs.FirstOrDefault(x => x.Mail == cred.Email || x.Pseudo == cred.User);
                if (user != null && Password.VerifyPassword(cred.Password, user.MotDePasse, user.Sel))
                {
                    return Json(Program.serverManager.GenerateToken(message.UserGuid, user));
                }
                else
                    return BadRequest(APIError.BAD_CREDENTIALS);
            });
        }

        private async Task<IActionResult> ProcessResponseWithoutCheckToken<T>(FTMessageClient message, Func<T, IActionResult> process) where T : class
        {
            await FTMServerManager.WaitLock();

            IActionResult res;

            try
            {
                bool exist = Program.serverManager.UserExist(message.UserGuid);
                if (exist)
                {
                    T? obj = message.SecureDecrypt<T>(Program.serverManager[message.UserGuid]);
                    if (obj != null)
                    {
                        res = process.Invoke(obj);
                    }
                    else
                        res = BadRequest(APIError.BAD_FORMAT_DATA);
                }
                else
                {
                    res = BadRequest(APIError.USER_ID_NOT_EXIST);
                }
            }
            catch (Exception ex)
            {
                res = BadRequest(APIError.CANCELED_REQUEST);
                LogError(ex.ToString());
            }
            finally
            {
                FTMServerManager.Release();
            }

            return res;
        }
    }
}
