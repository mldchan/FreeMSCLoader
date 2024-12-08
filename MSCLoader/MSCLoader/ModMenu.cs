#if !Mini
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using FreeLoader;
using IniParser;
using Newtonsoft.Json;

namespace MSCLoader;

internal class ModMenu : Mod
{
    internal static byte cfmu_set;
    internal static ModMenu instance;
    internal static SettingsCheckBox dm_disabler, dm_logST, dm_operr, dm_warn, dm_pcon;

    internal static SettingsCheckBox expWarning,
        modPath,
        forceMenuVsync,
        openLinksOverlay,
        skipGameIntro,
        skipConfigScreen;
    
    private FileVersionInfo coreVer;

    internal GameObject UI;
    public override string ID => "FreeLoader_Settings";
    public override string Name => "[INTERNAL] Mod Menu";
    public override string Version => ModLoader.FreeLoader_Ver;
    public override string Author => "piotrulos";

    public override void ModSetup()
    {
        SetupFunction(Setup.OnMenuLoad, Mod_OnMenuLoad);
        SetupFunction(Setup.ModSettings, Mod_Settings);
        SetupFunction(Setup.ModSettingsLoaded, Mod_SettingsLoaded);
    }

    private void Mod_Settings()
    {
        instance = this;
        Settings.ModSettings(this);
        if (ModLoader.devMode)
        {
            Settings.AddHeader(this, "DevMode Settings", new Color32(0, 0, 128, 255), Color.green);
            dm_disabler = Settings.AddCheckBox(this, "FreeLoader_dm_disabler", "Disable mods throwing errors");
            dm_logST = Settings.AddCheckBox(this, "FreeLoader_dm_logST", "Log-all stack trace (not recommended)");
            dm_operr = Settings.AddCheckBox(this, "FreeLoader_dm_operr", "Log-all open console on error");
            dm_warn = Settings.AddCheckBox(this, "FreeLoader_dm_warn", "Log-all open console on warning");
            dm_pcon = Settings.AddCheckBox(this, "FreeLoader_dm_pcon", "Persistent console (sometimes may break font)");
        }

        Settings.AddHeader(this, "Basic Settings");
        expWarning = Settings.AddCheckBox(this, "FreeLoader_expWarning",
            "Show experimental warning (experimental branch on Steam)", true);
        modPath = Settings.AddCheckBox(this, "FreeLoader_modPath", "Show mods folder (top left corner)", true,
            ModLoader.MainMenuPath);
        forceMenuVsync = Settings.AddCheckBox(this, "FreeLoader_forceMenuVsync", "60 FPS limit in Main Menu", true,
            VSyncSwitchCheckbox);
        openLinksOverlay = Settings.AddCheckBox(this, "FreeLoader_openLinksOverlay", "Open URLs in Steam overlay", true);
        Settings.AddText(this, "Skip stuff:");
        skipGameIntro = Settings.AddCheckBox(this, "FreeLoader_skipGameIntro", "Skip game splash screen", false,
            SkipIntroSet);
        skipConfigScreen = Settings.AddCheckBox(this, "FreeLoader_skipConfigScreen", "Skip configuration screen", false,
            SkipConfigScreen);

        Settings.AddHeader(this, "FreeLoader Credits", Color.black);
        Settings.AddText(this, "All source code contributors and used libraries are listed on GitHub");
        Settings.AddText(this,
            "<color=pink>Freedom by mldchan!</color> <color=cyan>tr</color><color=#ff77ff>an</color>s r<color=#ff77ff>ig</color><color=cyan>hts</color>");
        Settings.AddText(this, "Non-GitHub contributions:");
        Settings.AddText(this,
            "<color=aqua>BrennFuchS</color> - New default mod icon and expanded PlayMaker extensions.");

        Settings.AddHeader(this, "Detailed Version Information", new Color32(0, 128, 0, 255));
        coreVer = FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"),
            "FreeLoader.Preloader.dll"));
        Settings.AddText(this,
            $"FreeLoader modules:{Environment.NewLine}<color=yellow>FreeLoader.Preloader</color>: <color=aqua>v{coreVer.FileMajorPart}.{coreVer.FileMinorPart}.{coreVer.FileBuildPart} build {coreVer.FilePrivatePart}</color>{Environment.NewLine}<color=yellow>FreeLoader</color>: <color=aqua>v{ModLoader.FreeLoader_Ver} build {ModLoader.Instance.currentBuild}</color>");
        Settings.AddText(this,
            $"Build-in libraries:{Environment.NewLine}<color=yellow>Harmony</color>: <color=aqua>v{FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "0Harmony.dll")).FileVersion}</color>{Environment.NewLine}" +
            $"<color=yellow>Ionic.Zip</color>: <color=aqua>v{FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "Ionic.Zip.dll")).FileVersion}</color>{Environment.NewLine}" +
            $"<color=yellow>NAudio</color>: <color=aqua>v{FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "NAudio.dll")).FileVersion}</color>{Environment.NewLine}" +
            $"<color=yellow>NAudio (Vorbis)</color>: <color=aqua>v{FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "NVorbis.dll")).FileVersion}</color>{Environment.NewLine}" +
            $"<color=yellow>NAudio (Flac)</color>: <color=aqua>v{FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "NAudio.Flac.dll")).FileVersion}</color>{Environment.NewLine}" +
            $"<color=yellow>Newtonsoft.Json</color>: <color=aqua>v{FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "Newtonsoft.Json.dll")).FileVersion}</color>{Environment.NewLine}" +
            $"<color=yellow>INIFileParser</color>: <color=aqua>v{FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "INIFileParser.dll")).FileVersion}</color>");
    }

    private void Mod_SettingsLoaded()
    {
        var ini = new FileIniDataParser().ReadFile("doorstop_config.ini");
        var skipIntro = ini["FreeLoader"]["skipIntro"];
        var skipCfg = ini["FreeLoader"]["skipConfigScreen"];
        bool introSkip, configSkip;
        if (!bool.TryParse(skipIntro, out introSkip))
        {
            skipGameIntro.SetValue(false);
            Console.WriteLine($"skipIntro - Excepted boolean, received '{skipIntro ?? "<null>"}'.");
        }
        else
        {
            skipGameIntro.SetValue(introSkip);
        }

        if (!bool.TryParse(skipCfg, out configSkip))
        {
            skipConfigScreen.SetValue(false);
            Console.WriteLine($"skipConfigScreen - Excepted boolean, received '{skipCfg ?? "<null>"}'.");
        }
        else
        {
            skipConfigScreen.SetValue(configSkip);
        }

        cfmu_set = 7;
    }

    private void SkipIntroSet()
    {
        var parser = new FileIniDataParser();
        var ini = parser.ReadFile("doorstop_config.ini");
        ini["FreeLoader"]["skipIntro"] = skipGameIntro.GetValue().ToString().ToLower();
        parser.WriteFile("doorstop_config.ini", ini, Encoding.ASCII);
    }

    private void SkipConfigScreen()
    {
        if (coreVer.FilePrivatePart < 263)
        {
            ModUI.ShowMessage(
                "To use <color=yellow>Skip Configuration Screen</color> you need to update the core module of FreeLoader, download the latest version and launch <color=aqua>MSCPatcher.exe</color> to update",
                "Outdated module");
            return;
        }

        var parser = new FileIniDataParser();
        var ini = parser.ReadFile("doorstop_config.ini");
        ini["FreeLoader"]["skipConfigScreen"] = skipConfigScreen.GetValue().ToString().ToLower();
        parser.WriteFile("doorstop_config.ini", ini, Encoding.ASCII);
    }

    private static void VSyncSwitchCheckbox()
    {
        if (ModLoader.GetCurrentScene() == CurrentScene.MainMenu)
        {
            if (forceMenuVsync.GetValue())
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;
        }
    }

    private void Mod_OnMenuLoad()
    {
        try
        {
            CreateSettingsUI();
        }
        catch (Exception e)
        {
            ModUI.ShowMessage(
                $"Fatal error:{Environment.NewLine}<color=orange>{e.Message}</color>{Environment.NewLine}Please install FreeLoader correctly.",
                "Fatal Error");
        }
    }

    public void CreateSettingsUI()
    {
        var ab = LoadAssets.LoadBundle(this, "settingsui.unity3d");
        var UIp = ab.LoadAsset<GameObject>("FreeLoader Canvas menu.prefab");
        UI = GameObject.Instantiate(UIp);
        GameObject.DontDestroyOnLoad(UI);
        GameObject.Destroy(UIp);
        ab.Unload(false);
    }

    // Reset keybinds
    public static void ResetBinds(Mod mod)
    {
        if (mod != null)
        {
            // Revert binds
            var bind = Keybind.Get(mod).ToArray();
            for (var i = 0; i < bind.Length; i++)
            {
                var original = Keybind.GetDefault(mod).Find(x => x.ID == bind[i].ID);

                if (original != null)
                {
                    bind[i].Key = original.Key;
                    bind[i].Modifier = original.Modifier;
                }
            }

            // Save binds
            SaveModBinds(mod);
        }
    }


    // Save all keybinds to config files.
    public static void SaveAllBinds()
    {
        for (var i = 0; i < ModLoader.LoadedMods.Count; i++) SaveModBinds(ModLoader.LoadedMods[i]);
    }


    // Save keybind for a single mod to config file.
    public static void SaveModBinds(Mod mod)
    {
        var list = new KeybindList();
        var path = Path.Combine(ModLoader.GetModSettingsFolder(mod), "keybinds.json");

        var binds = Keybind.Get(mod).ToArray();
        for (var i = 0; i < binds.Length; i++)
        {
            if (binds[i].ID == null || binds[i].Vals != null)
                continue;
            var keybinds = new Keybinds
            {
                ID = binds[i].ID,
                Key = binds[i].Key,
                Modifier = binds[i].Modifier
            };

            list.keybinds.Add(keybinds);
        }

        var serializedData = JsonConvert.SerializeObject(list, Formatting.Indented);
        File.WriteAllText(path, serializedData);
    }

    // Reset settings
    public static void ResetSettings(Mod mod)
    {
        if (mod == null) return;
        for (var i = 0; i < Settings.Get(mod).Count; i++) ResetSpecificSetting(Settings.Get(mod)[i]);
        SaveSettings(mod);
    }

    internal static void ResetSpecificSetting(ModSetting set)
    {
        switch (set.SettingType)
        {
            case SettingsType.CheckBoxGroup:
                var scbg = (SettingsCheckBoxGroup)set;
                scbg.Value = scbg.DefaultValue;
                scbg.IsVisible = scbg.DefaultVisibility;
                break;
            case SettingsType.CheckBox:
                var scb = (SettingsCheckBox)set;
                scb.Value = scb.DefaultValue;
                scb.IsVisible = scb.DefaultVisibility;
                break;
            case SettingsType.Slider:
                var ss = (SettingsSlider)set;
                ss.Value = ss.DefaultValue;
                ss.IsVisible = ss.DefaultVisibility;
                break;
            case SettingsType.SliderInt:
                var ssi = (SettingsSliderInt)set;
                ssi.Value = ssi.DefaultValue;
                ssi.IsVisible = ssi.DefaultVisibility;
                break;
            case SettingsType.TextBox:
                var stb = (SettingsTextBox)set;
                stb.Value = stb.DefaultValue;
                stb.IsVisible = stb.DefaultVisibility;
                break;
            case SettingsType.DropDown:
                var sddl = (SettingsDropDownList)set;
                sddl.Value = sddl.DefaultValue;
                sddl.IsVisible = sddl.DefaultVisibility;
                break;
            case SettingsType.ColorPicker:
                var scp = (SettingsColorPicker)set;
                scp.Value = scp.DefaultColorValue;
                scp.IsVisible = scp.DefaultVisibility;
                break;
        }
    }

    // Save settings for a single mod to config file.
    internal static void SaveSettings(Mod mod)
    {
        var list = new SettingsList();
        list.isDisabled = mod.isDisabled;
        var path = Path.Combine(ModLoader.GetModSettingsFolder(mod), "settings.json");

        for (var i = 0; i < Settings.Get(mod).Count; i++)
            switch (Settings.Get(mod)[i].SettingType)
            {
                case SettingsType.Button:
                case SettingsType.RButton:
                case SettingsType.Header:
                case SettingsType.Text:
                    continue;
                case SettingsType.CheckBoxGroup:
                    var group = (SettingsCheckBoxGroup)Settings.Get(mod)[i];
                    list.settings.Add(new Setting(group.ID, group.Value));
                    break;
                case SettingsType.CheckBox:
                    var check = (SettingsCheckBox)Settings.Get(mod)[i];
                    list.settings.Add(new Setting(check.ID, check.Value));
                    break;
                case SettingsType.Slider:
                    var slider = (SettingsSlider)Settings.Get(mod)[i];
                    list.settings.Add(new Setting(slider.ID, slider.Value));
                    break;
                case SettingsType.SliderInt:
                    var sliderInt = (SettingsSliderInt)Settings.Get(mod)[i];
                    list.settings.Add(new Setting(sliderInt.ID, sliderInt.Value));
                    break;
                case SettingsType.TextBox:
                    var textBox = (SettingsTextBox)Settings.Get(mod)[i];
                    list.settings.Add(new Setting(textBox.ID, textBox.Value));
                    break;
                case SettingsType.DropDown:
                    var dropDown = (SettingsDropDownList)Settings.Get(mod)[i];
                    list.settings.Add(new Setting(dropDown.ID, dropDown.Value));
                    break;
                case SettingsType.ColorPicker:
                    var colorPicker = (SettingsColorPicker)Settings.Get(mod)[i];
                    list.settings.Add(new Setting(colorPicker.ID, colorPicker.Value));
                    break;
            }

        var serializedData = JsonConvert.SerializeObject(list, Formatting.Indented);
        File.WriteAllText(path, serializedData);
    }

    // Load all keybinds.
    public static void LoadBinds()
    {
        var binds = ModLoader.LoadedMods.Where(mod => Keybind.Get(mod).Count > 0).ToArray();
        for (var i = 0; i < binds.Length; i++)
        {
            // Check if there is custom keybinds file (if not, create)
            var path = Path.Combine(ModLoader.GetModSettingsFolder(binds[i]), "keybinds.json");
            if (!File.Exists(path))
            {
                SaveModBinds(binds[i]);
                continue;
            }

            //Load and deserialize 
            var keybinds = JsonConvert.DeserializeObject<KeybindList>(File.ReadAllText(path));
            if (keybinds.keybinds.Count == 0)
                continue;
            for (var k = 0; k < keybinds.keybinds.Count; k++)
            {
                var bind = binds[i].Keybinds.Find(x => x.ID == keybinds.keybinds[k].ID);
                if (bind == null)
                    continue;
                bind.Key = keybinds.keybinds[k].Key;
                bind.Modifier = keybinds.keybinds[k].Modifier;
            }
        }
    }

    // Load all settings.
    internal static void LoadSettings()
    {
        for (var i = 0; i < ModLoader.LoadedMods.Count; i++)
        {
            // Check if there is custom settings file (if not, ignore)
            var path = Path.Combine(ModLoader.GetModSettingsFolder(ModLoader.LoadedMods[i]), "settings.json");
            if (!File.Exists(path))
                SaveSettings(ModLoader.LoadedMods[i]); //create settings file if not exists.
            //Load and deserialize 
            var settings = JsonConvert.DeserializeObject<SettingsList>(File.ReadAllText(path));
            ModLoader.LoadedMods[i].isDisabled = settings.isDisabled;
            if (!ModLoader.LoadedMods[i].isDisabled)
                try
                {
                    if (ModLoader.LoadedMods[i].newFormat && ModLoader.LoadedMods[i].fileName != null)
                    {
                        if (ModLoader.LoadedMods[i].A_OnMenuLoad != null)
                        {
                            ModLoader.LoadedMods[i].A_OnMenuLoad.Invoke();
                            ModLoader.LoadedMods[i].disableWarn = true;
                        }
                    }
                    else
                    {
                        if (ModLoader.LoadedMods[i].LoadInMenu && ModLoader.LoadedMods[i].fileName != null)
                        {
                            ModLoader.LoadedMods[i].OnMenuLoad();
                            ModLoader.LoadedMods[i].disableWarn = true;
                        }

                        if (ModLoader.CheckEmptyMethod(ModLoader.LoadedMods[i], "MenuOnLoad"))
                        {
                            ModLoader.LoadedMods[i].MenuOnLoad();
                            ModLoader.LoadedMods[i].disableWarn = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    ModLoader.ModException(e, ModLoader.LoadedMods[i]);
                }

            if (Settings.Get(ModLoader.LoadedMods[i]).Count == 0)
                continue;

            for (var j = 0; j < settings.settings.Count; j++)
            {
                var ms = Settings.Get(ModLoader.LoadedMods[i]).Find(x => x.ID == settings.settings[j].ID);
                if (ms == null)
                    continue;
                switch (ms.SettingType)
                {
                    case SettingsType.Button:
                    case SettingsType.RButton:
                    case SettingsType.Header:
                    case SettingsType.Text:
                        continue;
                    case SettingsType.CheckBoxGroup:
                        var group = (SettingsCheckBoxGroup)ms;
                        group.SetValue(bool.Parse(settings.settings[j].Value.ToString()));
                        break;
                    case SettingsType.CheckBox:
                        var check = (SettingsCheckBox)ms;
                        check.SetValue(bool.Parse(settings.settings[j].Value.ToString()));
                        break;
                    case SettingsType.Slider:
                        var slider = (SettingsSlider)ms;
                        slider.SetValue(float.Parse(settings.settings[j].Value.ToString()));
                        break;
                    case SettingsType.SliderInt:
                        var sliderInt = (SettingsSliderInt)ms;
                        sliderInt.SetValue(int.Parse(settings.settings[j].Value.ToString()));
                        break;
                    case SettingsType.TextBox:
                        var textBox = (SettingsTextBox)ms;
                        textBox.SetValue(settings.settings[j].Value.ToString());
                        break;
                    case SettingsType.DropDown:
                        var dropDown = (SettingsDropDownList)ms;
                        dropDown.SetSelectedItemIndex(int.Parse(settings.settings[j].Value.ToString()));
                        break;
                    case SettingsType.ColorPicker:
                        var colorPicker = (SettingsColorPicker)ms;
                        colorPicker.Value = settings.settings[j].Value.ToString();
                        break;
                }
            }

            try
            {
                if (!ModLoader.LoadedMods[i].isDisabled)
                {
                    if (ModLoader.LoadedMods[i].newSettingsFormat)
                    {
                        if (ModLoader.LoadedMods[i].A_ModSettingsLoaded != null)
                            ModLoader.LoadedMods[i].A_ModSettingsLoaded.Invoke();
                    }
                    else
                    {
                        ModLoader.LoadedMods[i].ModSettingsLoaded();
                    }
                }
            }
            catch (Exception e)
            {
                ModLoader.ModException(e, ModLoader.LoadedMods[i]);
            }
        }
    }

    internal static void ModMenuHandle()
    {
        GameObject.Find("Systems").transform.Find("OptionsMenu").gameObject.AddComponent<ModMenuHandler>().modMenuUI =
            instance.UI.transform.GetChild(0).gameObject;
        instance.UI.transform.GetChild(0).gameObject.SetActive(false);
    }

    public class ModMenuHandler : MonoBehaviour
    {
        public GameObject modMenuUI;
        private bool isApplicationQuitting;

        private void OnEnable()
        {
            modMenuUI.SetActive(true);
            //    StartCoroutine(CursorPM());
        }

        private void OnDisable()
        {
            if (isApplicationQuitting) return;
            modMenuUI.SetActive(false);
            if (ListStuff.settingsOpened)
            {
                SaveSettings(ModLoader.LoadedMods[0]);
                SaveSettings(ModLoader.LoadedMods[1]);
            }
        }

        private void OnApplicationQuit()
        {
            isApplicationQuitting = true;
        }
    }
}

#endif