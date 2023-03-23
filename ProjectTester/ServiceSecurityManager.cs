using AppCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTester
{
    [TestClass]
    public class ServiceSecurityManager
    {

        [TestMethod]
        public void TestAsyncEncryption()
        {
            SecurityManager serverSecurity = new SecurityManager();
            string pk = serverSecurity.GenerateUnsafeAsyncKeysAndReturnPublicKey();

            SecurityManager clientSecurity = new SecurityManager();
            clientSecurity.SetUnsafeAsyncKey(pk);

            string synckey = "mySecretSyncKey";

            byte[] encryptedSyncKey = clientSecurity.EncryptUnsafeKey(synckey);

            string decryptedSyncKey = serverSecurity.DecryptUnsafeKey(encryptedSyncKey);

            Assert.AreEqual(decryptedSyncKey, synckey);
        }

        [TestMethod]
        public void TestSyncEncryption()
        {
            SecurityManager serverSecurity = new SecurityManager();
            string sync_key = serverSecurity.GenerateAndGetSyncKey();

            SecurityManager clientSecurity = new SecurityManager();
            clientSecurity.SetSyncKey(sync_key);

            string myData = "123456789";

            string myDataEncrypted = serverSecurity.Encrypt(myData);
            string myDataDecrypted = clientSecurity.Decrypt(myDataEncrypted);

            Assert.AreEqual(myData, myDataDecrypted);

            string myData2 = myData.Reverse().ToString()!;

            string myDataEncrypted2 = clientSecurity.Encrypt(myData2);
            string myDataDecrypted2 = serverSecurity.Decrypt(myDataEncrypted2);

            Assert.AreEqual(myData2, myDataDecrypted2);
        }

        [TestMethod]
        public void TestTransfertSyncKey()
        {
            SecurityManager serverSecurity = new SecurityManager();
            SecurityManager clientSecurity = new SecurityManager();

            string pk = clientSecurity.GenerateUnsafeAsyncKeysAndReturnPublicKey();
            serverSecurity.SetUnsafeAsyncKey(pk);

            string sync_key = serverSecurity.GenerateAndGetSyncKey();
            byte[] encryptedSyncKey = serverSecurity.EncryptUnsafeKey(sync_key);

            // SENDING : encryptedSyncKey

            string client_sync_key = clientSecurity.DecryptUnsafeKey(encryptedSyncKey);
            clientSecurity.SetSyncKey(sync_key);

            string reallyImportantData = "user:hello,password:world";
            string reallyImportantDataDecrypted = serverSecurity.Decrypt( clientSecurity.Encrypt(reallyImportantData) );

            string response = "Your are connected !";
            string responseDecrypted = clientSecurity.Decrypt(serverSecurity.Encrypt(response));

            Assert.AreEqual(sync_key, client_sync_key);
            Assert.AreEqual(reallyImportantData, reallyImportantDataDecrypted);
            Assert.AreEqual(response, responseDecrypted);
        }

        [TestMethod]
        public void TestSignature()
        {
            SecurityManager serverSecurity = new SecurityManager();
            SecurityManager clientSecurity = new SecurityManager();

            string public_client_key_sign = clientSecurity.GenerateSignatureKeyAndReturnPubKey();
            string public_server_key_sign = serverSecurity.GenerateSignatureKeyAndReturnPubKey();

            serverSecurity.SetPublicKeySignature(public_client_key_sign);
            clientSecurity.SetPublicKeySignature(public_server_key_sign);

            string myData = "123456789";
            string signedMyData = serverSecurity.SignData(myData);

            Assert.IsTrue(clientSecurity.CheckSign(myData, signedMyData));

            string myData2 = "987654321";
            string signedMyData2 = clientSecurity.SignData(myData2);

            Assert.IsTrue(serverSecurity.CheckSign(myData2, signedMyData2));
        }
    }
}
