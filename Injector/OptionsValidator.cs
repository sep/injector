using Monad;

namespace Injector
{
    public static class OptionsValidator
    {
        public static Option<ExitCode> Validate(Options opts)
        {
            if (opts.IsJsonPath && opts.IsValueEnvironmentVariable)
                return Option.Return(() => ExitCode.JsonPathAndEnvVarAreMutuallyExclusive);
            return Option.Nothing<ExitCode>();
        }
    }
}