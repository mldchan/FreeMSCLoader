#if !Mini
using System;
using System.IO;

namespace MSCLoader;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[Obsolete("Same exact shit as LoadAssets")]
public static class ModAssets
{
    [Obsolete("=> LoadAssets.LoadBundle()", true)]
    public static AssetBundle LoadBundle(byte[] bundleBytes)
    {
        return AssetBundle.CreateFromMemoryImmediate(bundleBytes);
    }

    [Obsolete("=> LoadAssets.LoadBundle()", true)]
    public static AssetBundle LoadBundle(string filePath)
    {
        if (File.Exists(filePath))
            return AssetBundle.CreateFromMemoryImmediate(File.ReadAllBytes(filePath));
        throw new FileNotFoundException($"<b>LoadBundle() Error:</b> No AssetBundle file found at path: {filePath}");
    }

    [Obsolete("=> LoadAssets.LoadBundle()", true)]
    public static AssetBundle LoadBundle(Mod mod, string bundleName)
    {
        return LoadAssets.LoadBundle(mod, bundleName);
    }

    [Obsolete("=> LoadAssets.LoadTexture()", true)]
    public static Texture2D LoadTexture(Mod mod, string textureName, bool normalMap = false)
    {
        return LoadAssets.LoadTexture(mod, textureName, normalMap);
    }

    [Obsolete("=> LoadAssets.LoadTexture()", true)]
    public static Texture2D LoadTexture(string filePath, bool normalMap = false)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"<b>LoadTexture() Error:</b> File not found: {filePath}", filePath);

        var fileExtension = Path.GetExtension(filePath).ToLower();

        switch (fileExtension)
        {
            case ".jpg":
                return LoadTextureJPG(filePath, normalMap);
            case ".png":
                return LoadTexturePNG(filePath, normalMap);
            case ".dds":
                return LoadTextureDDS(filePath, normalMap);
            case ".tga":
                return LoadTextureTGA(filePath, normalMap);
            default:
                throw new NotSupportedException(
                    $"<b>LoadTexture() Error:</b> File {fileExtension} not supported as a texture: {filePath}");
        }
    }

    [Obsolete("=> LoadAssets.LoadTexture()", true)]
    public static Texture2D LoadTexturePNG(string filePath, bool normalMap = false)
    {
        var t2d = new Texture2D(1, 1);
        t2d.LoadImage(File.ReadAllBytes(filePath));
        return t2d;
    }

    [Obsolete("=> LoadAssets.LoadTexture()", true)]
    public static Texture2D LoadTextureJPG(string filePath, bool normalMap = false)
    {
        return LoadTexturePNG(filePath, normalMap);
    }

    [Obsolete("=> LoadAssets.LoadTexture()", true)]
    public static Texture2D LoadTextureDDS(string filePath, bool normalMap = false)
    {
        return LoadAssets.LoadDDS(filePath);
    }

    [Obsolete("=> LoadAssets.LoadTexture()", true)]
    public static Texture2D LoadTextureTGA(string filePath, bool normalMap = false)
    {
        return LoadAssets.LoadTGA(filePath);
    }

    [Obsolete("=> LoadAssets.LoadOBjMesh()", true)]
    public static Mesh LoadMeshOBJ(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException(
                $"<b>LoadMeshOBJ() Error:</b> File not found: {filePath}{Environment.NewLine}", filePath);
        var ext = Path.GetExtension(filePath).ToLower();
        if (ext == ".obj")
        {
            var obj = new OBJLoader();
            var mesh = obj.ImportFile(filePath);
            mesh.name = Path.GetFileNameWithoutExtension(filePath);
            return mesh;
        }

        throw new NotSupportedException(
            $"<b>LoadMeshOBJ() Error:</b> Only (*.obj) files are supported{Environment.NewLine}");
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#endif