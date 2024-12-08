#if !Mini
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace MSCLoader;

internal class MSCLInternal
{
    internal static bool AsyncRequestInProgress;
    internal static bool AsyncRequestError;
    internal static string AsyncRequestResult = string.Empty;

    internal static bool ValidateVersion(string version)
    {
        try
        {
            new Version(version);
        }
        catch
        {
            ModConsole.Error(
                $"Invalid version: {version}{Environment.NewLine}Please use proper version format: (0.0 or 0.0.0 or 0.0.0.0)");
            return false;
        }

        return true;
    }

    internal static string MSCLDataRequest(string reqPath, NameValueCollection msclData)
    {
        var response = "";
        using (var MSCLDconn = new WebClient())
        {
            MSCLDconn.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
            try
            {
                var raw = MSCLDconn.UploadValues($"{ModLoader.serverURL}/{reqPath}", "POST", msclData);
                response = Encoding.UTF8.GetString(raw, 0, raw.Length);
            }
            catch (Exception e)
            {
                ModConsole.Error($"Request failed with error: {e.Message}");
                Console.WriteLine(e);
                response = "error";
            }
        }
#if DEBUG
        ModConsole.Warning(response);
#endif
        return response;
    }


    internal static string MSCLDataRequest(string reqPath, Dictionary<string, string> data)
    {
        var msclData = new NameValueCollection { { "msclData", JsonConvert.SerializeObject(data) } };
        return MSCLDataRequest(reqPath, msclData);
    }

    internal static string MSCLDataRequest(string reqPath, Dictionary<string, List<string>> data)
    {
        var msclData = new NameValueCollection { { "msclData", JsonConvert.SerializeObject(data) } };
        return MSCLDataRequest(reqPath, msclData);
    }

    internal static void MSCLRequestAsync(string reqPath, NameValueCollection msclData)
    {
        AsyncRequestInProgress = true;
        using (var webClient = new WebClient())
        {
            webClient.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
            webClient.UploadValuesCompleted += ModsUpdateData;
            webClient.UploadProgressChanged += ModsUpdateDataProgress;
            webClient.UploadValuesAsync(new Uri($"{ModLoader.serverURL}/{reqPath}"), "POST", msclData);
        }
    }

    private static void ModsUpdateDataProgress(object sender, UploadProgressChangedEventArgs e)
    {
        AsyncRequestInProgress = true;
    }

    private static void ModsUpdateData(object sender, UploadValuesCompletedEventArgs e)
    {
        AsyncRequestInProgress = false;
        if (e.Error != null)
        {
            ModConsole.Error("Failed to check for mods updates");
            ModConsole.Error(e.Error.Message);
            Console.WriteLine(e.Error);
            AsyncRequestError = true;
        }
        else
        {
            AsyncRequestResult = Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
        }
    }

    internal static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        var dir = new DirectoryInfo(sourceDirName);
        if (!dir.Exists)
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        var dirs = dir.GetDirectories();

        Directory.CreateDirectory(destDirName);

        var files = dir.GetFiles();
        foreach (var file in files)
        {
            var tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
        }

        if (copySubDirs)
            foreach (var subdir in dirs)
            {
                var tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
            }
    }

    internal static void SaveMSCLDataFile()
    {
        if (!ES2.Exists(ModLoader.GetMetadataFolder("MSCLData.bin")))
            ES2.Save(new byte[1] { 0x01 }, $"{ModLoader.GetMetadataFolder("MSCLData.bin")}?tag=MSCLData");
        using (var writer = ES2Writer.Create(ModLoader.GetMetadataFolder("MSCLData.bin")))
        {
            var config = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            for (var i = 0; i < ModLoader.Instance.MetadataUpdateList.Count; i++)
            {
                var mod = ModLoader.GetModByID(ModLoader.Instance.MetadataUpdateList[i], true);
                if (mod.metadata == null) continue;
                var serializedData = JsonConvert.SerializeObject(mod.metadata, config);
                var bytes = Encoding.UTF8.GetBytes(serializedData);
                writer.Write(bytes, $"{mod.ID}||metadata");
            }

            writer.Save();
        }
    }

    internal static void SaveMSCLDataFile(Mod mod)
    {
        if (!ES2.Exists(ModLoader.GetMetadataFolder("MSCLData.bin")))
            ES2.Save(new byte[1] { 0x01 }, $"{ModLoader.GetMetadataFolder("MSCLData.bin")}?tag=MSCLData");
        if (mod.metadata == null) return;
        var config = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.None
        };
        var serializedData = JsonConvert.SerializeObject(mod.metadata, config);
        var bytes = Encoding.UTF8.GetBytes(serializedData);

        ES2.Save(bytes, $"{ModLoader.GetMetadataFolder("MSCLData.bin")}?tag={mod.ID}||metadata");
    }

    internal static void LoadMSCLDataFile()
    {
        if (!ES2.Exists(ModLoader.GetMetadataFolder("MSCLData.bin")))
        {
            ES2.Save(new byte[1] { 0x01 }, $"{ModLoader.GetMetadataFolder("MSCLData.bin")}?tag=MSCLData");
            var oldm = Directory.GetFiles(ModLoader.GetMetadataFolder(""), "*.json");
            if (oldm.Length > 0)
                for (var i = 0; i < oldm.Length; i++)
                    File.Delete(oldm[i]);

            return;
        }

        using (var reader = ES2Reader.Create(ModLoader.GetMetadataFolder("MSCLData.bin")))
        {
            for (var i = 0; i < ModLoader.Instance.actualModList.Length; i++)
            {
                var mod = ModLoader.Instance.actualModList[i];
                if (!reader.TagExists($"{mod.ID}||metadata")) continue;
                var bytes = reader.ReadArray<byte>($"{mod.ID}||metadata");
                var serializedData = Encoding.UTF8.GetString(bytes);
                mod.metadata = JsonConvert.DeserializeObject<MSCLData>(serializedData);
            }
        }
    }

    internal static bool MSCLDataExists(string modID)
    {
        return ES2.Exists($"{ModLoader.GetMetadataFolder("MSCLData.bin")}?tag={modID}||metadata");
    }

    internal static bool IsEAFile(string path)
    {
        var bytes = new byte[3];
        try
        {
            using (var reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                reader.Read(bytes, 0, 3);
                reader.Close();
                return bytes[0] == 0x45 && bytes[1] == 0x41 && bytes[2] == 0x4D;
            }
        }
        catch (Exception ex)
        {
            ModConsole.Error(ex.Message);
            return false;
        }
    }
}

