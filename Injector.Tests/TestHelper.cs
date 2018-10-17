using System;
using System.IO;
using Disposing;
using Xunit;

namespace Injector.Tests
{
    public static class TestHelper
    {
        public static void TestInjection(string originalContent, string expectedContent, Action<Injection> exercise)
        {
            using (WithTempFile(filename =>
            {
                var actualContent = WithContent(filename, originalContent.Trim(), () =>
                {
                    var injection = new Injection(filename);

                    exercise(injection);
                });


                Assert.Equal(expectedContent.Trim(), actualContent.Trim());
            })) ;
        }

        public static void TestRunner(string originalContent, string expectedContent, Options opts)
        {
            using (WithFile(opts.ConfigFile, () =>
            {
                var actualContent = WithContent(opts.ConfigFile, originalContent.Trim(), () =>
                {
                    Runner.Run(opts);
                });

                Assert.Equal(expectedContent.Trim(), actualContent.Trim());
            })) ;
        }

        public static IDisposable WithEnvVar(string variableName, string value)
        {
            var currentValue = Environment.GetEnvironmentVariable(variableName);
            Environment.SetEnvironmentVariable(variableName, value);
            return Disposable.Create(() => Environment.SetEnvironmentVariable(variableName, currentValue));
        }

        public static IDisposable WithTempFile(Action<string> doTest)
        {
            var filename = Path.GetTempFileName();
            doTest(filename);
            return Disposable.Create(() => File.Delete(filename));
        }

        public static IDisposable WithFile(string filename, Action doTest)
        {
            doTest();
            return Disposable.Create(() => File.Delete(filename));
        }
        public static IDisposable WithFile2(string filename)
        {
            return Disposable.Create(() => File.Delete(filename));
        }

        public static string WithContent(string filename, string content, Action test)
        {
            File.WriteAllText(filename, content);

            test();

            return File.ReadAllText(filename);
        }
    }
}