using CSharpx;

namespace Injector
{
    public static class OptionsValidator
    {
        public static Maybe<ExitCode> Validate(Options opts)
        {
            if (opts.IsJsonPath && opts.IsValueEnvironmentVariable)
                return Maybe.Just(ExitCode.JsonPathAndEnvVarAreMutuallyExclusive);
            return Maybe.Nothing<ExitCode>();
        }
    }
}