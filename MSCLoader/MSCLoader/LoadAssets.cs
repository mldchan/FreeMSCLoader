﻿#if !Mini
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MSCLoader;

/// <summary>
///     Class for Loading custom assets from Assets folder
/// </summary>
public static class LoadAssets
{
    internal static List<string> assetNames = new();

    /// <summary>
    ///     Make GameObject Pickable, make sure your GameObject has Rigidbody and colliders attached.
    /// </summary>
    /// <param name="go">Your GameObject</param>
    public static void MakeGameObjectPickable(GameObject go)
    {
        go.layer = LayerMask.NameToLayer("Parts");
        go.tag = "PART";
    }

    /// <summary>
    ///     Load texture (*.dds, *.jpg, *.png, *.tga) from mod assets folder
    /// </summary>
    /// <param name="mod">Mod instance.</param>
    /// <param name="fileName">File name to load from assets folder (for example "texture.dds")</param>
    /// <param name="normalMap">Normal mapping (default false)</param>
    /// <returns>Returns unity Texture2D</returns>
    public static Texture2D LoadTexture(Mod mod, string fileName, bool normalMap = false)
    {
        var fn = Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName);

        if (!File.Exists(fn))
            throw new FileNotFoundException($"<b>LoadTexture() Error:</b> File not found: {fn}{Environment.NewLine}",
                fn);
        var ext = Path.GetExtension(fn).ToLower();
        if (ext == ".png" || ext == ".jpg")
        {
            var t2d = new Texture2D(1, 1);
            t2d.LoadImage(File.ReadAllBytes(fn));
            return t2d;
        }

        if (ext == ".dds")
        {
            var returnTex = LoadDDS(fn);
            return returnTex;
        }

        if (ext == ".tga")
        {
            var returnTex = LoadTGA(fn);
            return returnTex;
        }

