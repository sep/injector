namespace Injector
{
    public interface IInjection
    {
        void InjectAppSetting(string settingKey, string value);
        void InjectWcfEndpoint(string endpointName, string endpoint);
        void InjectConnectionString(string connectionStringName, string connectionString);
    }
}