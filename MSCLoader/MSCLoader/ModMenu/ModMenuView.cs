using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Steamworks;
using UnityEngine.UI;

namespace MSCLoader;

internal class ModMenuView : MonoBehaviour
{
    public Text ModTab, ReferenceTab, UpdateTab;

    public GameObject ModElementPrefab, ReferenceElementPrefab, UpdateElementPrefab;
    public GameObject HeaderGroupPrefab;
    public GameObject ButtonPrefab, CheckBoxPrefab, KeyBindPrefab, LabelPrefab, SliderPrefab, TextBoxPrefab;
    public GameObject DropDownListPrefab, ColorPickerPrefab;
    public UniversalView universalView;

    public bool modList;
    public GameObject modListView;
#if !Mini
    public void RefreshTabs()
    {
        if (ModLoader.InvalidMods.Count > 0)
            ModTab.text =
                $"Mods (<color=lime>{ModLoader.Instance.actualModList.Length}</color>/<color=magenta>{ModLoader.InvalidMods.Count}</color>)";
        else
            ModTab.text = $"Mods (<color=lime>{ModLoader.Instance.actualModList.Length}</color>)";
        ReferenceTab.text = $"References (<color=aqua>{ModLoader.Instance.ReferencesList.Count}</color>)";
        UpdateTab.text =
            $"Updates (<color=yellow>{ModLoader.HasUpdateModList.Count + ModLoader.HasUpdateRefList.Count}</color>)";
    }

    public void ModMenuOpened()
    {
        RefreshTabs();
        if (modList) ModList(modListView, string.Empty);
    }

