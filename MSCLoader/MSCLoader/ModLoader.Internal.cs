#if !Mini
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MSCLoader;

public partial class ModLoader
{
    internal static bool LogAllErrors = false;
    internal static List<InvalidMods> InvalidMods;
    internal static ModLoader Instance;
    internal static bool unloader = false;
    internal static bool returnToMainMenu = false;
    internal static List<string> saveErrors;
    internal static List<Mod> HasUpdateModList = new();
    internal static List<References> HasUpdateRefList = new();

    internal static string steamID;
    internal static bool loaderPrepared = false;

    internal static string ModsFolder = Path.GetFullPath(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.Combine("MySummerCar", "Mods")));

    internal static string ConfigFolder = Path.Combine(ModsFolder, "Config");
    internal static string SettingsFolder = Path.Combine(ConfigFolder, "Mod Settings");
    internal static string MetadataFolder = Path.Combine(ConfigFolder, "Mod Metadata");
    internal static string AssetsFolder = Path.Combine(ModsFolder, "Assets");
    internal static List<string> ModSelfUpdateList = new();
    internal static List<string> RefSelfUpdateList = new();

    // internal static readonly string serverURL = "http://my-summer-car.ovh"; //Main url NUKE THIS
    // internal static readonly string serverURL2 = "http://my-summer-car.ml"; //Backup secondary url (if first fails)

    internal static readonly string metadataURL = "man.php?v=3&modid=";

    //  internal static readonly string earlyAccessURL = $"mscl_ea.php";
    // internal static readonly string serverURL = "http://localhost/msc2"; //localhost for testing only

    internal Mod[] actualModList = new Mod[0];
    internal bool allModsLoaded = false;
    internal Mod[] BC_ModList = new Mod[0];
    internal List<string> crashedGuids = new();

    internal int currentBuild = Assembly.GetExecutingAssembly().GetName().Version.Revision;
    internal Mod[] FixedUpdateMods = new Mod[0];
    internal GUISkin guiskin;


    internal bool IsModsLoading = false;
    internal bool IsModsResetting = false;
    internal GameObject mainMenuInfo;
    internal Animation menuInfoAnim;

    internal List<string> MetadataUpdateList = new();
    internal Mod[] Mod_FixedUpdate = new Mod[0]; //Calls unity FixedUpdate
    internal Mod[] Mod_OnGUI = new Mod[0]; //Calls unity OnGUI
    internal Mod[] Mod_OnLoad = new Mod[0]; //Phase 2 (mod loading)  

    //New Stuff
    internal Mod[] Mod_OnNewGame = new Mod[0]; //When New Game is started
    internal Mod[] Mod_OnSave = new Mod[0]; //When game saves
    internal Mod[] Mod_PostLoad = new Mod[0]; //Phase 3 (mod loading)
    internal Mod[] Mod_PreLoad = new Mod[0]; //Phase 1 (mod loading)
    internal Mod[] Mod_Update = new Mod[0]; //Calls unity Update
    internal List<string> modIDsReferences = new();
    internal bool ModloaderUpdateMessage = false;
    internal string[] ModsUpdateDir;
    internal MSCUnloader mscUnloader;
    internal int newBuild = 0;
    internal string newVersion = FreeLoader_Ver;
    internal Mod[] OnGUImods = new Mod[0];
    internal Mod[] OnSaveMods = new Mod[0];

    //Old stuff
    internal Mod[] PLoadMods = new Mod[0];
    internal List<References> ReferencesList = new();
    internal string[] RefsUpdateDir;
    internal Mod[] SecondPassMods = new Mod[0];

    internal string[] stdRef = new[]
    {
        "mscorlib", "System.Core", "UnityEngine", "PlayMaker", "MSCLoader", "System", "Assembly-CSharp",
        "Assembly-CSharp-firstpass", "Assembly-UnityScript", "Assembly-UnityScript-firstpass", "ES2", "Ionic.Zip",
        "UnityEngine.UI", "0Harmony", "cInput", "Newtonsoft.Json", "System.Xml"
    };

    internal Mod[] UpdateMods = new Mod[0];
}

#endif