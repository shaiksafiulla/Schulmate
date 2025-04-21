namespace SchoolCoreAPI.IServices
{
    public interface IVapidKeyService
    {
        string EncryptPrivateKey(string privateKey);
        string DecryptPrivateKey(string encryptedPrivateKey);
        (string PublicKey, string DecryptedPrivateKey) GetKeysFromAppSettings();
    }
}
