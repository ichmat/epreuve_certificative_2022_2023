using AppCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTester
{
    [TestClass]
    public class TestPassword
    {
        [TestMethod]
        public void GenerateSaltAndVerifyPwd()
        {
            string clearPassword = "my clear password";
            string hash = Password.HashPasword(clearPassword, out string salt);

            string badPassword = "this is bad";

            Assert.IsFalse(Password.VerifyPassword(badPassword, hash, salt));

            string rightPassword = "my clear password";

            Assert.IsTrue(Password.VerifyPassword(rightPassword, hash, salt));
        }
    }
}
