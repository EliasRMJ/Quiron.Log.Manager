## What is the Quiron.Log.Manager?

Package created to customize the server's default log manager (ILogger), as well as providing an interface to format the log text into a JSON list.

## Give a Star! ⭐

If you find this project useful, please give it a star! It helps us grow and improve the community.

## Installation

```csharp
dotnet add package Quiron.Log.Manager
```

## Namespaces and Dependencies

- ✅ Quiron.Log.Manager
- ✅ Microsoft.Extensions.Logging

## Methods

- ✅ GetAsync

## Some Basic Examples

### Your program.cs file
```csharp
public static void RegisterLog(this WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IViewLog, ViewLog>();

    builder.Logging.ClearProviders();
    builder.Logging.AddProvider(new FileLoggerProvider("Logs")); Note: You can enter the folder name in 'FileLoggerProvider'; by default, if not entered, it will be 'Logs'
}
```

### Getting the Log
```csharp
public class MyClass(IViewLog viewLog)
{
    public async Task<ViewLogViewModel[]> GetAsync(DateOnly begin, DateOnly end, string text = "")
    {
        return await viewLog.GetAsync(begin, end, text);
    }
}
```
- ⚠️ Use dependency injection to add your class or method to the 'IViewLog' interface

## Usage Reference

For more details, access the test project that has the practical implementation of the package's use.

https://github.com/EliasRMJ/Quiron.EntityFrameworkCore.Test

Supports:

- ✅ .NET Standard 2.1  
- ✅ .NET 9 through 9 (including latest versions)  
- ⚠️ Legacy support for .NET Core 3.1 and older (with limitations)
  
## About
Quiron.Log.Manager was developed by [EliasRMJ](https://www.linkedin.com/in/elias-medeiros-98232066/) under the [MIT license](LICENSE).