//GPL-3 licensed ByteArrayExtensions by Bruno Tabbia
internal static class ByteArrayExtensions
{
    private const int bitsinbyte = 8;

    public static byte[] Cry_ScrambleByteRightEnc(this byte[] cleardata, byte[] password)
    {
        var cdlen = cleardata.LongLength;
        var cryptdata = new byte[cdlen];
        // first loop: fill crypt array with bytes from cleardata 
        // corresponding to the '1' in passwords bit
        long ci = 0;
        for (var b = cdlen - 1; b >= 0; b--)
            if (password.GetBitR(b))
            {
                cryptdata[ci] = cleardata[b];
                ci++;
            }

        // second loop: fill crypt array with bytes from cleardata 
        // corresponding to the '0' in passwords bit
        for (var b = cdlen - 1; b >= 0; b--)
            if (!password.GetBitR(b))
            {
                cryptdata[ci] = cleardata[b];
                ci++;
            }

        return cryptdata;
    }

    public static byte[] Cry_ScrambleByteRightDec(this byte[] cryptdata, byte[] password)
    {
        var cdlen = cryptdata.LongLength;
        var cleardata = new byte[cdlen];
        long ci = 0;
        for (var b = cdlen - 1; b >= 0; b--)
            if (password.GetBitR(b))
            {
                cleardata[b] = cryptdata[ci];
                ci++;
            }

        for (var b = cdlen - 1; b >= 0; b--)
            if (!password.GetBitR(b))
            {
                cleardata[b] = cryptdata[ci];
                ci++;
            }

        return cleardata;
    }

    // --------------------------------------------------------------------------------------

    public static byte[] Cry_ScrambleBitRightEnc(this byte[] cleardata, byte[] password)
    {
        var cdlen = cleardata.LongLength;
        var cryptdata = new byte[cdlen];
        // first loop: fill crypt array with bits from cleardata 
        // corresponding to the '1' in passwords bit
        long ci = 0;

        for (var b = cdlen * bitsinbyte - 1; b >= 0; b--)
            if (password.GetBitR(b))
            {
                SetBitR(cryptdata, ci, cleardata.GetBitR(b));
                ci++;
            }

        // second loop: fill crypt array with bits from cleardata 
        // corresponding to the '0' in passwords bit
        for (var b = cdlen * bitsinbyte - 1; b >= 0; b--)
            if (!password.GetBitR(b))
            {
                SetBitR(cryptdata, ci, cleardata.GetBitR(b));
                ci++;
            }

        return cryptdata;
    }

    public static byte[] Cry_ScrambleBitRightDec(this byte[] cryptdata, byte[] password)
    {
        var cdlen = cryptdata.LongLength;
        var cleardata = new byte[cdlen];
        long ci = 0;

        for (var b = cdlen * bitsinbyte - 1; b >= 0; b--)
            if (password.GetBitR(b))
            {
                SetBitR(cleardata, b, cryptdata.GetBitR(ci));
                ci++;
            }

        for (var b = cdlen * bitsinbyte - 1; b >= 0; b--)
            if (!password.GetBitR(b))
            {
                SetBitR(cleardata, b, cryptdata.GetBitR(ci));
                ci++;
            }

        return cleardata;
    }

    // -----------------------------------------------------------------------------------

    public static bool GetBitR(this byte[] bytearray, long bit)
    {
        return ((bytearray[bit / bitsinbyte % bytearray.LongLength] >>
                 ((int)bit % bitsinbyte)) & 1) == 1;
    }

    public static void SetBitR(byte[] bytearray, long bit, bool set)
    {
        var bytepos = bit / bitsinbyte;
        if (bytepos < bytearray.LongLength)
        {
            var bitpos = (int)bit % bitsinbyte;
            byte adder;
            if (set)
            {
                adder = (byte)(1 << bitpos);
                bytearray[bytepos] = (byte)(bytearray[bytepos] | adder);
            }
            else
            {
                adder = (byte)(byte.MaxValue ^ (byte)(1 << bitpos));
                bytearray[bytepos] = (byte)(bytearray[bytepos] & adder);
            }
        }
    }
}

internal class InvalidMods
{
    //If isManaged
    public List<string> AdditionalRefs = new();
    public string AsmGuid;
    public string ErrorMessage;
    public string FileName;
    public bool IsManaged;

    internal InvalidMods(string fileName, bool isManaged, string errorMessage)
    {
        FileName = fileName;
        IsManaged = isManaged;
        ErrorMessage = errorMessage;
    }

    internal InvalidMods(string fileName, bool isManaged, string errorMessage, List<string> additionalRefs,
        string asmGuid)
    {
        FileName = fileName;
        IsManaged = isManaged;
        ErrorMessage = errorMessage;
        AdditionalRefs = additionalRefs;
        AsmGuid = asmGuid;
    }
}
#endif