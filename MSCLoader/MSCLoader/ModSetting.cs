#if !Mini
using System;
using System.ComponentModel;
using UnityEngine.UI;

namespace MSCLoader;

/// <summary>
///     Mod Setting base class
/// </summary>
public class ModSetting
{
    internal bool DefaultVisibility = true;
    internal Action DoAction;
    internal SettingsGroup HeaderElement;
    internal string ID;
    internal bool IsVisible = true;
    internal string Name;

    internal SettingsElement SettingsElement;
    internal SettingsType SettingType;

    internal ModSetting(string id, string name, Action doAction, SettingsType type, bool visibleByDefault)
    {
        ID = id;
        Name = name;
        DoAction = doAction;
        SettingType = type;
        DefaultVisibility = visibleByDefault;
        IsVisible = visibleByDefault;
    }

    internal void UpdateName(string name)
    {
        Name = name;
        if (SettingsElement == null) return;
        if (SettingsElement.settingName != null) SettingsElement.settingName.text = Name;
    }

    internal void UpdateValue(object Value)
    {
        if (SettingsElement == null) return;
        if (SettingsElement.value != null)
            switch (SettingType)
            {
                case SettingsType.TextBox:
                    SettingsElement.textBox.text = Value.ToString();
                    break;
                case SettingsType.DropDown:
                    SettingsElement.dropDownList.SelectedIndex = int.Parse(Value.ToString());
                    break;
                default:
                    SettingsElement.value.text = Value.ToString();
                    break;
            }
    }

    public void SetVisibility(bool value)
    {
        IsVisible = value;
        if (SettingsElement != null) SettingsElement.gameObject.SetActive(value);
        if (HeaderElement != null) HeaderElement.gameObject.SetActive(value);
    }
}

/// <summary>
///     Settings checkbox
/// </summary>
public class SettingsCheckBox : ModSetting
{
    internal bool DefaultValue;

    /// <summary>
    ///     Settings Instance (used for custom reset button)
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Settings Instance;

    internal bool Value;

    internal SettingsCheckBox(string id, string name, bool value, Action doAction, bool visibleByDefault) : base(id,
        name, doAction, SettingsType.CheckBox, visibleByDefault)
    {
        Value = value;
        DefaultValue = value;
        Instance = new Settings(this); //Compatibility only
    }

    /// <summary>
    ///     Get checkbox value
    /// </summary>
    /// <returns>true/false</returns>
    public bool GetValue()
    {
        return Value;
    }

    /// <summary>
    ///     Set checkbox value
    /// </summary>
    /// <param name="value">true/false</param>
    public void SetValue(bool value)
    {
        Value = value;
        UpdateValue(value);
    }
}

/// <summary>
///     CheckBox group (aka radio button)
/// </summary>
public class SettingsCheckBoxGroup : ModSetting
{
    internal string CheckBoxGroup = string.Empty;
    internal bool DefaultValue;

    /// <summary>
    ///     Settings Instance (used for custom reset button)
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Settings Instance;

    internal bool Value;

    internal SettingsCheckBoxGroup(string id, string name, bool value, string group, Action doAction,
        bool visibleByDefault) : base(id, name, doAction, SettingsType.CheckBoxGroup, visibleByDefault)
    {
        Value = value;
        DefaultValue = value;
        CheckBoxGroup = group;
        Instance = new Settings(this); //Compatibility only
    }

    /// <summary>
    ///     Get checkbox value
    /// </summary>
    /// <returns>true/false</returns>
    public bool GetValue()
    {
        return Value;
    }

    /// <summary>
    ///     Set checkbox value
    /// </summary>
    /// <param name="value">true/false</param>
    public void SetValue(bool value)
    {
        Value = value;
        UpdateValue(value);
    }
}

/// <summary>
///     Integer version of Settings Slider
/// </summary>
public class SettingsSliderInt : ModSetting
{
    internal int DefaultValue;

    /// <summary>
    ///     Settings Instance (used for custom reset button)
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Settings Instance;

    internal int MaxValue = 100;
    internal int MinValue;
    internal string[] TextValues;
    internal int Value;

    internal SettingsSliderInt(string id, string name, int value, int minValue, int maxValue, Action onValueChanged,
        string[] textValues, bool visibleByDefault) : base(id, name, onValueChanged, SettingsType.SliderInt,
        visibleByDefault)
    {
        Value = value;
        DefaultValue = value;
        MinValue = minValue;
        MaxValue = maxValue;
        TextValues = textValues;
        Instance = new Settings(this); //Compatibility only
    }

