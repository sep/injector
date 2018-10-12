using Xunit;
using static Injector.Tests.TestHelper;

namespace Injector.Tests
{
    public class InjectionTests
    {
        [Fact]
        public void InjectsAppSetting()
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
    <add key=""TestKey"" value=""new value"" />
  </appSettings>
</configuration>
";

            TestInjection(original, expected,
                injection => injection.InjectAppSetting("TestKey", "new value"));
        }

        [Fact]
        public void InjectsConnectionString()
        {
            var original = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <connectionStrings>
    <add name=""DatabaseConnection"" connectionString=""original connection string"" />
  </connectionStrings>
</configuration>
";

            var expected = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <connectionStrings>
    <add name=""DatabaseConnection"" connectionString=""new connection string"" />
  </connectionStrings>
</configuration>
";

            TestInjection(original, expected,
                injection => injection.InjectConnectionString("DatabaseConnection", "new connection string"));
        }

        [Fact]
        public void InjectsWcfEndpoint()
        {
            var original = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <system.serviceModel>
    <client>
      <endpoint name=""ServiceName"" address=""original endpoint address"" />
    </client>
  </system.serviceModel>
</configuration>
";

            var expected = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <system.serviceModel>
    <client>
      <endpoint name=""ServiceName"" address=""new endpoint address"" />
    </client>
  </system.serviceModel>
</configuration>
";

            TestInjection(original, expected,
                injection => injection.InjectWcfEndpoint("ServiceName", "new endpoint address"));
        }
    }
}
