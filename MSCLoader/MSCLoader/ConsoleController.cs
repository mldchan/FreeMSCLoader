﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MSCLoader;

internal delegate void CommandHandler(string[] args);

internal class ConsoleController
{
    // Used to communicate with ConsoleView
    internal delegate void LogChangedHandler(string[] log);

    internal event LogChangedHandler logChanged;

    // Object to hold information about each command
    internal class CommandRegistration
    {
        public CommandRegistration(string command, CommandHandler handler, string help, bool showInHelp)
        {
            this.command = command;
            this.handler = handler;
            this.help = help;
            this.showInHelp = showInHelp;
        }

        public string command { get; }
        public CommandHandler handler { get; }
        public string help { get; }
        public bool showInHelp { get; }
    }

    // How many log lines should be retained?
    // Note that strings submitted to appendLogLine with embedded newlines will be counted as a single line.
#if DevMode
        const int scrollbackSize = 500;
#else
    private const int scrollbackSize = 200;
#endif
    public Queue<string> scrollback;
    public List<string> commandHistory = new();
    internal Dictionary<string, CommandRegistration> commands = new();

    public string[] log { get; private set; } //Copy of scrollback as an array for easier use by ConsoleView
#if !Mini
    public ConsoleController()
    {
        if (ModLoader.devMode && MSCUnloader.dm_pcon != null)
            scrollback = new Queue<string>(MSCUnloader.dm_pcon);
        else
            scrollback = new Queue<string>(scrollbackSize);
        RegisterCommand("help", HelpCommand, "This screen", "?");
        RegisterCommand("clear", ClearConsole, "Clears the console screen", "cls");
    }

    public void RegisterCommand(string command, CommandHandler handler, string help, bool inHelp = true)
    {
        commands.Add(command, new CommandRegistration(command, handler, help, inHelp));
    }

    public void RegisterCommand(string command, CommandHandler handler, string help, string alias, bool inHelp = true)
    {
        var cmd = new CommandRegistration(command, handler, help, inHelp);
        commands.Add(command, cmd);
        commands.Add(alias, cmd);
    }

    private void ClearConsole(string[] args)
    {
        scrollback.Clear();
        log = scrollback.ToArray();
        logChanged(log);
    }

    public void AppendLogLine(string line)
    {
        if (scrollback.Count >= scrollbackSize) scrollback.Dequeue();
        scrollback.Enqueue(line);

        log = scrollback.ToArray();
        logChanged?.Invoke(log);
    }

    public void RunCommandString(string commandString)
    {
        if (!string.IsNullOrEmpty(commandString))
        {
            AppendLogLine(string.Format("{1}<b><color=orange>></color></b> {0}", commandString, Environment.NewLine));

            var commandSplit = ParseArguments(commandString);
            var args = new string[0];
            if (commandSplit.Length < 1)
            {
                AppendLogLine(string.Format("<color=red>Unable to process command:</color> <b>{0}</b>", commandString));
                return;
            }

            if (commandSplit.Length >= 2)
            {
                var numArgs = commandSplit.Length - 1;
                args = new string[numArgs];
                Array.Copy(commandSplit, 1, args, 0, numArgs);
            }

            RunCommand(commandSplit[0].ToLower(), args);
            commandHistory.Add(commandString);
        }
    }

    private void RunCommand(string command, string[] args)
    {
        if (!string.IsNullOrEmpty(command))
        {
            if (!commands.TryGetValue(command, out var reg))
            {
                AppendLogLine(string.Format(
                    "Unknown command <b><color=red>{0}</color></b> please, type <color=lime><b>help</b></color> for list of all commands.",
                    command));
            }
            else
            {
                if (reg.handler == null)
                    AppendLogLine(string.Format(
                        "<color=red>Unable to process command:</color> <b>{0}</b>, <color=red>handler was null.</color>",
                        command));
                else
                    reg.handler(args);
            }
        }
    }

    private static string[] ParseArguments(string commandString)
    {
        var parmChars = new LinkedList<char>(commandString.ToCharArray());
        var inQuote = false;
        var node = parmChars.First;
        while (node != null)
        {
            var next = node.Next;
            if (node.Value == '"')
            {
                inQuote = !inQuote;
                parmChars.Remove(node);
            }

            if (!inQuote && node.Value == ' ') node.Value = '\n';
            node = next;
        }

        var parmCharsArr = new char[parmChars.Count];
        parmChars.CopyTo(parmCharsArr, 0);
        return new string(parmCharsArr).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private void HelpCommand(string[] args)
    {
        ModConsole.Print("<color=green><b>Available commands:</b></color>");
        var cmds = commands.Values.GroupBy(x => x.command).Select(g => g.First()).Distinct().ToArray();
        for (var i = 0; i < cmds.Length; i++)
            if (cmds[i].showInHelp)
                AppendLogLine(string.Format("<color=orange><b>{0}</b></color>: {1}", cmds[i].command, cmds[i].help));
        if (ModLoader.GetCurrentScene() != CurrentScene.Game)
            AppendLogLine("<b><color=red>More commands may appear after you load a save...</color></b>");
    }
#endif
}