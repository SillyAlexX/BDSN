using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(BDSN.BuildInfo.Description)]
[assembly: AssemblyDescription(BDSN.BuildInfo.Description)]
[assembly: AssemblyCompany(BDSN.BuildInfo.Company)]
[assembly: AssemblyProduct(BDSN.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + BDSN.BuildInfo.Author)]
[assembly: AssemblyTrademark(BDSN.BuildInfo.Company)]
[assembly: AssemblyVersion(BDSN.BuildInfo.Version)]
[assembly: AssemblyFileVersion(BDSN.BuildInfo.Version)]
[assembly: MelonInfo(typeof(BDSN.BDSN), BDSN.BuildInfo.Name, BDSN.BuildInfo.Version, BDSN.BuildInfo.Author, BDSN.BuildInfo.DownloadLink)]
[assembly: MelonColor()]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]