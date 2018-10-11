using System;
using System.Xml;

namespace Injector
{
    public class Injection
    {
        private readonly string _configFile;

        public Injection(string configFile)
        {
            _configFile = configFile;
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
        private static void InjectValue(string filename, string xpath, string value)
        {
            var xml = new XmlDocument();
            xml.Load(filename);

            var node = xml.SelectSingleNode(xpath);
            if (node == null)
            {
                Console.Error.WriteLine($"Cannot find {xpath}.");
                return;
            }

            node.Value = value;

            xml.Save(filename);

            Console.Out.WriteLine($"Injected {value} into {filename} at {xpath}.");
        }
    }
}