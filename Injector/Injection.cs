using System.IO;
using System.Xml;

namespace Injector
{
    public class Injection
    {
        private readonly string _configFile;
        private readonly TextWriter _out;
        private readonly TextWriter _error;

        public Injection(string configFile, TextWriter @out, TextWriter error)
        {
            _configFile = configFile;
            _out = @out;
            _error = error;
        }

        public void InjectAppSetting(string settingKey, string value)
        {
            InjectValue(
                _configFile,
                $"//configuration/appSettings/add[@key = '{settingKey}']/@value",
                value);
        }

        public void InjectWcfEndpoint(string endpointName, string endpoint)
        {
            InjectValue(
                _configFile,
                $"//configuration/system.serviceModel/client/endpoint[@name = '{endpointName}']/@address",
                endpoint);
        }

        public void InjectConnectionString(string connectionStringName, string connectionString)
        {
            InjectValue(
                _configFile,
                $"//configuration/connectionStrings/add[@name = '{connectionStringName}']/@connectionString",
                connectionString);
        }
        private void InjectValue(string filename, string xpath, string value)
        {
            var xml = new XmlDocument();
            xml.Load(filename);

            var node = xml.SelectSingleNode(xpath);
            if (node == null)
            {
                _error.WriteLine($"Cannot find {xpath}.");
                return;
            }

            node.Value = value;

            xml.Save(filename);

            _out.WriteLine($"Injected {value} into {filename} at {xpath}.");
        }
    }
}