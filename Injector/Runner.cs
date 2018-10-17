using System;
using System.IO;
using dotenv.net;

namespace Injector
{
    public static class Runner
    {
        public static void Run(Options opts)
        {
            if (opts.EnvFileSpecified)
                LoadEnv(opts);

            var value = opts.IsValueEnvironmentVariable
                ? GetEnvVar(opts.Value)
                : opts.Value;

            var injection = new Injection(opts.ConfigFile);

            if (opts.IsAppSetting)
                injection.InjectAppSetting(opts.Name, value);
            else if (opts.IsConnectionString)
                injection.InjectConnectionString(opts.Name, value);
            else if (opts.IsWcfEndpoint) injection.InjectWcfEndpoint(opts.Name, value);
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
    }
}