using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(BLSN.BuildInfo.Description)]
[assembly: AssemblyDescription(BLSN.BuildInfo.Description)]
[assembly: AssemblyCompany(BLSN.BuildInfo.Company)]
[assembly: AssemblyProduct(BLSN.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + BLSN.BuildInfo.Author)]
[assembly: AssemblyTrademark(BLSN.BuildInfo.Company)]
[assembly: AssemblyVersion(BLSN.BuildInfo.Version)]
[assembly: AssemblyFileVersion(BLSN.BuildInfo.Version)]
[assembly: MelonInfo(typeof(BLSN.BLSN), BLSN.BuildInfo.Name, BLSN.BuildInfo.Version, BLSN.BuildInfo.Author, BLSN.BuildInfo.DownloadLink)]
[assembly: MelonColor()]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]