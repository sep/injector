﻿using System.Collections.Generic;
using CommandLine;

namespace Injector
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Runner.Run)
                .WithNotParsed(HandleParseError);
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
        }
    }

    public class Options
    {
        [Option('a', "app_setting", HelpText = "Whether or not this is an app setting injection.")]
        public bool IsAppSetting { get; set; }

        [Option('c', "connection_string", HelpText = "Whether or not this is a connection string injection.")]
        public bool IsConnectionString { get; set; }

        [Option('w', "wcf_client_endpoint", HelpText = "Whether or not this is a WCF client endpoint injection.")]
        public bool IsWcfEndpoint{ get; set; }

        [Option('e', "environment_value", HelpText = "Whether or not VALUE is an environment variable. If it is, the value will be pulled from the environment named by VALUE.")]
        public bool IsValueEnvironmentVariable { get; set; }

        [Option(longName: "envFile", Required = false, HelpText = "Path to a \".env\" file (or other filename).")]
        public string EnvFile { get; set; }

        [Value(0, MetaName = "config file path", Required = true, HelpText = "config file path")]
        public string ConfigFile { get; set; }

        [Value(1, MetaName = "connection string name", Required = true, HelpText = "name of element to inject (app setting key, connection string name, or WCF client endpoint name)")]
        public string Name { get; set; }

        [Value(2, MetaName = "value", Required = true, HelpText = "the value to be injected OR name of environment variable to use when injecting (combined with -e option)")]
        public string Value { get; set; }

        public bool EnvFileSpecified => EnvFile != null;
    }
}
