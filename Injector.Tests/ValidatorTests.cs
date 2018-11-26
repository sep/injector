using System.IO;
using CSharpx;
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

            Assert.True(result.IsJust());
            result.Match(
                ifJust: v => Assert.Equal(v.Value, ExitCode.JsonPathAndEnvVarAreMutuallyExclusive.Value),
                ifNothing: () => Assert.True(false, "Bad case"));
        }
    }
}