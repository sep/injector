using System;

namespace Injector
{
    public static class Runner
    {
        public static void Run(Options opts)
        {
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

        private static string GetEnvVar(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Process)
                   ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User)
                   ?? Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);
        }
    }
}