    private IEnumerator ModListAsync(GameObject listView, string search)
    {
        var filteredList = new Mod[0];
        var filteredInvalidList = new InvalidMods[0];
        if (search == string.Empty)
        {
            filteredList = ModLoader.Instance.actualModList;
            filteredInvalidList = ModLoader.InvalidMods.ToArray();
        }
        else
        {
            filteredList = ModLoader.Instance.actualModList.Where(x =>
                x.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                x.ID.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
            filteredInvalidList = ModLoader.InvalidMods
                .Where(x => x.FileName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
        }

        //  ModConsole.Warning(filteredList.Length.ToString());
        for (var i = 0; i < filteredList.Length; i++)
        {
            var mod = Instantiate(ModElementPrefab);
            mod.GetComponent<MenuElementList>().mod = filteredList[i];
            mod.GetComponent<MenuElementList>().ModInfoFill();
            mod.GetComponent<MenuElementList>().ModButtonsPrep(universalView);
            mod.transform.SetParent(listView.transform, false);
            mod.SetActive(true);
            yield return null;
        }

        for (var i = 0; i < filteredInvalidList.Length; i++)
        {
            var mod = Instantiate(ModElementPrefab);
            mod.GetComponent<MenuElementList>().InvalidMod(filteredInvalidList[i]);
            mod.transform.SetParent(listView.transform, false);
            mod.SetActive(true);
            yield return null;
        }

        if (filteredList.Length == 0 && filteredInvalidList.Length == 0)
        {
            var tx = CreateText(listView.transform, "<color=aqua>Nothing found</color>");
            tx.settingName.alignment = TextAnchor.MiddleCenter;
        }
    }

    public void ModList(GameObject listView, string search)
    {
        StopAllCoroutines();
        RemoveChildren(listView.transform);
        if (ModLoader.Instance.actualModList.Length == 0 && search == string.Empty)
        {
            var tx = CreateText(listView.transform,
                $"<color=aqua>A little empty here, seems like there are no mods installed.{Environment.NewLine}If you think that you installed mods, check if you put mods in correct folder.{Environment.NewLine}Current Mod folder is: <color=yellow>{ModLoader.ModsFolder}</color></color>");
            tx.settingName.alignment = TextAnchor.MiddleCenter;
        }

        StartCoroutine(ModListAsync(listView, search));
    }

    public void UpdateList(GameObject listView)
    {
        RemoveChildren(listView.transform);
        if (ModLoader.HasUpdateModList.Count == 0 && ModLoader.HasUpdateRefList.Count == 0)
        {
            var tx = CreateText(listView.transform, "<color=aqua>Everything seems to be up to date!</color>");
            tx.settingName.alignment = TextAnchor.MiddleCenter;
        }

        for (var i = 0; i < ModLoader.HasUpdateModList.Count; i++)
        {
            var mod = Instantiate(UpdateElementPrefab);
            mod.GetComponent<MenuElementList>().mod = ModLoader.HasUpdateModList[i];
            mod.GetComponent<MenuElementList>().UpdateInfoFill();
            mod.transform.SetParent(listView.transform, false);
            mod.SetActive(true);
        }

        for (var i = 0; i < ModLoader.HasUpdateRefList.Count; i++)
        {
            var mod = Instantiate(UpdateElementPrefab);
            mod.GetComponent<MenuElementList>().refs = ModLoader.HasUpdateRefList[i];
            mod.GetComponent<MenuElementList>().UpdateInfoFill();
            mod.transform.SetParent(listView.transform, false);
            mod.SetActive(true);
        }
    }

    public void ReferencesList(GameObject listView)
    {
        RemoveChildren(listView.transform);
        if (ModLoader.Instance.ReferencesList.Count == 0)
        {
            var tx = CreateText(listView.transform, "<color=aqua>No additional references are installed.</color>");
            tx.settingName.alignment = TextAnchor.MiddleCenter;
        }

        for (var i = 0; i < ModLoader.Instance.ReferencesList.Count; i++)
        {
            var mod = Instantiate(ReferenceElementPrefab);
            mod.GetComponent<MenuElementList>().ReferenceInfoFill(ModLoader.Instance.ReferencesList[i]);
            mod.transform.SetParent(listView.transform, false);
            mod.SetActive(true);
        }
    }

    public void MainSettingsList(GameObject listView)
    {
        RemoveChildren(listView.transform);
        Transform currentTransform = null;
        for (var i = 0; i < Settings.Get(ModLoader.LoadedMods[0]).Count; i++)
            if (Settings.Get(ModLoader.LoadedMods[0])[i].SettingType == SettingsType.Header)
                currentTransform = SettingsHeader(Settings.Get(ModLoader.LoadedMods[0])[i], listView.transform);
            else
                SettingsList(Settings.Get(ModLoader.LoadedMods[0])[i], currentTransform);
        var keyBind = Instantiate(KeyBindPrefab);
        keyBind.GetComponent<KeyBinding>().LoadBind(ModLoader.LoadedMods[0].Keybinds[0], ModLoader.LoadedMods[0]);
        keyBind.transform.SetParent(currentTransform, false);
        for (var i = 0; i < Settings.Get(ModLoader.LoadedMods[1]).Count; i++)
            if (Settings.Get(ModLoader.LoadedMods[1])[i].SettingType == SettingsType.Header)
                currentTransform = SettingsHeader(Settings.Get(ModLoader.LoadedMods[1])[i], listView.transform);
            else
                SettingsList(Settings.Get(ModLoader.LoadedMods[1])[i], currentTransform);
    }

    public void MetadataInfoList(GameObject listView, Mod mod)
    {
        RemoveChildren(listView.transform);
        SaveLoad.LoadModsSaveData();
        var savedDataCount = SaveLoad.saveFileData.GetTags().Where(x => x.StartsWith($"{mod.ID}||")).Count();
        listView.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
        //Info Header
        var header = CreateHeader(listView.transform, "Mod Information", Color.cyan);
        var tx = CreateText(header.HeaderListView.transform,
            $"<color=yellow>ID:</color> <color=aqua>{mod.ID}</color> (Compiled using FreeLoader <color=yellow>{mod.compiledVersion}</color>){Environment.NewLine}" +
            $"<color=yellow>Version:</color> <color=aqua>{mod.Version}</color>{Environment.NewLine}" +
            $"<color=yellow>Author:</color> <color=aqua>{mod.Author}</color>{Environment.NewLine}" +
            $"<color=yellow>Additional references used by this Mod:</color>{Environment.NewLine}");
        if (mod.AdditionalReferences != null)
            tx.settingName.text += $"<color=aqua>{string.Join(", ", mod.AdditionalReferences)}</color>";
        else
            tx.settingName.text += "<color=aqua>[None]</color>";
        if (savedDataCount > 0)
        {
            tx.settingName.text +=
                $"{Environment.NewLine}{Environment.NewLine}<color=yellow>Unified save system: </color><color=aqua>{savedDataCount}</color><color=lime> saved values</color>";
            var rbtn = CreateButton(header.HeaderListView.transform, "Reset save file", Color.white, Color.black);
            rbtn.button.onClick.AddListener(delegate
            {
                if (ModLoader.GetCurrentScene() != CurrentScene.MainMenu)
                    ModUI.ShowMessage("You can only use this option in main menu.");
                ModUI.ShowYesNoMessage(
                    $"Resetting this mod's save file will reset this mod to default state. {Environment.NewLine}This cannot be undone.{Environment.NewLine}{Environment.NewLine}Are you sure you want to continue?",
                    "Warning", delegate
                    {
                        SaveLoad.ResetSaveForMod(mod);
                        MetadataInfoList(listView, mod);
                    });
            });
        }

        if (mod.metadata == null)
        {
            var header2 = CreateHeader(listView.transform, "No metadata", Color.yellow);
            CreateText(header2.HeaderListView.transform,
                "<color=yellow>This mod doesn't contain additional information</color>");
        }
        else
        {
            var header2 = CreateHeader(listView.transform, "Website Links", Color.yellow);
            if (string.IsNullOrEmpty(mod.metadata.links[0]) && string.IsNullOrEmpty(mod.metadata.links[1]) &&
                string.IsNullOrEmpty(mod.metadata.links[2]))
            {
                CreateText(header2.HeaderListView.transform, "<color=yellow>This mod doesn't contain links</color>");
            }
            else
            {
                if (!string.IsNullOrEmpty(mod.metadata.links[0]))
                {
                    var nexusBtn = CreateButton(header2.HeaderListView.transform,
                        "SHOW ON <color=orange>NEXUSMODS.COM</color>", Color.white, new Color32(2, 35, 60, 255));
                    nexusBtn.settingName.alignment = TextAnchor.MiddleLeft;
                    nexusBtn.iconElement.texture = nexusBtn.iconPack[1];
                    nexusBtn.iconElement.gameObject.SetActive(true);
                    nexusBtn.button.onClick.AddListener(() => OpenModLink(mod.metadata.links[0]));
                }

                if (!string.IsNullOrEmpty(mod.metadata.links[1]))
                {
                    var rdBtn = CreateButton(header2.HeaderListView.transform,
                        "SHOW ON <color=orange>RACEDEPARTMENT.COM</color>", Color.white, new Color32(2, 35, 49, 255));
                    rdBtn.settingName.alignment = TextAnchor.MiddleLeft;
                    rdBtn.iconElement.texture = rdBtn.iconPack[0];
                    rdBtn.iconElement.gameObject.SetActive(true);
                    rdBtn.button.onClick.AddListener(() => OpenModLink(mod.metadata.links[1]));
                }

                if (!string.IsNullOrEmpty(mod.metadata.links[2]))
                {
                    var ghBtn = CreateButton(header2.HeaderListView.transform,
                        "SHOW ON <color=orange>GITHUB.COM</color>", Color.white, Color.black);
                    ghBtn.settingName.alignment = TextAnchor.MiddleLeft;
                    ghBtn.iconElement.texture = ghBtn.iconPack[2];
                    ghBtn.iconElement.gameObject.SetActive(true);
                    ghBtn.button.onClick.AddListener(() => OpenModLink(mod.metadata.links[2]));
                }
            }

            var header3 = CreateHeader(listView.transform, "Description", Color.yellow);
            CreateText(header3.HeaderListView.transform, mod.metadata.description);
        }
    }

    internal void MetadataUploadForm(GameObject listView, Mod mod)
    {
        RemoveChildren(listView.transform);
        listView.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
        //Info Header
        var header = CreateHeader(listView.transform, "Mod Information", Color.cyan);
        var tx = CreateText(header.HeaderListView.transform,
            $"<color=yellow>ID:</color> <color=aqua>{mod.ID}</color> (Compiled using FreeLoader <color=yellow>{mod.compiledVersion}</color>){Environment.NewLine}" +
            $"<color=yellow>Version:</color> <color=aqua>{mod.Version}</color>{Environment.NewLine}" +
            $"<color=yellow>Author:</color> <color=aqua>{mod.Author}</color>{Environment.NewLine}" +
            $"<color=yellow>Additional references used by this Mod:</color>{Environment.NewLine}");
        if (mod.AdditionalReferences != null)
            tx.settingName.text += $"<color=aqua>{string.Join(", ", mod.AdditionalReferences)}</color>";
        else
            tx.settingName.text += "<color=aqua>[None]</color>";
        //----------------------------------
        var refList = new List<References>();
        //----------------------------------
        var header2 = CreateHeader(listView.transform, "Bundle Assets", Color.yellow);
        var tx2 = CreateText(header2.HeaderListView.transform, "");
        if (!Directory.Exists(Path.Combine(ModLoader.AssetsFolder, mod.ID)))
        {
            tx2.settingName.text = "<color=yellow>Looks like this mod doesn't have assets folder.</color>";
        }
        else
        {
            tx2.settingName.text =
                "Select below option if you want to include Assets folder (in most cases you should do it)";
            var checkboxP = Instantiate(CheckBoxPrefab);
            var checkbox = checkboxP.GetComponent<SettingsElement>();
            checkbox.settingName.text = "Include Assets Folder";
            checkbox.checkBox.isOn = true;
            checkbox.checkBox.onValueChanged.AddListener(delegate { _ = checkbox.checkBox.isOn; });
            checkbox.transform.SetParent(header2.HeaderListView.transform, false);
        }

        var header3 = CreateHeader(listView.transform, "Bundle References", Color.yellow);
        var tx3 = CreateText(header3.HeaderListView.transform, "");
        if (mod.AdditionalReferences == null)
        {
            tx3.settingName.text = "<color=yellow>Looks like this mod doesn't use additional references.</color>";
        }
        else
        {
            tx3.settingName.text =
                $"<color=yellow>You can bundle references{Environment.NewLine}Do it only if reference is exclusive to your mod, otherwise you should create Reference update separately.</color>{Environment.NewLine}";
            foreach (var rf in mod.AdditionalReferences)
            {
                if (rf.StartsWith("MSCLoader"))
                {
                    tx3.settingName.text +=
                        $"{Environment.NewLine}<color=red><b>{rf}</b></color> - Cannot be bundled (blacklisted)";
                    continue;
                }

                var refe = ModLoader.Instance.ReferencesList.Where(x => x.AssemblyID == rf).FirstOrDefault();
                if (refe == null)
                {
                    tx3.settingName.text +=
                        $"{Environment.NewLine}<color=aqua><b>{rf}</b></color> - Looks like mod, cannot be bundled.";
                }
                else
                {
                    if (refe.UpdateInfo != null)
                    {
                        if (refe.UpdateInfo.ref_type == 1)
                            tx3.settingName.text +=
                                $"{Environment.NewLine}<color=lime><b>{rf}</b></color> - Reference is updated separately";
                        else
                            tx3.settingName.text +=
                                $"{Environment.NewLine}<color=orange><b>{rf}</b></color> - Registered Reference, update it first";
                    }
                    else
                    {
                        var checkboxP3 = Instantiate(CheckBoxPrefab);
                        var checkbox3 = checkboxP3.GetComponent<SettingsElement>();
                        checkbox3.settingName.text = rf;
                        checkbox3.checkBox.isOn = false;
                        checkbox3.checkBox.onValueChanged.AddListener(delegate
                        {
                            if (checkbox3.checkBox.isOn)
                                refList.Add(refe);
                            else
                                refList.Remove(refe);
                        });
                        checkbox3.transform.SetParent(header3.HeaderListView.transform, false);
                    }
                }
            }
        }

        var header4 = CreateHeader(listView.transform, "Upload", Color.yellow);
        CreateText(header4.HeaderListView.transform,
            "<b><color=aqua>Recomended option!</color></b> Updates mod information to latest version and Uploads it, so it's available as in-game update.");

        var uploadBtn = CreateButton(header4.HeaderListView.transform, "Upload and Update Mod", Color.white,
            Color.black);
        uploadBtn.button.onClick.AddListener(delegate
        {
            ModConsole.Error("Can't upload mods on freedom version...");
        });
        CreateText(header4.HeaderListView.transform,
            $"{Environment.NewLine}This button below updates mod's version only. Use this if you want to update version number only, without uploading update file. (mod will not be available as in-game update)");

        var uploadBtn2 = CreateButton(header4.HeaderListView.transform, "Update Mod version only", Color.white,
            Color.black);
        uploadBtn2.button.onClick.AddListener(delegate
        {
            ModConsole.Error("Can't upload mods on freedom version...");
        });
    }

    internal static void OpenModLink(string url)
    {
        if (ModMenu.openLinksOverlay.GetValue())
        {
            //try opening in steam overlay
            try
            {
#if !Mini
                SteamFriends.ActivateGameOverlayToWebPage(url);
#endif
            }
            catch (Exception e)
            {
                ModConsole.Error(e.Message);
                Console.WriteLine(e);
                Application.OpenURL(url);
                Console.WriteLine(url);
            }
        }
        else
        {
            Application.OpenURL(url);
            Console.WriteLine(url);
        }
    }

    public void ModSettingsList(GameObject listView, Mod mod)
    {
        RemoveChildren(listView.transform);
        listView.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
        Transform currentTransform = null;
        //If first settings element is not header, create one.
        if (mod.proSettings)
        {
            var header = CreateHeader(listView.transform, "Incompatible settings", Color.white);
            header.HeaderBackground.color = Color.red;
            CreateText(header.HeaderListView.transform,
                $"<color=aqua>Incompatible settings format! Settings below may not load or work correctly.</color>{Environment.NewLine}Report that to mod author to use proper settings format.");
        }

        if (Settings.Get(mod)[0].SettingType != SettingsType.Header)
        {
            var header = CreateHeader(listView.transform, "Settings", Color.cyan);
            currentTransform = header.HeaderListView.transform;
        }

        for (var i = 0; i < Settings.Get(mod).Count; i++)
            if (Settings.Get(mod)[i].SettingType == SettingsType.Header)
                currentTransform = SettingsHeader(Settings.Get(mod)[i], listView.transform);
            else
                SettingsList(Settings.Get(mod)[i], currentTransform);

        if (!mod.hideResetAllSettings)
        {
            var rbtn = CreateButton(listView.transform, "Reset all settings to default", Color.white, Color.black);
            rbtn.button.onClick.AddListener(delegate
            {
                ModMenu.ResetSettings(mod);
                universalView.FillSettings(mod);
                if (mod.newSettingsFormat)
                {
                    if (mod.A_ModSettingsLoaded != null) mod.A_ModSettingsLoaded.Invoke();
                }
                else
                {
                    mod.ModSettingsLoaded();
                }
            });
        }
    }

    public void KeyBindsList(GameObject listView, Mod mod)
    {
        RemoveChildren(listView.transform);
        listView.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
        Transform currentTransform = null;
        //If first settings element is not header, create one.
        if (mod.Keybinds[0].ID != null && mod.Keybinds[0].Vals == null)
        {
            var header = CreateHeader(listView.transform, "Keybinds", Color.yellow);
            currentTransform = header.HeaderListView.transform;
        }

        for (var i = 0; i < mod.Keybinds.Count; i++)
            if (mod.Keybinds[i].ID == null && mod.Keybinds[i].Vals != null)
            {
                var header = CreateHeader(listView.transform, mod.Keybinds[i].Name, (Color)mod.Keybinds[i].Vals[1]);
                header.HeaderBackground.color = (Color)mod.Keybinds[i].Vals[0];
                currentTransform = header.HeaderListView.transform;
            }
            else
            {
                var keyBind = Instantiate(KeyBindPrefab);
                keyBind.GetComponent<KeyBinding>().LoadBind(mod.Keybinds[i], mod);
                keyBind.transform.SetParent(currentTransform, false);
            }

        var rbtn = CreateButton(listView.transform, "Reset all Keybinds to default", Color.white, Color.black);
        rbtn.button.onClick.AddListener(delegate
        {
            ModMenu.ResetBinds(mod);
            universalView.FillKeybinds(mod);
        });
    }

    private Transform SettingsHeader(ModSetting set, Transform listView)
    {
        var setting = (SettingsHeader)set;
        var header = CreateHeader(listView.transform, setting.Name, setting.TextColor);
        header.HeaderBackground.color = setting.BackgroundColor;
        setting.HeaderElement = header;
        if (setting.CollapsedByDefault) header.SetHeaderNoAnim(false);
        header.gameObject.SetActive(setting.IsVisible);
        return header.HeaderListView.transform;
    }

    private void SettingsList(ModSetting set, Transform listView)
    {
        switch (set.SettingType)
        {
            case SettingsType.CheckBox:
                var settingCheckBox = (SettingsCheckBox)set;
                var checkboxP = Instantiate(CheckBoxPrefab);
                var checkbox = checkboxP.GetComponent<SettingsElement>();
                settingCheckBox.SettingsElement = checkbox;
                checkbox.SetupCheckbox(settingCheckBox.Name, settingCheckBox.Value, null);
                checkbox.checkBox.onValueChanged.AddListener(delegate
                {
                    settingCheckBox.Value = checkbox.checkBox.isOn;
                    if (settingCheckBox.DoAction != null)
                        settingCheckBox.DoAction.Invoke();
                });
                checkbox.transform.SetParent(listView, false);
                checkbox.gameObject.SetActive(settingCheckBox.IsVisible);
                break;
            case SettingsType.CheckBoxGroup:
                var settingsCheckBoxGroup = (SettingsCheckBoxGroup)set;
                GameObject group;
                if (listView.FindChild(settingsCheckBoxGroup.CheckBoxGroup) == null)
                {
                    group = new GameObject(settingsCheckBoxGroup.CheckBoxGroup);
                    group.AddComponent<ToggleGroup>();
                    group.transform.SetParent(listView, false);
                }
                else
                {
                    group = listView.FindChild(settingsCheckBoxGroup.CheckBoxGroup).gameObject;
                }

                var checkboxGP = Instantiate(CheckBoxPrefab);
                var checkboxG = checkboxGP.GetComponent<SettingsElement>();
                settingsCheckBoxGroup.SettingsElement = checkboxG;
                checkboxG.SetupCheckbox(settingsCheckBoxGroup.Name, settingsCheckBoxGroup.Value,
                    group.GetComponent<ToggleGroup>());

                if (settingsCheckBoxGroup.Value)
                    checkboxG.checkBox.group.NotifyToggleOn(checkboxG.checkBox);
                checkboxG.checkBox.onValueChanged.AddListener(delegate
                {
                    settingsCheckBoxGroup.Value = checkboxG.checkBox.isOn;
                    if (settingsCheckBoxGroup.DoAction != null)
                        settingsCheckBoxGroup.DoAction.Invoke();
                });
                checkboxG.transform.SetParent(listView, false);
                checkboxG.gameObject.SetActive(settingsCheckBoxGroup.IsVisible);
                break;
            case SettingsType.Button:
                var settingBtn = (SettingsButton)set;
                var btnP = Instantiate(ButtonPrefab);
                var btn = btnP.GetComponent<SettingsElement>();
                settingBtn.SettingsElement = btn;
                btn.SetupButton(settingBtn.Name.ToUpper(), settingBtn.TextColor, settingBtn.BackgroundColor);
                if (settingBtn.PredefinedIcon != SettingsButton.ButtonIcon.None)
                {
                    btn.settingName.alignment = TextAnchor.MiddleLeft;
                    if (settingBtn.PredefinedIcon == SettingsButton.ButtonIcon.Custom)
                    {
                        if (settingBtn.CustomIcon == null)
                            ModConsole.Error($"Custom icon for Button {settingBtn.Name} is null.");
                        btn.iconElement.texture = settingBtn.CustomIcon;
                    }
                    else
                    {
                        btn.iconElement.texture = btn.iconPack[(int)settingBtn.PredefinedIcon];
                    }

                    btn.iconElement.gameObject.SetActive(true);
                }

                btn.button.onClick.AddListener(settingBtn.DoAction.Invoke);
                btn.transform.SetParent(listView, false);
                btn.gameObject.SetActive(settingBtn.IsVisible);
                break;
            case SettingsType.RButton:
                var settingRes = (SettingsResetButton)set;
                var rbtnP = Instantiate(ButtonPrefab);
                var rbtn = rbtnP.GetComponent<SettingsElement>();
                settingRes.SettingsElement = rbtn;
                rbtn.SetupButton(settingRes.Name.ToUpper(), Color.white, Color.black);
                rbtn.button.onClick.AddListener(delegate
                {
                    settingRes.ResetSettings();
                    universalView.FillSettings(settingRes.ThisMod);
                    if (settingRes.ThisMod.newSettingsFormat)
                    {
                        if (settingRes.ThisMod.A_ModSettingsLoaded != null)
                            settingRes.ThisMod.A_ModSettingsLoaded.Invoke();
                    }
                    else
                    {
                        settingRes.ThisMod.ModSettingsLoaded();
                    }
                });
                rbtn.transform.SetParent(listView, false);
                rbtn.gameObject.SetActive(settingRes.IsVisible);
                break;
            case SettingsType.SliderInt:
                var settingSliderInt = (SettingsSliderInt)set;
                var slidrIntP = Instantiate(SliderPrefab);
                var slidrInt = slidrIntP.GetComponent<SettingsElement>();
                settingSliderInt.SettingsElement = slidrInt;
                slidrInt.SetupSliderInt(settingSliderInt.Name, settingSliderInt.Value, settingSliderInt.MinValue,
                    settingSliderInt.MaxValue, settingSliderInt.TextValues);
                slidrInt.slider.onValueChanged.AddListener(delegate
                {
                    settingSliderInt.SetValue((int)slidrInt.slider.value);
                    if (settingSliderInt.TextValues != null)
                        slidrInt.value.text = settingSliderInt.TextValues[settingSliderInt.Value];
                    if (settingSliderInt.DoAction != null)
                        settingSliderInt.DoAction.Invoke();
                });
                slidrInt.transform.SetParent(listView, false);
                slidrInt.gameObject.SetActive(settingSliderInt.IsVisible);
                break;
            case SettingsType.Slider:
                var settingSlider = (SettingsSlider)set;
                var slidrP = Instantiate(SliderPrefab);
                var slidr = slidrP.GetComponent<SettingsElement>();
                settingSlider.SettingsElement = slidr;
                slidr.SetupSlider(settingSlider.Name, settingSlider.Value, settingSlider.MinValue,
                    settingSlider.MaxValue);
                slidr.slider.onValueChanged.AddListener(delegate
                {
                    settingSlider.SetValue((float)Math.Round(slidr.slider.value, settingSlider.DecimalPoints));
                    if (settingSlider.DoAction != null)
                        settingSlider.DoAction.Invoke();
                });
                slidr.transform.SetParent(listView, false);
                slidr.gameObject.SetActive(settingSlider.IsVisible);
                break;
            case SettingsType.TextBox:
                var settingTxtBox = (SettingsTextBox)set;
                var txtP = Instantiate(TextBoxPrefab);
                var txt = txtP.GetComponent<SettingsElement>();
                settingTxtBox.SettingsElement = txt;
                txt.SetupTextBox(settingTxtBox.Name, settingTxtBox.Value, settingTxtBox.Placeholder,
                    settingTxtBox.ContentType);
                txt.textBox.onValueChange.AddListener(delegate { settingTxtBox.Value = txt.textBox.text; });
                txt.transform.SetParent(listView, false);
                txt.gameObject.SetActive(settingTxtBox.IsVisible);
                break;
            case SettingsType.DropDown:
                var settingDropDown = (SettingsDropDownList)set;
                var ddlP = Instantiate(DropDownListPrefab);
                var ddl = ddlP.GetComponent<SettingsElement>();
                settingDropDown.SettingsElement = ddl;
                ddl.settingName.text = settingDropDown.Name;
                ddl.dropDownList.Items = new List<DropDownListItem>();
                for (var i = 0; i < settingDropDown.ArrayOfItems.Length; i++)
                {
                    var s = settingDropDown.ArrayOfItems[i];
                    var ddli = new DropDownListItem(s, i.ToString());
                    ddl.dropDownList.Items.Add(ddli);
                }

                ddl.dropDownList.SelectedIndex = settingDropDown.Value;
                ddl.dropDownList.OnSelectionChanged = delegate
                {
                    settingDropDown.Value = ddl.dropDownList.SelectedIndex;
                    if (settingDropDown.DoAction != null)
                        settingDropDown.DoAction.Invoke();
                };
                ddl.transform.SetParent(listView, false);
                ddl.gameObject.SetActive(settingDropDown.IsVisible);
                break;
            case SettingsType.ColorPicker:
                var settingColorPicker = (SettingsColorPicker)set;
                var colpp = Instantiate(ColorPickerPrefab);
                var colp = colpp.GetComponent<SettingsElement>();
                settingColorPicker.SettingsElement = colp;
                colp.settingName.text = settingColorPicker.Name;
                colp.colorPicker.CurrentColor = settingColorPicker.GetValue();
                if (settingColorPicker.ShowAlpha) colp.colorPicker.AlphaSlider.SetActive(true);
                colp.colorPicker.onValueChanged.AddListener(col =>
                {
                    settingColorPicker.SetValue(col);
                    if (settingColorPicker.DoAction != null)
                        settingColorPicker.DoAction.Invoke();
                });
                colp.transform.SetParent(listView, false);
                colp.gameObject.SetActive(settingColorPicker.IsVisible);
                break;
            case SettingsType.Text:
                var settingText = (SettingsText)set;
                var tx = Instantiate(LabelPrefab);
                var label = tx.GetComponent<SettingsElement>();
                settingText.SettingsElement = label;
                label.settingName.text = settingText.Name;
                tx.transform.SetParent(listView, false);
                label.gameObject.SetActive(settingText.IsVisible);
                break;
        }
    }

    public void RemoveChildren(Transform parent)
    {
        if (parent.childCount > 0)
            for (var i = 0; i < parent.childCount; i++)
                Destroy(parent.GetChild(i).gameObject);
    }

    public SettingsGroup CreateHeader(Transform listView, string title, Color textColor)
    {
        var hdr = Instantiate(HeaderGroupPrefab);
        var header = hdr.GetComponent<SettingsGroup>();
        header.HeaderTitle.text = title.ToUpper();
        header.HeaderTitle.color = textColor;
        hdr.transform.SetParent(listView, false);
        return header;
    }

    public SettingsElement CreateButton(Transform listView, string text, Color textColor, Color btnColor)
    {
        var btnP = Instantiate(ButtonPrefab);
        var btn = btnP.GetComponent<SettingsElement>();
        btn.settingName.text = text.ToUpper();
        btn.settingName.color = textColor;
        btn.button.GetComponent<Image>().color = btnColor;
        btn.transform.SetParent(listView.transform, false);
        return btn;
    }

    public SettingsElement CreateText(Transform listView, string text)
    {
        var tx = Instantiate(LabelPrefab);
        var txt = tx.GetComponent<SettingsElement>();
        txt.settingName.text = text;
        tx.transform.SetParent(listView.transform, false);
        return txt;
    }
#endif
}