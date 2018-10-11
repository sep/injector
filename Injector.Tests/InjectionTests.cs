using System;
using System.IO;
using Xunit;

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

        private void TestInjection(string originalContent, string expectedContent, Action<Injection> exercise)
        {
            using (WithFile(filename =>
            {
                var actualContent = WithContent(filename, originalContent.Trim(), () =>
                {
                    var injection = new Injection(filename);

                    exercise(injection);
                });


                Assert.Equal(expectedContent.Trim(), actualContent.Trim());
            }));
        }

        private IDisposable WithFile(Action<string> doTest)
        {
            var filename = Path.GetTempFileName();
            doTest(filename);
            return Disposing.Disposable.Create(() => File.Delete(filename));
        }

        private string WithContent(string filename, string content, Action test)
        {
            File.WriteAllText(filename, content);

            test();

            return File.ReadAllText(filename);
        }
    }
}
