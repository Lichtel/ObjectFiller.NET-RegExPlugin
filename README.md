[![Build status](https://ci.appveyor.com/api/projects/status/agpo7c366s6e449n?svg=true)](https://ci.appveyor.com/project/Lichtel/objectfiller-net-reverseregex)

# ObjectFiller.NET RegEx Plugin

<img align="left" src="https://raw.githubusercontent.com/Lichtel/ObjectFiller.NET-RegExPlugin/master/logo.png" alt="Logo" />
The plugin for
[ObjectFiller.NET](https://github.com/Tynamix/ObjectFiller.NET)
allows you to generate string values based on regular expressions. The resulting string values will match the given regex. The reverse regex functionality is provided by the [Fare](https://github.com/moodmosaic/Fare) project.

## NuGet

The plugin is available on NuGet:

[![NuGet](https://img.shields.io/nuget/v/Tynamix.ObjectFiller.RegEx.svg)](https://www.nuget.org/packages/Tynamix.ObjectFiller.RegEx/)

## Usage

```csharp
    /// <summary>
    /// Person model for code sample.
    /// </summary>
    public class TestPersonModel
        {
            public Guid Identifier { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }

    // ...
    // ObjectFiller.NET RegEx Plugin usage sample creates 100 instances with pattern-matching email adresses.

    var testPersonFiller = new Filler<TestPersonModel>();
    testPersonFiller.Setup()
        .OnProperty(t => t.Email).Use(new ReverseRegEx(@"^([a-z0-9\.\-]+)@([a-z0-9\-]+)((\.([a-z]){2,3})+)$"));

    var testPersons = testPersonFiller.Create(100);
```
