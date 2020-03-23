using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;

namespace Injector
{
    /// <summary>
    /// JSON file updating parts of this adapted from: https://stackoverflow.com/a/57990271/335675
    /// </summary>
    public class AppSettingsJsonInjection : IInjection
    {
        private readonly TextWriter _out;
        private readonly TextWriter _error;
        private readonly string _configFile;
        private readonly IConfigurationRoot _config;

        public AppSettingsJsonInjection(string configFile, TextWriter @out, TextWriter error)
        {
            _configFile = Path.IsPathRooted(configFile)
                ? configFile
                : Path.Combine(Environment.CurrentDirectory, configFile);
            _out = @out;
            _error = error;

            _config = new ConfigurationBuilder()
               .Add<WritableJsonConfigurationSource>(s =>
               {
                   s.FileProvider = null;
                   s.Path = _configFile;
                   s.Optional = false;
                   s.ResolveFileProvider();
                })
               .Build();
        }

        public void InjectAppSetting(string settingKey, string value)
        {
            InjectValue(settingKey, value);
        }

        public void InjectWcfEndpoint(string endpointName, string endpoint)
        {
            _error.WriteLine($"Injecting WCF endpoints is not supported for appsettings.json based files");
            throw new System.NotImplementedException("This is unsupported.");
        }

        public void InjectConnectionString(string connectionStringName, string connectionString)
        {
            InjectValue($"ConnectionStrings:{connectionStringName}", connectionString);
        }

        private void InjectValue(string settingKey, string value)
        {
            _config.GetSection(settingKey).Value = value;
            _out.WriteLine($"Injected {value} into {_configFile} at {settingKey}.");
        }
    }

    public class WritableJsonConfigurationProvider : JsonConfigurationProvider
    {
        public WritableJsonConfigurationProvider(JsonConfigurationSource source) : base(source)
        {
        }

        public override void Set(string key, string value)
        {
            base.Set(key, value);

            //Get Whole json file and change only passed key with passed value. It requires modification if you need to support change multi level json structure
            var fileFullPath = Source.FileProvider.GetFileInfo(base.Source.Path).PhysicalPath;
            var json = File.ReadAllText(fileFullPath);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            SetVal(jsonObj, key.Split(':'), value);
            var output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(fileFullPath, output);
        }

        private static void SetVal(dynamic jsonObj, IList<string> segments, string value)
        {
            if (segments.Count == 1)
            {
                jsonObj[segments[0]] = value;
                return;
            }
            SetVal(jsonObj[segments[0]], segments.Skip(1).ToList(), value);
        }
    }
    public class WritableJsonConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new WritableJsonConfigurationProvider(this);
        }
    }
}