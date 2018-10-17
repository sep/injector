using System.IO;
using Xunit;
using static Injector.Tests.TestHelper;

namespace Injector.Tests
{
    public class RunnerTests
    {
        [Fact]
        public void UsesEnvironmentVariableForValue()
        {
            var original = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""TestKey"" value=""original value"" />
  </appSettings>
</configuration>
";

            var expected = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""TestKey"" value=""environment value"" />
  </appSettings>
</configuration>
";

            using (WithEnvVar("INJECTOR_ENV_TEST", "environment value"))
            {
                TestRunner(original, expected, new Options
                {
                    ConfigFile = Path.GetTempFileName(),
                    IsAppSetting = true,
                    IsValueEnvironmentVariable = true,
                    Name = "TestKey",
                    Value = "INJECTOR_ENV_TEST"
                });
            }
        }

        [Fact]
        public void UsesPassedValueForValue()
        {
            var original = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""TestKey"" value=""original value"" />
  </appSettings>
</configuration>
";

            var expected = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""TestKey"" value=""INJECTOR_ENV_TEST"" />
  </appSettings>
</configuration>
";

            using (WithEnvVar("INJECTOR_ENV_TEST", "environment value"))
            {
                TestRunner(original, expected, new Options
                {
                    ConfigFile = Path.GetTempFileName(),
                    IsAppSetting = true,
                    IsValueEnvironmentVariable = false,
                    Name = "TestKey",
                    Value = "INJECTOR_ENV_TEST"
                });
            }
        }

        [Fact]
        public void LoadsEnvFileIfSpecified()
        {
            var original = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""TestKey"" value=""original value"" />
  </appSettings>
</configuration>
";

            var expected = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""TestKey"" value=""environment value from file"" />
  </appSettings>
</configuration>
";

            using (WithFile2(".env"))
            {
                File.WriteAllText(".env", "INJECTOR_ENV_TEST=environment value from file");
                TestRunner(original, expected, new Options
                {
                    EnvFile = ".env",
                    ConfigFile = Path.GetTempFileName(),
                    IsAppSetting = true,
                    IsValueEnvironmentVariable = true,
                    Name = "TestKey",
                    Value = "INJECTOR_ENV_TEST"
                });
            }
        }
    }
}