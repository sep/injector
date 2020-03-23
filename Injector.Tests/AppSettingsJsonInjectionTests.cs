using Xunit;

namespace Injector.Tests
{
    public class AppSettingsJsonInjectionTests
    {
        [Fact]
        public void InjectsAppSetting()
        {
            var original = @"
{
  ""TestKey"": ""original value""
}
";

            var expected = @"
{
  ""TestKey"": ""new value""
}
";

            TestHelper.TestAppSettingsJsonInjection(original, expected,
                injection => injection.InjectAppSetting("TestKey", "new value"));
        }

        [Fact]
        public void InjectsConnectionString()
        {
            var original = @"
{
  ""ConnectionStrings"": {
    ""DatabaseConnection"": ""original connection string""
  }
}";

            var expected = @"
{
  ""ConnectionStrings"": {
    ""DatabaseConnection"": ""new connection string""
  }
}";

            TestHelper.TestAppSettingsJsonInjection(original, expected,
                injection => injection.InjectConnectionString("DatabaseConnection", "new connection string"));
        }
    }
}