﻿#if !Mini
using System;

namespace MSCLoader;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[Obsolete("This class requires user to have 'Compatibility References' installed")]
public class ModSave
{
    [Obsolete("This requires user to have 'Compatibility References' installed")]
    public static void Save<T>(string fileName, T data, string encryptionKey = null) where T : class, new()
    {
        MSCLoaderHelpers.ModSave.Save(fileName, data, encryptionKey);
    }

    [Obsolete("This requires user to have 'Compatibility References' installed")]
    public static T Load<T>(string fileName, string encryptionKey = "") where T : class, new()
    {
        return MSCLoaderHelpers.ModSave.Load<T>(fileName, encryptionKey);
    }

    [Obsolete("This requires user to have 'Compatibility References' installed")]
    public static void Delete(string fileName)
    {
        MSCLoaderHelpers.ModSave.Delete(fileName);
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
#endif