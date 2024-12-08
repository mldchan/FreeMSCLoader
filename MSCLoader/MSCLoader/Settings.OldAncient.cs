#if !Mini
using System;
using System.ComponentModel;
using UnityEngine.UI;

namespace MSCLoader;

public partial class Settings
{
    //List of Ancient hard obsoleted Settings functions (used only as backwards compatibility)

    internal ModSetting modSetting;

    /// <summary>
    ///     Get value of setting.
    /// </summary>
    /// <returns>Raw value of setting</returns>
    [Obsolete("Please switch to new settings format.", true)]
    public object GetValue()
    {
        //Legacy Backwards compatibility with V3 settings
        switch (modSetting.SettingType)
        {
            case SettingsType.CheckBoxGroup:
                return ((SettingsCheckBoxGroup)modSetting).GetValue();
            case SettingsType.CheckBox:
                return ((SettingsCheckBox)modSetting).GetValue();
            case SettingsType.Slider:
                return ((SettingsSlider)modSetting).GetValue();
            case SettingsType.SliderInt:
                return ((SettingsSliderInt)modSetting).GetValue();
            case SettingsType.TextBox:
                return ((SettingsTextBox)modSetting).GetValue();
            case SettingsType.Header:
            case SettingsType.Button:
            case SettingsType.RButton:
            case SettingsType.Text:
            case SettingsType.DropDown:
            case SettingsType.ColorPicker:
            default:
                return null;
        }
    }

    /// <summary>
    ///     Add checkbox to settings menu (only bool Value accepted)
    ///     Can execute action when its value is changed.
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="setting">Your settings variable</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Please switch to new settings format.", true)]
    public static void AddCheckBox(Mod mod, Settings setting)
    {
        settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
        var s = AddCheckBox(setting.ID, setting.Name, bool.Parse(setting.Value.ToString()), setting.DoAction);
        setting.modSetting = s;
    }

    /// <summary>
    ///     Add checkbox to settings menu with group, only one checkbox can be set to true in this group
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="setting">Your settings variable</param>
    /// <param name="group">Unique group name, same for all checkboxes that will be grouped</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Please switch to new settings format.", true)]
    public static void AddCheckBox(Mod mod, Settings setting, string group)
    {
        settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
        var s = AddCheckBoxGroup(setting.ID, setting.Name, bool.Parse(setting.Value.ToString()), group,
            setting.DoAction);
        setting.modSetting = s;
    }

    /// <summary>
    ///     Add button that can execute function.
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="setting">Your settings variable</param>
    /// <param name="description">Short optional description for this button</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Please switch to new settings format.", true)]
    public static void AddButton(Mod mod, Settings setting, string description = null)
    {
        AddButton(mod, setting, new Color32(0, 113, 166, 255), new Color32(0, 153, 166, 255),
            new Color32(0, 183, 166, 255), description);
    }

    /// <summary>
    ///     Add button that can execute function.
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="setting">Your settings variable</param>
    /// <param name="normalColor">Button color</param>
    /// <param name="highlightedColor">Button color when highlighted</param>
    /// <param name="pressedColor">Button color when pressed</param>
    /// <param name="description">Short optional description for this button</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Please switch to new settings format.", true)]
    public static void AddButton(Mod mod, Settings setting, Color normalColor, Color highlightedColor,
        Color pressedColor, string description = null)
    {
        AddButton(mod, setting, normalColor, highlightedColor, pressedColor, Color.white, description);
    }

    /// <summary>
    ///     Add button that can execute function.
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="setting">Your settings variable</param>
    /// <param name="normalColor">Button color</param>
    /// <param name="highlightedColor">Button color when highlighted</param>
    /// <param name="pressedColor">Button color when pressed</param>
    /// <param name="buttonTextColor">Text color on Button</param>
    /// <param name="description">Short optional description for this button</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Please switch to new settings format.", true)]
    public static void AddButton(Mod mod, Settings setting, Color normalColor, Color highlightedColor,
        Color pressedColor, Color buttonTextColor, string description = null)
    {
        settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
        AddButton(setting.Name, setting.DoAction, normalColor, buttonTextColor);
    }

    /// <summary>
    ///     Add Slider, slider can execute action when its value is changed.
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="setting">Your settings variable</param>
    /// <param name="maxValue">Max value of slider</param>
    /// <param name="minValue">Min value of slider</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Please switch to new settings format.", true)]
    public static void AddSlider(Mod mod, Settings setting, int minValue, int maxValue)
    {
        AddSlider(mod, setting, minValue, maxValue, null);
    }

    /// <summary>
    ///     Add Slider, slider can execute action when its value is changed.
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="setting">Your settings variable</param>
    /// <param name="maxValue">Max value of slider</param>
    /// <param name="minValue">Min value of slider</param>
    /// <param name="textValues">Array of text values (array index equals to slider value)</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Please switch to new settings format.", true)]
    public static void AddSlider(Mod mod, Settings setting, int minValue, int maxValue, string[] textValues)
    {
        settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
        var s = AddSlider(setting.ID, setting.Name, minValue, maxValue, int.Parse(setting.Value.ToString()),
            setting.DoAction, textValues);
        setting.modSetting = s;
    }

    /// <summary>
    ///     Add Slider, slider can execute action when its value is changed.
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="setting">Your settings variable</param>
    /// <param name="maxValue">Max value of slider</param>
    /// <param name="minValue">Min value of slider</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Please switch to new settings format.", true)]
    public static void AddSlider(Mod mod, Settings setting, float minValue, float maxValue)
    {
        AddSlider(mod, setting, minValue, maxValue, 2);
    }


    /// <summary>
    ///     Add Slider, slider can execute action when its value is changed.
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="setting">Your settings variable</param>
    /// <param name="maxValue">Max value of slider</param>
    /// <param name="minValue">Min value of slider</param>
    /// <param name="decimalPoints">Round value to set number of decimal points (default = 2)</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Please switch to new settings format.", true)]
    public static void AddSlider(Mod mod, Settings setting, float minValue, float maxValue, int decimalPoints)
    {
        settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
        var s = AddSlider(setting.ID, setting.Name, minValue, maxValue, float.Parse(setting.Value.ToString()),
            setting.DoAction, decimalPoints);
        setting.modSetting = s;
    }

    /// <summary>
    ///     Add TextBox where user can type any text
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="setting">Your settings variable</param>
    /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Please switch to new settings format.", true)]
    public static void AddTextBox(Mod mod, Settings setting, string placeholderText)
    {
        AddTextBox(mod, setting, placeholderText, Color.white);
    }

    /// <summary>
    ///     Add TextBox where user can type any text
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="setting">Your settings variable</param>
    /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
    /// <param name="titleTextColor">Text color of title</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Please switch to new settings format.", true)]
    public static void AddTextBox(Mod mod, Settings setting, string placeholderText, Color titleTextColor)
    {
        AddTextBox(mod, setting, placeholderText, titleTextColor, InputField.ContentType.Standard);
    }

    /// <summary>
    ///     Add TextBox where user can type any text
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="setting">Your settings variable</param>
    /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
    /// <param name="titleTextColor">Text color of title</param>
    /// <param name="contentType">InputField content type</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Please switch to new settings format.", true)]
    public static void AddTextBox(Mod mod, Settings setting, string placeholderText, Color titleTextColor,
        InputField.ContentType contentType)
    {
        settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
        var s = AddTextBox(setting.ID, setting.Name, setting.Value.ToString(), placeholderText, contentType);
        setting.modSetting = s;
    }
}
#endif