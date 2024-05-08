using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Linq;
using System.Collections.Generic;
using System;

/// <summary>
/// Used by a batch script (-executeMethod) to automate building process for Unity from commandline or with continuous integration
/// </summary>
public class GameBuilder
{
    static string buildPath = "./Builds/Release";
    static string devPath = "./Builds/Dev";


    [System.Flags]
    public enum BuildFlags
    {
        none = 0,
        devMode = 1 << 0,
        steamworks = 1 << 1,
    }

    static bool PreExport()
    {
        bool success = true;

        //!Not needed if Build Addressables on Player build
        //? See Preferences > Addressables > "Build Addressables on build Player is allowed"
        //? Also see Addressable Asset Settings objects
        // Debug.Log("BuildAddressablesProcessor.PreExport start");
        // success = BuildAddressables();
        // Debug.Log("BuildAddressablesProcessor.PreExport done");

        return success;
    }

    [MenuItem("Build/Release/Build All", false, -10)]
    public static void BuildAll()
    {
        BuildFlags buildFlags = BuildFlags.none;
        // BuildFlags steamBuildFlags = buildFlags | BuildFlags.steamworks;

        _BuildWindows(buildFlags);
        // _BuildWindows(steamBuildFlags);        //*STEAMWORKS

        _BuildOSX(buildFlags);
        // _BuildOSX(steamBuildFlags);        //*STEAMWORKS

        //?_BuildLinux(buildFlags);
        // _BuildLinux(steamBuildFlags);        //*STEAMWORKS

        // _BuildSwitch(buildFlags);
    }

    // [MenuItem("Build/Development/Build All", false, -14)]
    // public static void DevBuildAll()
    // {
    //     BuildFlags buildFlags = BuildFlags.devMode;
    //     BuildFlags steamBuildFlags = buildFlags | BuildFlags.steamworks;

    //     _BuildWindows(buildFlags);
    //     _BuildWindows(steamBuildFlags);        //*STEAMWORKS

    //     _BuildOSX(buildFlags);
    //     _BuildOSX(steamBuildFlags);        //*STEAMWORKS

    //     _BuildLinux(buildFlags);
    //     _BuildLinux(steamBuildFlags);        //*STEAMWORKS

    //     _BuildSwitch(buildFlags);
    // }

    //*https://www.youtube.com/watch?v=WdIG0af7S0g
    static string FindDirectoryArgument(BuildFlags buildFlags)
    {
        string[] args = System.Environment.GetCommandLineArgs();

        //*find:-executeMethod
        //* executeMethod index + 1 = methodName (ie. GameBuilder.BuildAll)
        //* executeMethod index + 2 = Directory
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-executeMethod")
            {
                //*Make sure we don't leave array bounds
                if (i + 2 >= args.Length)
                    break;

                //* if the potential directory starts with a dash then we should avoid it, as it's a different command argument
                if (args[i + 2][0] == '-')
                    break;

                Debug.Log("CHOSEN PATH: " + args[i + 2]);
                return args[i + 2];
            }
        }

