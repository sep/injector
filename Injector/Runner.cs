using System;
using System.IO;
using dotenv.net;
using Monad;
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

            var injection = new Injection(opts.ConfigFile);

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
            var data = JObject.Parse(File.ReadAllText(jsonFile));
            var token = data.SelectToken(jsonPath);
            return token.ToString();
        }
    }
}