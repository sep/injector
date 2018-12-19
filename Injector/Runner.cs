using System;
using System.IO;
using dotenv.net;
using Monad;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Injector
{
    public static class Runner
    {
        public static Option<ExitCode> Run(Options opts)
        {
            var validated = OptionsValidator.Validate(opts);

            if (validated.HasValue())
            {
                return validated;
            }

            if (opts.EnvFileSpecified)
                LoadEnv(opts);

            var value = opts.IsValueEnvironmentVariable ? GetEnvVar(opts.Value)
                : opts.IsJsonPath ? GetJsonValue(opts.Value, opts.JsonFile)
                : opts.Value;

            var injection = new Injection(opts.ConfigFile, Console.Out, Console.Error);

            if (opts.IsAppSetting)
                injection.InjectAppSetting(opts.Name, value);
            else if (opts.IsConnectionString)
                injection.InjectConnectionString(opts.Name, value);
            else if (opts.IsWcfEndpoint) injection.InjectWcfEndpoint(opts.Name, value);

            return Option.Nothing<ExitCode>();
        }

        private static void LoadEnv(Options opts)
        {
            if (!File.Exists(opts.EnvFile))
            {
                Console.Error.WriteLine($"Environment file '{opts.EnvFile}' does not exist.");
            }
            else
            {
                Console.WriteLine($"Loaded Environment file '{opts.EnvFile}'.");
                DotEnv.Config(
                    throwOnError: false,
                    filePath: opts.EnvFile);
            }
        }

        private static string GetEnvVar(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Process)
                   ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User)
                   ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);
        }

        private static string GetJsonValue(string jsonPath, string jsonFile)
        {
            using (var file = File.OpenText(jsonFile))
            using (var reader = new JsonTextReader(file))
            {
                var token = JToken.ReadFrom(reader);
                var selected = token.SelectToken(jsonPath);
                return (selected ?? jsonPath).ToString();
            }
        }
    }
}