namespace SchoolCoreAPI.Services
{
    using Microsoft.AspNetCore.DataProtection;
    using SchoolCoreAPI.IServices;
    using System;
    using System.Text;

    public class VapidKeyService : IVapidKeyService
    {
        private readonly IDataProtector _protector;
        private readonly IConfiguration _configuration;

        public VapidKeyService(IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
        {
            _protector = dataProtectionProvider.CreateProtector("VapidPrivateKeyProtector");
            _configuration = configuration;
        }

        public string EncryptPrivateKey(string privateKey)
        {
            if (string.IsNullOrEmpty(privateKey))
            {
                throw new ArgumentException("Private key is null or empty.");
            }

            return Convert.ToBase64String(_protector.Protect(Encoding.UTF8.GetBytes(privateKey)));
        }

        public string DecryptPrivateKey(string encryptedPrivateKey)
        {
            if (string.IsNullOrEmpty(encryptedPrivateKey))
            {
                throw new ArgumentException("Encrypted private key is null or empty.");
            }

            return Encoding.UTF8.GetString(_protector.Unprotect(Convert.FromBase64String(encryptedPrivateKey)));
        }

        public (string PublicKey, string DecryptedPrivateKey) GetKeysFromAppSettings()
        {
            string publicKey = _configuration["VapidKeys:PublicKey"];
            string encryptedPrivateKey = _configuration["VapidKeys:PrivateKey"];
            return (publicKey, encryptedPrivateKey);
        }
    }
    public class VapidKeyStore
    {
        private readonly IConfiguration _configuration;
        private readonly IVapidKeyService _vapidkeyService;

        public VapidKeyStore(IConfiguration configuration, IVapidKeyService vapidkeyService)
        {
            _configuration = configuration;
            _vapidkeyService = vapidkeyService;
        }

        public void StoreKeysInAppSettings(string publicKey, string privateKey)
        {
            // Implementation here
        }
    }

}
