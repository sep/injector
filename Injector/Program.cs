using System;
using System.Collections.Generic;
using System.Xml;
using CommandLine;

namespace Injector
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(HandleParseError);
        }

        private static void Run(Options opts)
        {
            var value = opts.IsValueEnvironmentVariable
                ? GetEnvVar(opts.Value)
                : opts.Value;

            if (opts.IsConnectionString)
                InjectConnectionString(opts.ConfigFile, opts.Name, value);
            else if(opts.IsWcfEndpoint)
                InjectWcfEndpoint(opts.ConfigFile, opts.Name, value);
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
        }

        private static void InjectWcfEndpoint(string configFile, string endpointName, string endpoint)
        {
            InjectValue(
                configFile,
                $"//configuration/system.serviceModel/client/endpoint[@name = '{endpointName}']/@address",
                endpoint);
        }

        private static void InjectConnectionString(string configFile, string connectionStringName, string connectionString)
        {
            InjectValue(
                configFile,
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
                throw new InvalidOperationException($"Cannot find {xpath}.");
            }

            node.Value = value;

            xml.Save(filename);

            Console.Out.WriteLine($"Injected {value} into {filename} at {xpath}.");
        }
        private static string GetEnvVar(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Process)
                ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User)
                ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);
        }
    }
    class Options
    {
        [Option('c', "connection_string", HelpText = "Whether or not this is a connection string replacement.")]
        public bool IsConnectionString { get; set; }

        [Option('w', "wcf_client_endpoint", HelpText = "Whether or not this is a WCF client endpoint replacement.")]
        public bool IsWcfEndpoint{ get; set; }

        [Option('e', "environment_value", HelpText = "Whether or not VALUE is an environment variable. If it is, the value will be pulled from the environment named by VALUE.")]
        public bool IsValueEnvironmentVariable { get; set; }

        [Value(0, MetaName = "config file path", Required = true, HelpText = "config file path")]
        public string ConfigFile { get; set; }

        [Value(1, MetaName = "connection string name", Required = true, HelpText = "name of element to inject (either connection string name or WCF client endpoint name)")]
        public string Name { get; set; }

        [Value(2, MetaName = "value", Required = true, HelpText = "the value to be injected OR name of environment variable to use when injecting (combined with -e option)")]
        public string Value { get; set; }
    }
}
