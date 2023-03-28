using AppCore.Context;
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
    public class ConnexionController : Controller
    {
        private readonly FTDbContext dbContext;

        public ConnexionController(FTDbContext dbContext)
        {
            this.dbContext = dbContext;
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
                string? err = Program.serverManager.UserExistAndCheckState(message.UserGuid);
                if (err == null)
                {
                    FTMessageServer response = Program.serverManager.
                        SetPublicSignKeyAndReturnServerPkSignKey(message.UserGuid, message.Signature);
                    res = Json(response);
                }
                else
                {
                    res = BadRequest(err);
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
            await FTMServerManager.WaitLock();

            IActionResult res;
            try
            {
                string? err = Program.serverManager.UserExistAndCheckState(message.UserGuid);
                if (err == null)
                {
                    res = Ok();
                }
                else
                {
                    res = BadRequest(err);
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

        [HttpPost(APIRoute.ATTEMPT_CONNECTION)]
        public async Task<IActionResult> AttemptConnection(FTMessageClient message)
        {
            await FTMServerManager.WaitLock();

            IActionResult res;
            try
            {
                string? err = Program.serverManager.UserExistAndCheckState(message.UserGuid);
                if (err == null)
                {
                    Credentials? cred = message.SecureDecrypt<Credentials>(Program.serverManager[message.UserGuid]);
                    if(cred != null)
                    {
                        //dbContext.Utilisateurs.FirstOrDefaultAsync(x => x.);
                        res = Ok();
                    }
                    else
                        res = BadRequest(APIError.BAD_FORMAT_DATA);
                }
                else
                    res = BadRequest(err);
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
    }
}
