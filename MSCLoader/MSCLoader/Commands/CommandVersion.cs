﻿#if !Mini
using System;
using System.Reflection;
using Steamworks;

namespace MSCLoader.Commands;

internal class CommandVersion : ConsoleCommand
{
    public override string Name => "ver";
    public override string Alias => "version";

    public override string Help => "Version information";

    public override void Run(string[] args)
    {
        ModConsole.Print($"<color=yellow>Unity:</color> <color=aqua><b>{Application.unityVersion}</b></color>");
        try
        {
            ModConsole.Print(
                $"<color=yellow>MSC buildID:</color> <color=aqua><b>{SteamApps.GetAppBuildId()}</b></color>"); //Get steam buildID
        }
        catch (Exception e)
        {
            ModConsole.Error($"<color=red>Failed to get build ID:</color> <b>{e.Message}</b>"); //Show steamworks error
        }

        ModConsole.Print(
            $"<color=yellow>FreeLoader:</color> <color=aqua><b>{ModLoader.FreeLoader_Ver}</b></color> (build <color=aqua><b>{ModLoader.Instance.currentBuild}</b></color>) []");
        ModConsole.Print(
            $"<color=yellow>Runtime:</color> <color=aqua><b>{Assembly.GetExecutingAssembly().ImageRuntimeVersion}</b></color>");
        ModConsole.Print($"<color=yellow>OS:</color> <color=aqua><b>{ModLoader.SystemInfoFix()}</b></color>");
    }
}
#endif