    /// <summary>
    ///     Get slider value
    /// </summary>
    /// <returns>slider value in int</returns>
    public int GetValue()
    {
        return Value;
    }

    /// <summary>
    ///     Set value for slider
    /// </summary>
    /// <param name="value">value</param>
    public void SetValue(int value)
    {
        Value = value;
        UpdateValue(value);
    }
}

/// <summary>
///     Settings Slider
/// </summary>
public class SettingsSlider : ModSetting
{
    internal int DecimalPoints;
    internal float DefaultValue;

    /// <summary>
    ///     Settings Instance (used for custom reset button)
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Settings Instance;

    internal float MaxValue = 100;
    internal float MinValue;
    internal float Value;

    internal SettingsSlider(string id, string name, float value, float minValue, float maxValue, Action onValueChanged,
        int decimalPoints, bool visibleByDefault) : base(id, name, onValueChanged, SettingsType.Slider,
        visibleByDefault)
    {
        Value = value;
        DefaultValue = value;
        MinValue = minValue;
        MaxValue = maxValue;
        DecimalPoints = decimalPoints;
        Instance = new Settings(this); //Compatibility only
    }

    /// <summary>
    ///     Get slider value
    /// </summary>
    /// <returns>slider value in float</returns>
    public float GetValue()
    {
        return Value;
    }

    /// <summary>
    ///     Set value for slider
    /// </summary>
    /// <param name="value">value</param>
    public void SetValue(float value)
    {
        Value = value;
        UpdateValue(value);
    }
}

/// <summary>
///     Settings TextBox
/// </summary>
public class SettingsTextBox : ModSetting
{
    internal InputField.ContentType ContentType = InputField.ContentType.Standard;
    internal string DefaultValue = string.Empty;

    /// <summary>
    ///     Settings Instance (used for custom reset button)
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Settings Instance;

    internal string Placeholder = string.Empty;
    internal string Value = string.Empty;

    internal SettingsTextBox(string id, string name, string value, string placeholder,
        InputField.ContentType contentType, bool visibleByDefault) : base(id, name, null, SettingsType.TextBox,
        visibleByDefault)
    {
        Value = value;
        DefaultValue = value;
        Placeholder = placeholder;
        ContentType = contentType;
        Instance = new Settings(this); //Compatibility only
    }

    /// <summary>
    ///     Get TextBox value
    /// </summary>
    /// <returns>TextBox string value</returns>
    public string GetValue()
    {
        return Value;
    }

    /// <summary>
    ///     Set value for textbox
    /// </summary>
    /// <param name="value">value</param>
    public void SetValue(string value)
    {
        Value = value;
        UpdateValue(value);
    }
}

/// <summary>
///     Settings DropDown List
/// </summary>
public class SettingsDropDownList : ModSetting
{
    internal string[] ArrayOfItems = new string[0];
    internal int DefaultValue;

    /// <summary>
    ///     Settings Instance (used for custom reset button)
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Settings Instance;

    internal int Value;

    internal SettingsDropDownList(string id, string name, string[] arrayOfItems, int defaultValue,
        Action onSelectionChanged, bool visibleByDefault) : base(id, name, onSelectionChanged, SettingsType.DropDown,
        visibleByDefault)
    {
        Value = defaultValue;
        ArrayOfItems = arrayOfItems;
        DefaultValue = defaultValue;
        Instance = new Settings(this);
    }

    /// <summary>
    ///     Get DropDownList selected Item Index (can be accessed from anywhere)
    /// </summary>
    /// <returns>DropDownList selectedIndex as int</returns>
    public int GetSelectedItemIndex()
    {
        return Value;
    }

    /// <summary>
    ///     Get DropDownList selected Item Name (Only possible if settings are open).
    /// </summary>
    /// <returns>DropDownList selected item name as string</returns>
    public string GetSelectedItemName()
    {
        return ArrayOfItems[Value];
    }

    /// <summary>
    ///     Set DropDownList selected Item Index
    /// </summary>
    /// <param name="value">index</param>
    public void SetSelectedItemIndex(int value)
    {
        if (value >= ArrayOfItems.Length) Value = DefaultValue;
        Value = value;
        UpdateValue(value);
    }
}

