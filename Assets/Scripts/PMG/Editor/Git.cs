/* MIT License
Copyright (c) 2016 RedBlueGames
Code written by Doug Cox
*/

using System;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

/// <summary>
/// GitException includes the error output from a Git.Run() command as well as the
/// ExitCode it returned.
/// </summary>
public class GitException : InvalidOperationException
{
    public GitException(int exitCode, string errors) : base(errors) =>
        this.ExitCode = exitCode;

    /// <summary>
    /// The exit code returned when running the Git command.
    /// </summary>
    public readonly int ExitCode;
}

public static class Git
{
    /* Properties ============================================================================================================= */

    /// <summary>
    /// Retrieves the build version from git based on the most recent matching tag and
    /// commit history. This returns the version as: {major.minor.build} where 'build'
    /// represents the nth commit after the tagged commit.
    /// Note: The initial 'v' and the commit hash code are removed.
    /// </summary>
    public static string BuildVersion
    {
        get
        {
            var version = Run(@"describe --tags --abbrev=0 --match ""v[0-9]*");
            //*Count relevent commits to append to the end of the version (avoid any merges within this count)
            var commits = Run(@"rev-list --no-merges " + version + "..HEAD --count");
            //*Use substring to remove initial 'v'
            version = version.Substring(1) + '.' + commits;
            return version;
        }
    }

    //!Github desktop for some reason doesn't properly push these commits properly, for now we'll have to manually tag these before build
    // public static void TagCommitAsMilestone()
    // {
    //     string ver = Git.BuildVersion;
    //     string[] splitVer = ver.Split('.');

    //     if (splitVer[2] == "0")
    //     {
    //         Debug.Log("Current commit - v" + ver + " - is already a milestone!");
    //     }
    //     else
    //     {
    //         if (int.TryParse(splitVer[1], out int lastMilestone))
    //         {
    //             string tag = string.Format("v{0}.{1}", splitVer[0], lastMilestone + 1);
    //             Run(@"tag " + tag);
    //             //* Tries to push the tag to github immediately
    //             //Run(@"push origin " + tag);
    //             Debug.Log("Tagged as milestone. Old: v" + ver + ", New: v" + Git.BuildVersion);
    //         }
    //         else
    //             Debug.LogError("Invalid version name. Cannot properly convert v" + ver);
    //     }
    // }

    // public static void RevertTagCommitAsMilestone()
    // {
    //     string ver = Git.BuildVersion;
    //     string[] splitVer = ver.Split('.');

    //     if (splitVer[2] == "0")
    //     {
    //         string tag = string.Format("v{0}.{1}", splitVer[0], splitVer[1]);
    //         Run(@"tag -d " + tag);
    //         //* Tries to remove the relevant tag from github
    //         //if (Run(@"ls-remote origin " + tag) == Run(@"rev-parse HEAD"))
    //         //Run(@"push --delete origin " + tag);

    //         Debug.Log("Reverted commit from milestone. Old: v" + ver + ", New: v" + Git.BuildVersion);
    //     }
    //     else
    //     {
    //         Debug.Log("Current commit - v" + ver + " - is not a milestone!");
    //     }
    // }

    /// <summary>
    /// The currently active branch.
    /// </summary>
    public static string Branch => Run(@"rev-parse --abbrev-ref HEAD");

    /// <summary>
    /// Returns a listing of all uncommitted or untracked (added) files.
    /// </summary>
    public static string Status => Run(@"status --porcelain");


    /* Methods ================================================================================================================ */

    /// <summary>
    /// Runs git.exe with the specified arguments and returns the output.
    /// </summary>
    public static string Run(string arguments)
    {
        using (var process = new Process())
        {
            var exitCode = process.Run(@"git", arguments, Application.dataPath,
                out var output, out var errors);
            if (exitCode == 0)
            {
                return output;
            }
            else
            {
                throw new GitException(exitCode, errors);
            }
        }
    }
}
