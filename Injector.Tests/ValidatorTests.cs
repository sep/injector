using System.IO;
using Monad;
using Xunit;

namespace Injector.Tests
{
    public class ValidatorTests
    {
        [Fact]
        public void IsJsonPathAndIsEnvVarAreMutuallyExclusive()
        {
            var opts = new Options
            {
                ConfigFile = Path.GetTempFileName(),
                IsAppSetting = true,
                IsJsonPath = true,
                IsValueEnvironmentVariable = true,
                Name = "TestKey",
                Value = "$.Manufacturers[?(@.Name == 'Acme Co')].Name"
            };

            var result = OptionsValidator.Validate(opts);

            Assert.True(result.HasValue());
            Assert.Equal(result.Value().Value, ExitCode.JsonPathAndEnvVarAreMutuallyExclusive.Value);
        }
    }
}