        throw new NotSupportedException(
            $"<b>LoadTexture() Error:</b> Texture not supported: {fileName}{Environment.NewLine}");
    }

    /// <summary>
    ///     Load (*.obj) file from mod assets folder and return as GameObject
    /// </summary>
    /// <param name="mod">Mod instance.</param>
    /// <param name="fileName">File name to load from assets folder (for example "beer.obj")</param>
    /// <param name="collider">Apply mesh collider to object</param>
    /// <param name="rigidbody">Apply rigidbody to object to affect gravity (don't do it without collider)</param>
    /// <returns>Returns unity GameObject</returns>
    [Obsolete("LoadOBJ is deprecated, please use AssetBundles instead.", true)]
    public static GameObject LoadOBJ(Mod mod, string fileName, bool collider = true, bool rigidbody = false)
    {
        var mesh = LoadOBJMesh(mod, fileName);
        if (mesh != null)
        {
            var obj = new GameObject();
            obj.AddComponent<MeshFilter>().mesh = mesh;
            obj.AddComponent<MeshRenderer>();
            if (rigidbody)
                obj.AddComponent<Rigidbody>();
            if (collider)
            {
                if (rigidbody)
                    obj.AddComponent<MeshCollider>().convex = true;
                else
                    obj.AddComponent<MeshCollider>();
            }

            return obj;
        }

        return null;
    }

    /// <summary>
    ///     Load (*.obj) file from mod assets folder and return as Mesh
    /// </summary>
    /// <param name="mod">Mod instance.</param>
    /// <param name="fileName">File name to load from assets folder (for example "beer.obj")</param>
    /// <returns>Returns unity Mesh</returns>
    [Obsolete("LoadOBJMesh is deprecated, please use AssetBundles instead.", true)]
    public static Mesh LoadOBJMesh(Mod mod, string fileName)
    {
        var fn = Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName);
        if (!File.Exists(fn))
            throw new FileNotFoundException($"<b>LoadOBJ() Error:</b> File not found: {fn}{Environment.NewLine}", fn);
        var ext = Path.GetExtension(fn).ToLower();
        if (ext == ".obj")
        {
            var obj = new OBJLoader();
            var mesh = obj.ImportFile(Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName));
            mesh.name = Path.GetFileNameWithoutExtension(fn);
            return mesh;
        }

        throw new NotSupportedException(
            $"<b>LoadOBJ() Error:</b> Only (*.obj) files are supported{Environment.NewLine}");
    }


    /// <summary>
    ///     Loads assetbundle from Assets folder
    /// </summary>
    /// <param name="mod">Mod instance.</param>
    /// <param name="bundleName">File name to load (for example "something.unity3d")</param>
    /// <returns>Unity AssetBundle</returns>
    public static AssetBundle LoadBundle(Mod mod, string bundleName)
    {
        var bundle = Path.Combine(ModLoader.GetModAssetsFolder(mod), bundleName);
        if (File.Exists(bundle))
        {
            ModConsole.Print($"Loading Asset: {bundleName}...");
            var ab = AssetBundle.CreateFromMemoryImmediate(File.ReadAllBytes(bundle));
            var array = ab.GetAllAssetNames();
            for (var i = 0; i < array.Length; i++) assetNames.Add(Path.GetFileNameWithoutExtension(array[i]));
            return ab;
        }

        throw new FileNotFoundException(
            $"<b>LoadBundle() Error:</b> File not found: <b>{bundle}</b>{Environment.NewLine}", bundleName);
    }

    /// <summary>
    ///     Loads assetbundle from Resources
    /// </summary>
    /// <param name="assetBundleFromResources">Resource path</param>
    /// <returns>Unity AssetBundle</returns>
    public static AssetBundle LoadBundle(byte[] assetBundleFromResources)
    {
        if (assetBundleFromResources != null)
        {
            var ab = AssetBundle.CreateFromMemoryImmediate(assetBundleFromResources);
            var array = ab.GetAllAssetNames();
            for (var i = 0; i < array.Length; i++) assetNames.Add(Path.GetFileNameWithoutExtension(array[i]));
            return ab;
        }

        throw new Exception($"<b>LoadBundle() Error:</b> Resource doesn't exists{Environment.NewLine}");
    }

    /// <summary>
    ///     Loads assetbundle from Embedded Resources
    /// </summary>
    /// <param name="assetBundleEmbeddedResources">Resource path namespace.folder.file.extension</param>
    /// <returns>Unity AssetBundle</returns>
    public static AssetBundle LoadBundle(string assetBundleEmbeddedResources)
    {
        var a = Assembly.GetCallingAssembly();
        using (var resFilestream = a.GetManifestResourceStream(assetBundleEmbeddedResources))
        {
            if (resFilestream == null)
                throw new Exception($"<b>LoadBundle() Error:</b> Resource doesn't exists{Environment.NewLine}");

            var ba = new byte[resFilestream.Length];
            resFilestream.Read(ba, 0, ba.Length);
            var ab = AssetBundle.CreateFromMemoryImmediate(ba);
            var array = ab.GetAllAssetNames();
            for (var i = 0; i < array.Length; i++) assetNames.Add(Path.GetFileNameWithoutExtension(array[i]));
            return ab;
        }
    }

    // TGALoader by https://gist.github.com/mikezila/10557162
    internal static Texture2D LoadTGA(string fileName)
    {
        using (var imageFile = File.OpenRead(fileName))
        {
            return LoadTGA(imageFile);
        }
    }

    //DDS loader based on https://raw.githubusercontent.com/hobbitinisengard/crashday-3d-editor/7e7c6c78c9f67588156787af1af92cfad1019de9/Assets/IO/DDSDecoder.cs
    internal static Texture2D LoadDDS(string ddsPath)
    {
        try
        {
            var ddsBytes = File.ReadAllBytes(ddsPath);

            var ddsSizeCheck = ddsBytes[4];
            if (ddsSizeCheck != 124)
                throw new Exception(
                    "Invalid DDS DXTn texture. Unable to read"); //header byte should be 124 for DDS image files

            var height = ddsBytes[13] * 256 + ddsBytes[12];
            var width = ddsBytes[17] * 256 + ddsBytes[16];

            var DXTType = ddsBytes[87];
            var textureFormat = TextureFormat.DXT5;
            if (DXTType == 49) textureFormat = TextureFormat.DXT1;

            if (DXTType == 53) textureFormat = TextureFormat.DXT5;
            var DDS_HEADER_SIZE = 128;
            var dxtBytes = new byte[ddsBytes.Length - DDS_HEADER_SIZE];
            Buffer.BlockCopy(ddsBytes, DDS_HEADER_SIZE, dxtBytes, 0, ddsBytes.Length - DDS_HEADER_SIZE);

            var finf = new FileInfo(ddsPath);
            var texture = new Texture2D(width, height, textureFormat, false);
            texture.LoadRawTextureData(dxtBytes);
            texture.Apply();
            texture.name = finf.Name;

            return texture;
        }
        catch (Exception ex)
        {
            ModConsole.Error($"<b>LoadTexture() Error:</b>{Environment.NewLine}Error: Could not load DDS texture");
            if (ModLoader.devMode)
                ModConsole.Error(ex.ToString());
            Console.WriteLine(ex);
            return new Texture2D(8, 8);
        }
    }

    // TGALoader by https://gist.github.com/mikezila/10557162
    private static Texture2D LoadTGA(Stream TGAStream)
    {
        using (var r = new BinaryReader(TGAStream))
        {
            r.BaseStream.Seek(12, SeekOrigin.Begin);

            var width = r.ReadInt16();
            var height = r.ReadInt16();
            int bitDepth = r.ReadByte();
            r.BaseStream.Seek(1, SeekOrigin.Current);

            var tex = new Texture2D(width, height);
            var pulledColors = new Color32[width * height];

            if (bitDepth == 32)
                for (var i = 0; i < width * height; i++)
                {
                    var red = r.ReadByte();
                    var green = r.ReadByte();
                    var blue = r.ReadByte();
                    var alpha = r.ReadByte();

                    pulledColors[i] = new Color32(blue, green, red, alpha);
                }
            else if (bitDepth == 24)
                for (var i = 0; i < width * height; i++)
                {
                    var red = r.ReadByte();
                    var green = r.ReadByte();
                    var blue = r.ReadByte();

                    pulledColors[i] = new Color32(blue, green, red, 1);
                }
            else
                throw new Exception(
                    $"<b>LoadTexture() Error:</b> TGA texture is not 32 or 24 bit depth.{Environment.NewLine}");

            tex.SetPixels32(pulledColors);
            tex.Apply();
            return tex;
        }
    }
}
#endif