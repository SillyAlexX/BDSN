using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(BoneNotifier.BuildInfo.Description)]
[assembly: AssemblyDescription(BoneNotifier.BuildInfo.Description)]
[assembly: AssemblyCompany(BoneNotifier.BuildInfo.Company)]
[assembly: AssemblyProduct(BoneNotifier.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + BoneNotifier.BuildInfo.Author)]
[assembly: AssemblyTrademark(BoneNotifier.BuildInfo.Company)]
[assembly: AssemblyVersion(BoneNotifier.BuildInfo.Version)]
[assembly: AssemblyFileVersion(BoneNotifier.BuildInfo.Version)]
[assembly: MelonInfo(typeof(BoneNotifier.BoneNotifier), BoneNotifier.BuildInfo.Name, BoneNotifier.BuildInfo.Version, BoneNotifier.BuildInfo.Author, BoneNotifier.BuildInfo.DownloadLink)]
[assembly: MelonColor()]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]