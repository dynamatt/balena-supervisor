# Balena Supervisor API for .NET

.NET API for the Balena supervisor. Enables control of the Balena supervisor via the HTTP API.

## Getting Started

### Installing

The package may be installed using the NuGet package explorer in Visual Studio or using the `dotnet` CLI:

```sh
dotnet add package Balena.Supervisor
```


### Using the API

A connection to the Balena cloud is created with a `BalenaCloudConnection`:

```c#
using Balena.Cloud;
using Balena.Models;

var connection = new BalenaCloudConnection("https://api.balena-cloud.com/v5", "exampleapitoken");

// all API calls are asynchronous
List<Application> applications = await connection.GetAllApplications();
```

## Development

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

To build the project the `dotnet` CLI must be installed.

### Running the tests

Tests require a Balena Cloud account as the API is called during testing.

The tests require that the Balena Cloud API key is set in the `BALENA_API_TOKEN` environment variable of the system that is running the tests.

Tests may then be run using the CLI command:

```sh
dotnet test
```

Note that for automated tests running from GitHub, the API token is set as one of the project secrets.



## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
