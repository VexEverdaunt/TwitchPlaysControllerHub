# TwitchPlaysControllerHub
An easy to use program that will allow you to easily create and manage your own Twitch Plays Channel! 

## Building

1. Open the TwitchPlaysHub.sln file with Visual Studio.
2. Generate a key for code signing (don't share this with anyone)

This can be done by selecting properties on the TwitchPlaysHub C# project, choosing the 'Signing' section, and clicking 'Create Test Certificate'.

3. Restore nuget packages

This can be done by right clicking on the 'TwitchPlaysHub' solution in the Solution Explorer and clicking 'Restore NuGet Packages'. Alternatively, from the command line, in the same directory as the .sln file, run `nuget restore -SolutionDirectory . TwitchPlaysHub\packages.config`

4. Build and run

Use Visual Studio to run the application, for example by clicking >. Alternatively you can run `msbuild` from the command line in the same directory as the .sln file.
