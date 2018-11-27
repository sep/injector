using System.Collections.Generic;
using CommandLine;
using Monad;

namespace Injector
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    parsedFunc: Runner.Run,
                    notParsedFunc: HandleParseError)
                .Match(
                    Just: _ => _,
                    Nothing: ExitCode.Nominal)
                ().Value;
        }

        private static Option<ExitCode> HandleParseError(IEnumerable<Error> errs)
        {
            return Option.Return(() => ExitCode.OptionsParsingError);
        }
    }

    public class ExitCode
    {
        public static ExitCode OptionsParsingError => new ExitCode(-1);
        public static ExitCode Nominal => new ExitCode(0);
        public static ExitCode JsonPathAndEnvVarAreMutuallyExclusive => new ExitCode(1);

        private ExitCode(int value)
        {
            Value = value;
        }

        public int Value { get; }
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

        [Option('j', "json_path", HelpText = "Whether or not VALUE is a json path (https://www.newtonsoft.com/json/help/html/QueryJsonSelectTokenJsonPath.htm). If it is, the value will be the result of querying the JSON file specified by 'json_path'.")]
        public bool IsJsonPath { get; set; }

        [Option(longName: "envFile", Required = false, HelpText = "Path to a \".env\" file (or other filename).")]
        public string EnvFile { get; set; }

        [Option(longName: "json_file", Required = false, HelpText = "Path to a JSON file for use in conjunction with '-j' or '--json_path'.")]
        public string JsonFile { get; set; }

        [Value(0, MetaName = "config file path", Required = true, HelpText = "config file path")]
        public string ConfigFile { get; set; }

        [Value(1, MetaName = "connection string name", Required = true, HelpText = "name of element to inject (app setting key, connection string name, or WCF client endpoint name)")]
        public string Name { get; set; }

        [Value(2, MetaName = "value", Required = true, HelpText = "the value to be injected OR name of environment variable to use when injecting (combined with -e option)")]
        public string Value { get; set; }

        public bool EnvFileSpecified => EnvFile != null;
    }
}