/// <summary>
///     Settings Color Picker
/// </summary>
public class SettingsColorPicker : ModSetting
{
    internal string DefaultColorValue = "0,0,0,255";

    /// <summary>
    ///     Settings Instance (used for custom reset button)
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Settings Instance;

    internal bool ShowAlpha;
    internal string Value = "0,0,0,255";


    internal SettingsColorPicker(string id, string name, Color32 defaultColor, bool showAlpha, Action onColorChanged,
        bool visibleByDefault) : base(id, name, onColorChanged, SettingsType.ColorPicker, visibleByDefault)
    {
        Value = $"{defaultColor.r},{defaultColor.g},{defaultColor.b},{defaultColor.a}";
        DefaultColorValue = $"{defaultColor.r},{defaultColor.g},{defaultColor.b},{defaultColor.a}";
        ShowAlpha = showAlpha;
        Instance = new Settings(this); //Compatibility only
    }

    /// <summary>
    ///     Get Color32 value
    /// </summary>
    /// <returns>TextBox string value</returns>
    public Color32 GetValue()
    {
        var colb = Value.Split(',');
        return new Color32(byte.Parse(colb[0]), byte.Parse(colb[1]), byte.Parse(colb[2]), byte.Parse(colb[3]));
    }

    /// <summary>
    ///     Set Color32 value
    /// </summary>
    /// <param name="col">value</param>
    public void SetValue(Color32 col)
    {
        Value = $"{col.r},{col.g},{col.b},{col.a}";
    }
}

/// <summary>
///     Settings Header
/// </summary>
public class SettingsHeader : ModSetting
{
    internal Color BackgroundColor = new Color32(95, 34, 18, 255);
    internal bool CollapsedByDefault;
    internal Color TextColor = new Color32(236, 229, 2, 255);


    internal SettingsHeader(string name, Color backgroundColor, Color textColor, bool collapsedByDefault,
        bool visibleByDefault) : base(null, name, null, SettingsType.Header, visibleByDefault)
    {
        BackgroundColor = backgroundColor;
        TextColor = textColor;
        CollapsedByDefault = collapsedByDefault;
    }

    /// <summary>
    ///     Collapse this header
    /// </summary>
    public void Collapse()
    {
        Collapse(false);
    }

    /// <summary>
    ///     Collapse this header without animation
    /// </summary>
    /// <param name="skipAnimation">true = skip collapsing animation</param>
    public void Collapse(bool skipAnimation)
    {
        if (HeaderElement == null) return;
        if (skipAnimation)
        {
            HeaderElement.SetHeaderNoAnim(false);
            return;
        }

        HeaderElement.SetHeader(false);
    }

    /// <summary>
    ///     Expand this Header
    /// </summary>
    public void Expand()
    {
        Expand(false);
    }

    /// <summary>
    ///     Expand this Header without animation
    /// </summary>
    /// <param name="skipAnimation">true = skip expanding animation</param>
    public void Expand(bool skipAnimation)
    {
        if (HeaderElement == null) return;
        if (skipAnimation)
        {
            HeaderElement.SetHeaderNoAnim(true);
            return;
        }

        HeaderElement.SetHeader(true);
    }

    /// <summary>
    ///     Change title background color
    /// </summary>
    public void SetBackgroundColor(Color color)
    {
        if (HeaderElement == null) return;
        HeaderElement.HeaderBackground.color = color;
    }

    /// <summary>
    ///     Change title text.
    /// </summary>
    public void SetTextColor(Color color)
    {
        if (HeaderElement == null) return;
        HeaderElement.HeaderTitle.color = color;
    }
}

/// <summary>
///     Settings Text
/// </summary>
public class SettingsText : ModSetting
{
    internal SettingsText(string name, bool visibleByDefault) : base(null, name, null, SettingsType.Text,
        visibleByDefault)
    {
    }

    /// <summary>
    ///     Get Text value
    /// </summary>
    /// <returns>TextBox string value</returns>
    public string GetValue()
    {
        return Name;
    }

    /// <summary>
    ///     Set value for textbox
    /// </summary>
    /// <param name="value">value</param>
    public void SetValue(string value)
    {
        UpdateValue(value);
    }
}

/// <summary>
///     Settings Button
/// </summary>
public class SettingsButton : ModSetting
{
    public enum ButtonIcon
    {
        None = -2,
        Custom = -1,
        RaceDepartment,
        NexusMods,
        Github,
        GoBack,
        Bug,
        Website,
        Folder,
        Download,
        Info,
        Search,
        Settings,
        Warning
    }

