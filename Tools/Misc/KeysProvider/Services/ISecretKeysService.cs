namespace KeysProvider.Services
{
    public interface ISecretKeysService
    {
        void SetSecrets();
        void RemoveSecrets();
    }
}