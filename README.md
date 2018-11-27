# Injector

## What is it?

Console app for injecting values or the value of environment variables into .NET config files.

Can be used to inject...

* App settings
* Database connection strings
* WCF client endpoint addresses

## Usage

### CLI

```
Injector 1.1.1
Copyright c 2018 SEP, Inc

  -a, --app_setting                  Whether or not this is an app setting injection.

  -c, --connection_string            Whether or not this is a connection string injection.

  -w, --wcf_client_endpoint          Whether or not this is a WCF client endpoint injection.

  -e, --environment_value            Whether or not VALUE is an environment variable. If it is, the value will be pulled from the environment named by VALUE.

  -j, --json_path                    Whether or not VALUE is a json path (https://www.newtonsoft.com/json/help/html/QueryJsonSelectTokenJsonPath.htm). If it is, the value will be the result of querying the JSON
                                     file specified by 'json_path'.

  --envFile                          Path to a ".env" file (or other filename).

  --json_file                        Path to a JSON file for use in conjunction with '-j' or '--json_path'.

  --help                             Display this help screen.

  --version                          Display version information.

  config file path (pos. 0)          Required. config file path

  connection string name (pos. 1)    Required. name of element to inject (app setting key, connection string name, or WCF client endpoint name)

  value (pos. 2)                     Required. the value to be injected OR name of environment variable to use when injecting (combined with -e option)
  ```

### App Settings

`Injector.exe --app_setting path/to/App.config LoggingEnabled true`

or from environment variable

`Injector.exe --environment_value -app_setting path/to/App.config LoggingEnabled APP_LOGGING_ENABLED`

or from JSON file

`Injector.exe --json_path --json_file=example.json --app_setting path/to/App.config LoggingEnabled "$.[?(@.key=='ProductionLogging')].value"`

for JSON file:
```
# example.json

[
    {
        "key": "ProductionLogging",
        "value": "false"
    },
    {
        "key": "StagingLogging",
        "value": "true"
    },
    {
        "key": "DebugLogging",
        "value": "true"
    }
]
```

This updates the `LoggingEnabled` app setting in the following `App.config` example:

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <!-- snip -->
        <appSettings>
            <add key="LoggingEnabled" value="..." />
        </appSettings>

        <!-- snip -->
    </configuration>

**Note**: Here are some resources for the JSONPath mentioned above (as specified by the `j` flag):

* http://jsonpath.com/ - A live evaluator
* https://goessner.net/articles/JsonPath/index.htm - Some documentation of the specification

### Database Connection String

`Injector.exe -c path/to/App.config DatabaseServer "Data Source=host;Initial Catalog=database;User ID=username;Password=$3cr$t;"`

or from environment variable

`Injector.exe -e -c path/to/App.config DatabaseServer DB_CONN_STR`

This updates the `DatabaseServer` connection string in the following `App.config` example:

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <!-- snip -->
        <connectionStrings>
            <add name="DatabaseServer" connectionString="..." />
        </connectionStrings>

        <system.web>
            <!-- snip -->
        </system.web>
    </configuration>

### WCF Client Endpoint Address

`Injector.exe -w path/to/App.config MasterEndpoint net.tcp://localhost:5000/services/ping;`

or from environment variable

`Injector.exe -e -w path/to/App.config MasterEndpoint ENDPOINT_STR`

This updates the service client enpoint with name `MasterEndpoint` in the following `App.config` example:

    <?xml version="1.0" encoding="utf-8"?>

    <configuration>
    <!-- snip -->
    <system.serviceModel>
        <client>
            <endpoint name="MasterEndpoint" bindingConfiguration="MasterTcpEndPoint" address="ADDRESS TO REPLACE" binding="netTcpBinding" contract="IMasterEndpoint" />
        </client>

        <!-- snip -->
    </system.serviceModel>

## Installation

`Injector` is also available via NuGet:

PM> Install-Package Injector 
Or visit: https://www.nuget.org/packages/Injector/

---

[![Powered by SEP logo](https://raw.githubusercontent.com/sep/assets/master/images/powered-by-sep.svg?sanitize=true)](https://www.sep.com)

Injector is supported by SEP: a Software Product Design + Development company. If you'd like to [join our team](https://www.sep.com/careers/open-positions/), don't hesitate to get in touch!
