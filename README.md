# Injector

## What is it?

Console app for injecting values or the value of environment variables into .NET config files.

Can be used to inject...

* Database connection strings

    whatever

* WCF Client Endpoint addresses

    whatever

## Usage

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
