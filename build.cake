///////////////////////////////////////////////////////////////////////////////
// Add-ins
///////////////////////////////////////////////////////////////////////////////

#addin "Cake.Git"

///////////////////////////////////////////////////////////////////////////////
// Tools
///////////////////////////////////////////////////////////////////////////////

#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=NUnit.ConsoleRunner"

///////////////////////////////////////////////////////////////////////////////
// Build variables
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solution = "./Tynamix.ObjectFiller.RegExPlugin.sln";
var appName = "Tynamix.ObjectFiller.RegEx";

var local = BuildSystem.IsLocalBuild;
var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var buildNumber = AppVeyor.Environment.Build.Number;


var branchName = isRunningOnAppVeyor ? EnvironmentVariable("APPVEYOR_REPO_BRANCH") : GitBranchCurrent(DirectoryPath.FromString(".")).FriendlyName;
var isMasterBranch = System.String.Equals("master", branchName, System.StringComparison.OrdinalIgnoreCase);

///////////////////////////////////////////////////////////////////////////////
// Version
///////////////////////////////////////////////////////////////////////////////

var gitVersion = GitVersion();

///////////////////////////////////////////////////////////////////////////////
// Prepare
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() => {
        CleanDirectory("./nuget");
    });

Task("Restore-Nuget-Packages")
    .IsDependentOn("Clean")
    .Does(() => {
        NuGetRestore(solution);
    });

//////////////////////////////////////////////////////////////////////////////
// Build
//////////////////////////////////////////////////////////////////////////////

Task("Build")
    .IsDependentOn("Restore-Nuget-Packages")
    .Does(() => {
        MSBuild(solution, new MSBuildSettings {
            Configuration = configuration
        });
    });

//////////////////////////////////////////////////////////////////////////////
// Test
//////////////////////////////////////////////////////////////////////////////

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        NUnit3("./test/**/bin/Release/**/*.Tests.dll");
    });

//////////////////////////////////////////////////////////////////////////////
// Nuget
//////////////////////////////////////////////////////////////////////////////

Task("Pack")
    .IsDependentOn("Test")
    .Does(() => {

        CreateDirectory("nuget");
        CleanDirectory("nuget");

        var nuGetPackSettings   = new NuGetPackSettings {
                                Id                       = appName,
                                Version                  = gitVersion.NuGetVersionV2,
                                Title                    = appName,
                                Authors                  = new[] { "Erik Lichtenstein" },
                                Owners                   = new[] { "Erik Lichtenstein" },
                                Description              = "The extension for ObjectFiller .NET allows to generate string values that match a given regular expression.",
                                IconUrl                  = new Uri("https://github.com/Lichtel/ObjectFiller.NET-RegExPlugin/blob/master/logo.png"),
                                ProjectUrl               = new Uri("http://objectfiller.net/"),
                                LicenseUrl               = new Uri("https://github.com/Lichtel/ObjectFiller.NET-RegExPlugin/blob/master/LICENCE"),
                                Tags                     = new [] { "ObjectFiller", "Tynamix", "Test", "Data", "Regex", "Regular", "Expression" },
                                ReleaseNotes             = new [] { "Initial release" },
                                RequireLicenseAcceptance = false,
                                Symbols                  = false,
                                Files                    = new [] {
                                                                     new NuSpecContent { Source = "net35/Tynamix.ObjectFiller.RegExPlugin.dll", Target = "lib/net35" },
                                                                     new NuSpecContent { Source = "net35/Tynamix.ObjectFiller.RegExPlugin.xml", Target = "lib/net35" },
                                                                     new NuSpecContent { Source = "netstandard1.1/Tynamix.ObjectFiller.RegExPlugin.dll", Target = "lib/netstandard1.1" },
                                                                     new NuSpecContent { Source = "netstandard1.1/Tynamix.ObjectFiller.RegExPlugin.xml", Target = "lib/netstandard1.1" }
                                                                  },
                                Dependencies             = new [] {
                                                                     new NuSpecDependency { Id = "Tynamix.ObjectFiller", TargetFramework = "net35", Version = "1.5.4.1" },
                                                                     new NuSpecDependency { Id = "Fare", TargetFramework = "net35", Version = "2.1.1" },
                                                                     new NuSpecDependency { Id = "NETStandard.Library", TargetFramework = "netstandard1.1", Version = "1.6.1" },
                                                                     new NuSpecDependency { Id = "Tynamix.ObjectFiller", TargetFramework = "netstandard1.1", Version = "1.5.4.1" },
                                                                     new NuSpecDependency { Id = "Fare", TargetFramework = "netstandard1.1", Version = "2.1.1" }
                                                                  },
                                BasePath                 = "./src/bin/release",
                                OutputDirectory          = "./nuget"
                            };

        NuGetPack(nuGetPackSettings);
    });

Task("Publish")
    .IsDependentOn("Pack")
    .WithCriteria(() => isRunningOnAppVeyor)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isMasterBranch)
    .Does(() => {

        var apiKey = EnvironmentVariable("NUGET_API_KEY");

        if(string.IsNullOrEmpty(apiKey))    
            throw new InvalidOperationException("Could not resolve Nuget API key.");

        var package = "./nuget/" + appName + "." + gitVersion.NuGetVersionV2 + ".nupkg";

        // Push the package.
        NuGetPush(package, new NuGetPushSettings {
            Source = "https://www.nuget.org/api/v2/package",
            ApiKey = apiKey
        });
    });

///////////////////////////////////////////////////////////////////////////////
// Appveyor tasks
///////////////////////////////////////////////////////////////////////////////

Task("Update-AppVeyor-Build-Number")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion(gitVersion.FullSemVer);
});

Task("AppVeyor")
    .IsDependentOn("Update-AppVeyor-Build-Number")
    .IsDependentOn("Publish");

///////////////////////////////////////////////////////////////////////////////
// Execution
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);