        //*Default directory
        return (buildFlags.HasFlag(BuildFlags.devMode) ? devPath : buildPath);
    }

    //!If we want this to run in unity editor consider adding 1 to the commit value as it will be out of date when we commit the current version
    //![InitializeOnLoad]
    //![MenuItem("Build/Update Application Version", false, 100)]
    public static void UpdateApplicationVersion()
    {
        PlayerSettings.bundleVersion = Git.BuildVersion;
        Debug.Log("Updated to " + Git.BuildVersion);
    }

    [MenuItem("Build/Release/Windows", false, 0)] public static void BuildWindows() => _BuildWindows();
    [MenuItem("Build/Release/OSX", false, 1)] public static void BuildOSX() => _BuildOSX();
    [MenuItem("Build/Release/Linux", false, 2)] public static void BuildLinux() => _BuildLinux();
    // [MenuItem("Build/Release/Switch", false, 3)] public static void BuildSwitch() => _BuildSwitch();

    // [MenuItem("Build/Release/Steam Windows", false, 20)] public static void BuildSteamWindows() => _BuildWindows(BuildFlags.steamworks);
    // [MenuItem("Build/Release/Steam OSX", false, 21)] public static void BuildSteamOSX() => _BuildOSX(BuildFlags.steamworks);
    // [MenuItem("Build/Release/Steam Linux", false, 22)] public static void BuildSteamLinux() => _BuildLinux(BuildFlags.steamworks);

    // [MenuItem("Build/Development/Windows", false, 4)] public static void DevBuildWindows() => _BuildWindows(BuildFlags.devMode);
    // [MenuItem("Build/Development/OSX", false, 5)] public static void DevBuildOSX() => _BuildOSX(BuildFlags.devMode);
    // [MenuItem("Build/Development/Linux", false, 6)] public static void DevBuildLinux() => _BuildLinux(BuildFlags.devMode);
    // [MenuItem("Build/Development/Switch", false, 7)] public static void DevBuildSwitch() => _BuildSwitch(BuildFlags.devMode);

    // [MenuItem("Build/Development/Steam Windows", false, 20)] public static void DevBuildSteamWindows() => _BuildWindows(BuildFlags.devMode | BuildFlags.steamworks);
    // [MenuItem("Build/Development/Steam OSX", false, 21)] public static void DevBuildSteamOSX() => _BuildOSX(BuildFlags.devMode | BuildFlags.steamworks);
    // [MenuItem("Build/Development/Steam Linux", false, 22)] public static void DevBuildSteamLinux() => _BuildLinux(BuildFlags.devMode | BuildFlags.steamworks);


    public static string BuildFolderName(string platform, BuildFlags buildFlags)
    {
        PlayerSettings.bundleVersion = Git.BuildVersion;
        string releaseChannel = buildFlags.HasFlag(BuildFlags.devMode) ? "dev" : "release";

        //*Add steamtag to platform if flag exists
        //*Need to use underscore as dashes are parsed through build server to separate variables (and we want "steam" to be part of the platform name)
        if (buildFlags.HasFlag(BuildFlags.steamworks))
            platform += "_steam";

        return string.Format("/{0}-{1}-{2}-{3}/", Application.productName, platform, releaseChannel, Application.version.Replace('.', '_')).ToLower();
    }

    static void SetTargetBuildPreDefines(NamedBuildTarget namedTarget, BuildFlags buildFlags)
    {
        List<string> preDefines = new List<string>();

        //*Steamworks
        if (buildFlags.HasFlag(BuildFlags.steamworks))
            preDefines.Add("STEAMWORKS_NET");
        else
            preDefines.Add("DISABLESTEAMWORKS");

        PlayerSettings.SetScriptingDefineSymbols(namedTarget, preDefines.ToArray());
    }

    static void _BuildWindows(BuildFlags buildFlags = BuildFlags.none) => _BuildWindows(FindDirectoryArgument(buildFlags), buildFlags);
    static void _BuildWindows(string path, BuildFlags buildFlags = BuildFlags.none)
    {
        Debug.Log("Building for Windows");
        path += BuildFolderName("windows", buildFlags);//"/windows/" + Application.productName + "/";
        string executableName = Application.productName + ".exe";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
        buildPlayerOptions.locationPathName = path + executableName;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.targetGroup = BuildPipeline.GetBuildTargetGroup(buildPlayerOptions.target);
        buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Player;

        RunExport(buildPlayerOptions, buildFlags);

    }

    static void _BuildOSX(BuildFlags buildFlags = BuildFlags.none) => _BuildOSX(FindDirectoryArgument(buildFlags), buildFlags);
    static void _BuildOSX(string path, BuildFlags buildFlags = BuildFlags.none)
    {
        Debug.Log("Building for OSX");
        path += BuildFolderName("osx", buildFlags);//"/osx/";
        string executableName = Application.productName + ".app";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
        buildPlayerOptions.locationPathName = path + executableName;
        buildPlayerOptions.target = BuildTarget.StandaloneOSX;
        buildPlayerOptions.targetGroup = BuildPipeline.GetBuildTargetGroup(buildPlayerOptions.target);
        buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Player;

        RunExport(buildPlayerOptions, buildFlags);
    }

    static void _BuildLinux(BuildFlags buildFlags = BuildFlags.none) => _BuildLinux(FindDirectoryArgument(buildFlags), buildFlags);
    static void _BuildLinux(string path, BuildFlags buildFlags = BuildFlags.none)
    {
        Debug.Log("Building for Linux");
        path += BuildFolderName("linux", buildFlags);
        string executableName = Application.productName + ".x64";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
        buildPlayerOptions.locationPathName = path + executableName;
        buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
        buildPlayerOptions.targetGroup = BuildPipeline.GetBuildTargetGroup(buildPlayerOptions.target);
        buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Player;

        RunExport(buildPlayerOptions, buildFlags);
    }

    // static void _BuildSwitch(BuildFlags buildFlags = BuildFlags.none) => _BuildSwitch(FindDirectoryArgument(buildFlags), buildFlags);
    // static void _BuildSwitch(string path, BuildFlags buildFlags = BuildFlags.none)
    // {
    //         Debug.Log("Building for Switch");
    //     path += BuildFolderName("switch", buildFlags);
    //     string executableName = Application.productName + ".nsp";

    //     if (!Directory.Exists(path))
    //     {
    //         Directory.CreateDirectory(path);
    //     }

    //     BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
    //     buildPlayerOptions.scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
    //     buildPlayerOptions.locationPathName = path + executableName;
    //     buildPlayerOptions.target = BuildTarget.Switch;
    //     buildPlayerOptions.targetGroup = BuildPipeline.GetBuildTargetGroup(buildPlayerOptions.target);
    //     buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Player;

    //     EditorUserBuildSettings.switchCreateRomFile = true;

    //     RunExport(buildPlayerOptions, buildFlags);
    // }

    static void RunExport(BuildPlayerOptions buildPlayerOptions, BuildFlags buildFlags)
    {
        PlayerSettings.bundleVersion = Git.BuildVersion;
        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(buildPlayerOptions.target), buildPlayerOptions.target))
        {
            throw new System.Exception("Build failed to as not able to switch to build target " + buildPlayerOptions.target + ".");
        }
        EditorUserBuildSettings.selectedBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildPlayerOptions.target);
        EditorUserBuildSettings.standaloneBuildSubtarget = StandaloneBuildSubtarget.Player;
        EditorUserBuildSettings.selectedStandaloneTarget = buildPlayerOptions.target;

        if (buildFlags.HasFlag(BuildFlags.devMode))
        {
            buildPlayerOptions.options = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler;
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.connectProfiler = true;
            EditorUserBuildSettings.buildWithDeepProfilingSupport = true;
        }
        else
        {
            buildPlayerOptions.options = BuildOptions.None;
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.connectProfiler = false;
            EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
        }

        // if (!PreExport())
        // {
        //     throw new System.Exception("Build failed to start due to Addressables Error.");
        // }

        NamedBuildTarget namedTarget = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(buildPlayerOptions.target));

        //*Set build predefines for the target platform
        string originalDefines = PlayerSettings.GetScriptingDefineSymbols(namedTarget);
        SetTargetBuildPreDefines(namedTarget, buildFlags);

        Debug.Log("STARTING BUILD - v" + PlayerSettings.bundleVersion + " | " + buildPlayerOptions.locationPathName);
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("BuildSucceded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }

        //*Reset our player settings define symbols back to how they were!
        PlayerSettings.SetScriptingDefineSymbols(namedTarget, originalDefines);
    }


    //!Not needed if Build Addressables on Player build
    //? See Preferences > Addressables > "Build Addressables on build Player is allowed"
    //? Also see Addressable Asset Settings objects

    // #region Addressables

    // public static string build_script
    //             = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";

    // public static string settings_asset
    //     = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";

    // public static string profile_name = "Default";
    // private static AddressableAssetSettings settings;


    // static void getSettingsObject(string settingsAsset)
    // {
    //     // This step is optional, you can also use the default settings:
    //     //settings = AddressableAssetSettingsDefaultObject.Settings;

    //     settings
    //         = AssetDatabase.LoadAssetAtPath<ScriptableObject>(settingsAsset)
    //             as AddressableAssetSettings;

    //     if (settings == null)
    //         Debug.LogError($"{settingsAsset} couldn't be found or isn't " +
    //                        $"a settings object.");
    // }



    // static void setProfile(string profile)
    // {
    //     string profileId = settings.profileSettings.GetProfileId(profile);
    //     if (string.IsNullOrEmpty(profileId))
    //         Debug.LogWarning($"Couldn't find a profile named, {profile}, " +
    //                          $"using current profile instead.");
    //     else
    //         settings.activeProfileId = profileId;
    // }



    // static void setBuilder(IDataBuilder builder)
    // {
    //     int index = settings.DataBuilders.IndexOf((ScriptableObject)builder);

    //     if (index > 0)
    //         settings.ActivePlayerDataBuilderIndex = index;
    //     else
    //         Debug.LogWarning($"{builder} must be added to the " +
    //                          $"DataBuilders list before it can be made " +
    //                          $"active. Using last run builder instead.");
    // }



    // static bool buildAddressableContent()
    // {
    //     AddressableAssetSettings
    //         .BuildPlayerContent(out AddressablesPlayerBuildResult result);
    //     bool success = string.IsNullOrEmpty(result.Error);

    //     if (!success)
    //     {
    //         Debug.LogError("Addressables build error encountered: " + result.Error);
    //     }

    //     return success;
    // }


    // [MenuItem("Window/Asset Management/Addressables/Build Addressables only")]
    // public static bool BuildAddressables()
    // {
    //     getSettingsObject(settings_asset);
    //     setProfile(profile_name);
    //     IDataBuilder builderScript
    //         = AssetDatabase.LoadAssetAtPath<ScriptableObject>(build_script) as IDataBuilder;

    //     if (builderScript == null)
    //     {
    //         Debug.LogError(build_script + " couldn't be found or isn't a build script.");
    //         return false;
    //     }

    //     setBuilder(builderScript);

    //     return buildAddressableContent();
    // }

    // #endregion
}
