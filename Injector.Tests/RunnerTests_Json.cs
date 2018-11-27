using System.IO;
using Xunit;

namespace Injector.Tests
{
    public class RunnerTests_Json
    {
        [Fact]
        public void UsesJsonFileForValue_JsonArray()
        {
            var json = @"[
    {
        ""key"": ""ProductionLogging"",
        ""value"": ""Info""
    },
    {
        ""key"": ""StagingLogging"",
        ""value"": ""Error""
    },
    {
        ""key"": ""DebugLogging"",
        ""value"": ""Debug""
    }
]";
            var original = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""LoggingLevel"" value=""None"" />
  </appSettings>
</configuration>
";

            var expected = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""LoggingLevel"" value=""Info"" />
  </appSettings>
</configuration>
";

            var jsonFile = Path.GetTempFileName();

            using (TestHelper.WithFile(jsonFile))
            {
                TestHelper.WithContent(jsonFile, json.Trim(), () =>
                {
                    TestHelper.TestRunner(original, expected, new Options
                    {
                        ConfigFile = Path.GetTempFileName(),
                        IsAppSetting = true,
                        IsJsonPath = true,
                        JsonFile = jsonFile,
                        Name = "LoggingLevel",
                        Value = "$.[?(@.key == 'ProductionLogging')].value"
                    });
                });
            }
        }
        [Fact]
        public void UsesJsonFileForValue_JsonObject()
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

            using (TestHelper.WithFile(jsonFile))
            {
                TestHelper.WithContent(jsonFile, json.Trim(), () =>
                {
                    TestHelper.TestRunner(original, expected, new Options
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
    }
}