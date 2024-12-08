﻿#if !Mini
using System;
using System.ComponentModel;
using UnityEngine.Events;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MSCLoader;

[EditorBrowsable(EditorBrowsableState.Never)]
[Obsolete("Nothing", true)]
public class ModPromptButton
{
}

[EditorBrowsable(EditorBrowsableState.Never)]
[Obsolete("=> ModUI", true)]
public class ModPrompt
{
    [Obsolete("=> ModUI", true)]
    public ModPromptButton AddButton(string buttonText, UnityAction action)
    {
        return null;
    }

    [Obsolete("=> ModUI.ShowMessage", true)]
    public static ModPrompt CreatePrompt(string message, string title = "MESSAGE", UnityAction onPromptClose = null)
    {
        ModUI.ShowMessage(message);
        return null;
    }

    [Obsolete("=> ModUI.ShowYesNoMessage", true)]
    public static ModPrompt CreateYesNoPrompt(string message, string title, UnityAction onYes, UnityAction onNo = null,
        UnityAction onPromptClose = null)
    {
        ModUI.ShowYesNoMessage(message, title, null, onYes);
        return null;
    }

    [Obsolete("=> ModUI.ShowRetryCancelMessage", true)]
    public static ModPrompt CreateRetryCancelPrompt(string message, string title, UnityAction onRetry,
        UnityAction onCancel = null, UnityAction onPromptClose = null)
    {
        ModUI.ShowRetryCancelMessage(message, title, null, onRetry);
        return null;
    }

    [Obsolete("=> ModUI.ShowContinueAbortMessage", true)]
    public static ModPrompt CreateContinueAbortPrompt(string message, string title, UnityAction onContinue,
        UnityAction onAbort = null, UnityAction onPromptClose = null)
    {
        ModUI.ShowContinueAbortMessage(message, title, null, onContinue);
        return null;
    }

    [Obsolete("=> ModUI.ShowCustomMessage", true)]
    public static ModPrompt CreateCustomPrompt()
    {
        return null;
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#endif