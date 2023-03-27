using AppCore.Context;
using AppCore.Services;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
                res = BadRequest(ex.Message);
            }
            finally
            {
                FTMServerManager.Release();
            }

            return res;
        }
    }
}
