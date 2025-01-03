﻿#if !Mini
using System.Linq;

namespace MSCLoader.Commands;

internal class MetadataCommand : ConsoleCommand
{
    // What the player has to type into the console to execute your commnad
    public override string Name => "Metadata";

    public override string Alias => "Manifest";

    // The help that's displayed for your command when typing help
    public override string Help => "Command Description";
    public override bool ShowInHelp => false;

    // The function that's called when executing command
    public override void Run(string[] args)
    {
        if (args.Length == 2)
        {
            var mod = ModLoader.LoadedMods.Where(w => w.ID == args[1]).FirstOrDefault();
            var refs = ModLoader.Instance.ReferencesList.Where(w => w.AssemblyID == args[1]).FirstOrDefault();
            switch (args[0].ToLower())
            {
                case "create":
                    if (args[1].Contains(" "))
                    {
                        ModConsole.Error("ModID with spaces are not allowed");
                        return;
                    }

                    if (mod == null)
                        ModConsole.Error("Invalid ModID (ModID is case sensitive)");

                    break;
                case "create_ref":
                    if (refs != null)
                    {
                    }
                    else
                    {
                        ModConsole.Error("Invalid ReferenceID, it's usually your assembly name. (case sensitive)");
                        ModConsole.Warning(
                            "This command is only for references located in <b>References</b> folder, for regular mods use create command.");
                    }

                    break;
                case "update":
                    if (args[1].Contains(" "))
                    {
                        ModConsole.Error("ModID with spaces are not allowed");
                        return;
                    }

                    if (mod == null)
                        ModConsole.Error("Invalid ModID (ModID is case sensitive)");

                    break;
                case "update_ref":
                    if (refs == null)
                    {
                        ModConsole.Error("Invalid ReferenceID, it's usually your assembly name. (case sensitive)");
                        ModConsole.Warning(
                            "This command is only for references located in <b>References</b> folder, for regular mods use create command.");
                    }

                    break;
                case "auth":
                    break;
                default:
                    ModConsole.Warning("Usage: metadata <create|create_ref|update|update_ref> <ModID>");
                    break;
            }
        }
        else
        {
            ModConsole.Warning("Usage: metadata <create|create_ref|update|update_ref> <ModID>");
        }
    }
}
#endif