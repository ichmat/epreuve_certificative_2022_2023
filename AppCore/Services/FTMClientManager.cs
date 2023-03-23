using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class FTMClientManager
    {
        private readonly string _id;
        private readonly SecurityManager _securityManager;
        private string? _token;

        public FTMClientManager() {
            _id = Guid.NewGuid().ToString();
            _securityManager = new SecurityManager();
        }

        
    }
}