    internal Color BackgroundColor = new Color32(85, 38, 0, 255);
    internal Texture2D CustomIcon;
    internal ButtonIcon PredefinedIcon = ButtonIcon.None;
    internal Color TextColor = Color.white;

    internal SettingsButton(string name, Action doAction, Color backgroundColor, Color textColor, bool visibleByDefault,
        ButtonIcon icon, Texture2D customIcon) : base(null, name, doAction, SettingsType.Button, visibleByDefault)
    {
        BackgroundColor = backgroundColor;
        TextColor = textColor;
        PredefinedIcon = icon;
        if (customIcon != null) PredefinedIcon = ButtonIcon.Custom;
        CustomIcon = customIcon;
    }
}

public class SettingsResetButton : ModSetting
{
    internal ModSetting[] SettingsToReset;
    internal Mod ThisMod;

    internal SettingsResetButton(Mod mod, string name, ModSetting[] sets) : base(null, name, null, SettingsType.RButton,
        true)
    {
        ThisMod = mod;
        SettingsToReset = sets;
    }

    internal void ResetSettings()
    {
        if (SettingsToReset == null)
        {
            ModConsole.Error($"[<b>{ThisMod}</b>] SettingsResetButton: no settings to reset");
            return;
        }

        for (var i = 0; i < SettingsToReset.Length; i++) ModMenu.ResetSpecificSetting(SettingsToReset[i]);
        ModMenu.SaveSettings(ThisMod);
    }
}

/// <summary>
///     Settings Dynamic Header
/// </summary>
[Obsolete("Moved to => SettingsHeader", true)]
public class SettingsDynamicHeader : ModSetting
{
    internal Color BackgroundColor = new Color32(95, 34, 18, 255);
    internal bool CollapsedByDefault;
    internal Color TextColor = new Color32(236, 229, 2, 255);


    internal SettingsDynamicHeader(string name, Color backgroundColor, Color textColor, bool collapsedByDefault) : base(
        null, name, null, SettingsType.Header, true)
    {
        BackgroundColor = backgroundColor;
        TextColor = textColor;
        CollapsedByDefault = collapsedByDefault;
    }

    /// <summary>
    ///     Collapse this header
    /// </summary>
    public void Collapse()
    {
        Collapse(false);
    }

    /// <summary>
    ///     Collapse this header without animation
    /// </summary>
    /// <param name="skipAnimation">true = skip collapsing animation</param>
    public void Collapse(bool skipAnimation)
    {
        if (HeaderElement == null) return;
        if (skipAnimation)
        {
            HeaderElement.SetHeaderNoAnim(false);
            return;
        }

        HeaderElement.SetHeader(false);
    }

    /// <summary>
    ///     Expand this Header
    /// </summary>
    public void Expand()
    {
        Expand(false);
    }

    /// <summary>
    ///     Expand this Header without animation
    /// </summary>
    /// <param name="skipAnimation">true = skip expanding animation</param>
    public void Expand(bool skipAnimation)
    {
        if (HeaderElement == null) return;
        if (skipAnimation)
        {
            HeaderElement.SetHeaderNoAnim(true);
            return;
        }

        HeaderElement.SetHeader(true);
    }

    /// <summary>
    ///     Change title background color
    /// </summary>
    public void SetBackgroundColor(Color color)
    {
        if (HeaderElement == null) return;
        HeaderElement.HeaderBackground.color = color;
    }

    /// <summary>
    ///     Change title text.
    /// </summary>
    public void SetTextColor(Color color)
    {
        if (HeaderElement == null) return;
        HeaderElement.HeaderTitle.color = color;
    }
}

/// <summary>
///     Settings Dynamic Text
/// </summary>
[Obsolete("Moved to => SettingsText", true)]
public class SettingsDynamicText : ModSetting
{
    internal SettingsDynamicText(string name) : base(null, name, null, SettingsType.Text, true)
    {
    }

    /// <summary>
    ///     Get Text value
    /// </summary>
    /// <returns>TextBox string value</returns>
    public string GetValue()
    {
        return Name;
    }

    /// <summary>
    ///     Set value for textbox
    /// </summary>
    /// <param name="value">value</param>
    public void SetValue(string value)
    {
        UpdateValue(value);
    }
}

#endif