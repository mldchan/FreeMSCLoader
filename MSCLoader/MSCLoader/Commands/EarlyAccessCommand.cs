#if !Mini
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Random = System.Random;

namespace MSCLoader.Commands;

internal class EarlyAccessCommand : ConsoleCommand
{
    private readonly Random random = new();
    public override string Name => "ea";

    public override string Help => "ea stuff (modders only)";

    public override bool ShowInHelp => false;

    public override void Run(string[] args)
    {
        if (args.Length == 2)
        {
            if (args[0].ToLower() == "create")
            {
                var randomShit = MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(RandomString(16)));
                var s = BitConverter.ToString(randomShit).Replace("-", "");
                try
                {
                    GenerateFile(args[1], s);
                }
                catch (Exception e)
                {
                    ModConsole.Error("Failed with error:");
                    ModConsole.Error(e.Message);
                    Console.WriteLine(e);
                }
            }
            else
            {
                ModConsole.Error("Invalid syntax");
            }
        }
        else if (args.Length == 3)
        {
            if (args[0].ToLower() == "create")
            {
                if (args[2].Contains("|"))
                {
                    ModConsole.Error("Forbidden charater '|' in key");
                    return;
                }

                var s = args[2];
                try
                {
                    GenerateFile(args[1], s);
                }
                catch (Exception e)
                {
                    ModConsole.Error("Failed with error:");
                    ModConsole.Error(e.Message);
                    Console.WriteLine(e);
                }
            }
            else
            {
                ModConsole.Error("Invalid syntax");
            }
        }
        else
        {
            ModConsole.Error("Invalid syntax");
        }
    }

    private string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private void GenerateFile(string file, string key)
    {
        var output = Path.Combine(ModLoader.ModsFolder, "EA_Output");
        if (File.Exists(Path.Combine(ModLoader.ModsFolder, file)))
        {
            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);
            byte[] header = { 0x45, 0x41, 0x4D };
            var modFile = File.ReadAllBytes(Path.Combine(ModLoader.ModsFolder, file));
            var modoutput = modFile.Cry_ScrambleByteRightEnc(Encoding.ASCII.GetBytes(key));
            File.WriteAllBytes(
                Path.Combine(output,
                    $"{Path.GetFileNameWithoutExtension(Path.Combine(ModLoader.ModsFolder, file))}.dll"),
                header.Concat(modoutput).ToArray());
            var txt =
                $"Use this command to register your mod:{Environment.NewLine}{Environment.NewLine}!ea registerfile {Path.GetFileNameWithoutExtension(Path.Combine(ModLoader.ModsFolder, file))} {key}{Environment.NewLine}{Environment.NewLine}If you already registered that file before and want to update key use this:{Environment.NewLine}{Environment.NewLine}!ea setkey {Path.GetFileNameWithoutExtension(Path.Combine(ModLoader.ModsFolder, file))} {key}";
            File.WriteAllText(
                Path.Combine(output,
                    $"{Path.GetFileNameWithoutExtension(Path.Combine(ModLoader.ModsFolder, file))}.txt"), txt);
            ModConsole.Print($"Go to: {Path.GetFullPath(output)}");
        }
        else
        {
            ModConsole.Error("File not found");
        }
    }
}
#endif