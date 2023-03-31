using AppCore.Context;
using AppCore.Services;
using AppCore.Services.APIMessages;
using AppCore.Services.GeneralMessage;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationAPI.Controllers
{
    [Produces("application/json")]
    public abstract class FreshTechController : Controller
    {
        protected readonly FTDbContext dbContext;

        protected FreshTechController(FTDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        protected async Task<IActionResult> ProcessResponse<T>(FTMessageClient message, Func<T, IActionResult> process) where T : class
        {
            await FTMServerManager.WaitLock();

            IActionResult res;

            try
            {
                string? err = Program.serverManager.UserExistAndCheckState(message.UserGuid);
                if (err == null)
                {
                    T? obj = message.SecureDecrypt<T>(Program.serverManager[message.UserGuid]);
                    if(obj != null)
                    {
                        res = process.Invoke(obj);
                    }
                    else
                        res = BadRequest(APIError.BAD_FORMAT_DATA);
                }
                else
                {
                    res = BadRequest(err);
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

        protected async Task<IActionResult> ProcessResponse(FTMessageClient message, Func<IActionResult> process)
        {
            await FTMServerManager.WaitLock();

            IActionResult res;

            try
            {
                string? err = Program.serverManager.UserExistAndCheckState(message.UserGuid);
                if (err == null)
                {
                    res = process.Invoke();
                }
                else
                {
                    res = BadRequest(err);
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

        protected FTMessageServer Message(string UserGuid, object data)
        {
            return FTMessageServer.GenerateSecure(
                Program.serverManager[UserGuid],
                data);
        }

        protected FTMessageServer Message(FTMessageClient message, object data)
        {
            return FTMessageServer.GenerateSecure(
                Program.serverManager[message.UserGuid],
                data);
        }

        protected void LogError(string err)
        {
            Console.Error.WriteLine(err);
        }

    }
}
