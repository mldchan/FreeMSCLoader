﻿using UnityEngine;
using MSCLoader.Commands;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

namespace MSCLoader
{
    /// <summary>
    /// The console for MSCLoader.
    /// </summary>
    public class ModConsole : Mod
    {
#pragma warning disable CS1591

        public override string ID => "MSCLoader_Console";
        public override string Name => "Console";
        public override string Version => ModLoader.Version;
        public override string Author => "piotrulos";

        public override bool UseAssetsFolder => true;
        public override bool LoadInMenu => true;

        public static bool IsOpen { get; private set; }

        public static ConsoleView console;
        private Keybind consoleKey = new Keybind("Open", "Open console", KeyCode.BackQuote);
        GameObject UI;

        //Testing this shish
        Settings testBool = new Settings("Test", "Do some shit", false);
        Settings testBool2 = new Settings("Test2", "Do some shit 2", true);
        Settings testButton = new Settings("button", "reset", testing);
        Settings testButton2 = new Settings("button2", "some text", testing);
        static Settings testSlider = new Settings("slider", "test slider", 14,fontFun);

        public override void ModSettings()
        {
            Settings.AddCheckBox(this, testBool);
            Settings.AddCheckBox(this, testBool2);
            Settings.AddButton(this, testButton2);
            Settings.AddButton(this, testButton, "Do some fancy shit");
            Settings.AddSlider(this, testSlider, 10, 20);
        }

        public static void testing()
        {
            ModConsole.Print("OMG");
        }

        public static void fontFun()
        {
            console.logTextArea.fontSize = int.Parse(testSlider.GetValue().ToString());
        }
        public void CreateConsoleUI()
        {
            AssetBundle ab = LoadAssets.LoadBundle(this, "console.unity3d");
            UI = ab.LoadAsset("MSCLoader Console.prefab") as GameObject;
            Texture2D cursor = ab.LoadAsset("resizeCur.png") as Texture2D;
            ab.Unload(false);
            UI = Object.Instantiate(UI);
            console = UI.AddComponent<ConsoleView>();
            UI.GetComponent<ConsoleView>().viewContainer = UI.transform.GetChild(0).gameObject;
            UI.GetComponent<ConsoleView>().inputField = UI.GetComponent<ConsoleView>().viewContainer.transform.GetChild(0).gameObject.GetComponent<InputField>();
            //UI.GetComponent<ConsoleView>().inputField.onEndEdit.AddListener(delegate { UI.GetComponent<ConsoleView>().runCommand(); });
            UI.GetComponent<ConsoleView>().viewContainer.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(() => UI.GetComponent<ConsoleView>().runCommand());
            UI.GetComponent<ConsoleView>().logTextArea = UI.GetComponent<ConsoleView>().viewContainer.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>();
            UI.GetComponent<ConsoleView>().viewContainer.transform.GetChild(4).gameObject.AddComponent<ConsoleUIResizer>().logview = UI.GetComponent<ConsoleView>().viewContainer.transform.GetChild(2).gameObject;
            UI.GetComponent<ConsoleView>().viewContainer.transform.GetChild(4).gameObject.GetComponent<ConsoleUIResizer>().scrollbar = UI.GetComponent<ConsoleView>().viewContainer.transform.GetChild(3).gameObject;
            UI.GetComponent<ConsoleView>().viewContainer.transform.GetChild(4).gameObject.GetComponent<ConsoleUIResizer>().cursor = cursor;
            EventTrigger trigger = UI.GetComponent<ConsoleView>().viewContainer.transform.GetChild(4).gameObject.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((eventData) => { UI.GetComponent<ConsoleView>().viewContainer.transform.GetChild(4).gameObject.GetComponent<ConsoleUIResizer>().OnMouseEnter(); });
            trigger.delegates.Add(entry);
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerExit;
            entry.callback.AddListener((eventData) => { UI.GetComponent<ConsoleView>().viewContainer.transform.GetChild(4).gameObject.GetComponent<ConsoleUIResizer>().OnMouseExit(); });
            trigger.delegates.Add(entry);

            UI.transform.SetParent(GameObject.Find("MSCLoader Canvas").transform, false);
        }
        
        public override void Update()
        {
            if (consoleKey.IsDown())
            {
                console.toggleVisibility();
            }
            if(Input.GetKeyDown(KeyCode.F6))
            {
                ModConsole.Print(testBool.GetValue());
            }
        }

        public override void OnMenuLoad()
        {
            Keybind.Add(this, consoleKey);
            //Keybind.Add(this, consoleSizeKey);
            CreateConsoleUI();
            console.console = new ConsoleController();
            ConsoleCommand.cc = console.console;
            console.setVisibility(false);
            UI.GetComponent<ConsoleView>().viewContainer.transform.GetChild(4).gameObject.GetComponent<ConsoleUIResizer>().LoadConsoleSize();
            ConsoleCommand.Add(new CommandClear());
            ConsoleCommand.Add(new CommandHelp());
            ConsoleCommand.Add(new CommandLogAll());
        }
#pragma warning restore CS1591
        /// <summary>
        /// Print a message to console.
        /// </summary>
        /// <param name="str">Text to print to console.</param>
        /// <example><code source="Examples.cs" region="ModConsolePrint" lang="C#" 
        /// title="Example Code" /></example>
        public static void Print(string str)
        {
            console.console.appendLogLine(str);
            Debug.Log(string.Format("MSCLoader Message: {0}", Regex.Replace(str, "<.*?>", string.Empty)));
        }
        /// <summary>
        /// OBSOLETE: For compatibility with 0.1 plugins, please use string str overload!
        /// </summary>
        /// <param name="obj">Text or object to append to console.</param>
        /// <example><code source="Examples.cs" region="ModConsolePrint" lang="C#" 
        /// title="Example Code" /></example>
        public static void Print(object obj)
        {
            console.console.appendLogLine(obj.ToString());
            Debug.Log(string.Format("MSCLoader Message: {0}", obj));
        }
        /// <summary>
        /// Print an error to the console.
        /// </summary>
        /// <param name="str">Text to print to error log.</param>
        /// <example><code source="Examples.cs" region="ModConsoleError" lang="C#" 
        /// title="Example Code" /></example>
        public static void Error(string str)
        {
            console.setVisibility(true);
            console.console.appendLogLine(string.Format("<color=red><b>Error: </b>{0}</color>", str));
            Debug.Log(string.Format("MSCLoader ERROR: {0}", Regex.Replace(str, "<.*?>", string.Empty)));
        }

        /// <summary>
        /// Clear the console.
        /// </summary>
        public static void Clear()
        {
            console.console.clearConsole();
        }

    }
}