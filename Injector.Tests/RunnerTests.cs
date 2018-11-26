using System.IO;
using Xunit;
using static Injector.Tests.TestHelper;

namespace Injector.Tests
{
    public class RunnerTests
    {
        [Fact]
        public void UsesJsonFileForValue()
        {
            var json = @"{
  'Stores': [
    'Lambton Quay',
    'Willis Street'
  ],
  'Manufacturers': [
    {
      'Name': 'Acme Co',
      'Products': [
        {
          'Name': 'Anvil',
          'Price': 50
        }
      ]
    },
    {
      'Name': 'Contoso',
      'Products': [
        {
          'Name': 'Elbow Grease',
          'Price': 99.95
        },
        {
          'Name': 'Headlight Fluid',
          'Price': 4
        }
      ]
    }
  ]
}";
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
    <add key=""TestKey"" value=""Acme Co"" />
  </appSettings>
</configuration>
";

            var jsonFile = Path.GetTempFileName();

            using (WithFile(jsonFile))
            {
                WithContent(jsonFile, json.Trim(), () =>
                {
                    TestRunner(original, expected, new Options
                    {
                        ConfigFile = Path.GetTempFileName(),
                        IsAppSetting = true,
                        IsJsonPath = true,
                        JsonFile = jsonFile,
                        Name = "TestKey",
                        Value = "$.Manufacturers[?(@.Name == 'Acme Co')].Name"
                    });
                });
            }
        }

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

            using (WithFile(".env"))
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