#if !Mini
using System;
using System.Collections.Generic;
using System.Linq;
using Version = System.Version;

namespace MSCLoader;

internal class RequiredList
{
    public List<string> mods = new();
    public List<string> references = new();
}

internal class ModVersions
{
    public List<MetaVersion> versions = new();
}

internal class MetaVersion
{
    public string mod_id;
    public int mod_rev;
    public byte mod_type;
    public string mod_version;
}

internal class RefVersions
{
    public List<RefVersion> versions = new();
}

internal class RefVersion
{
    public string ref_id;
    public byte ref_type;
    public string ref_version;
}
/*internal class ModsManifest
{
    public string modID;
    // public string version;
    public string description;
    public ManifestLinks links = new ManifestLinks();
    public ManifestIcon icon = new ManifestIcon();
    public ManifestMinReq minimumRequirements = new ManifestMinReq();
    public ManifestModConflict modConflicts = new ManifestModConflict();
    public ManifestModRequired requiredMods = new ManifestModRequired();
    public string sign;
    public string sid_sign;
    public byte type;
    public string msg = null;
    public int rev = 0;

}
internal class ManifestLinks
{
    public string nexusLink = null;
    public string rdLink = null;
    public string githubLink = null;
}
internal class ManifestIcon
{
    public string iconFileName = null;
    public bool isIconRemote = false;
    // public bool isIconUrl = false;
}
internal class ManifestMinReq
{
    public string MSCLoaderVer = null;
    public uint MSCbuildID = 0;
    public bool disableIfVer = false;
}
internal class ManifestModConflict
{
    public string modIDs = null;
    public string customMessage = null;
    public bool disableIfConflict = false;
}
internal class ManifestModRequired
{
    public string modID = null;
    public string minVer = null;
    public string customMessage = null;
}*/

//Metadata V3
public class MSCLData
{
    public string description;
    public string icon;
    public List<string> links = new();
    public MinimumRequirements minimumRequirements = new();
    public ModConflicts modConflicts = new();
    public string modID;
    public string msg;
    public int rev = 0;
    public string sign;
    public int type = 1;
}

public class MinimumRequirements
{
    public bool disableIfVer;
    public int MSCbuildID = 0;
    public string MSCLoaderVer;
}

public class ModConflicts
{
    public string customMessage;
    public bool disableIfConflict = false;
    public List<string> modIDs = new();
}

internal class ModMetadata
{
    internal static void ReadUpdateInfo(ModVersions mv)
    {
        ModLoader.ModSelfUpdateList = new List<string>();
        ModLoader.Instance.MetadataUpdateList = new List<string>();
        ModLoader.HasUpdateModList = new List<Mod>();
        if (mv == null) return;

        for (var i = 0; i < mv.versions.Count; i++)
            try
            {
                var mod = ModLoader.GetModByID(mv.versions[i].mod_id, true);
                if (mod == null) continue;
                mod.UpdateInfo = mv.versions[i];
                if (mv.versions[i].mod_type == 2 || mv.versions[i].mod_type == 9)
                {
                    mod.isDisabled = true;
                    ModMenu.SaveSettings(mod);
                }

                var v1 = new Version(mv.versions[i].mod_version);
                var v2 = new Version(mod.Version);
                switch (v1.CompareTo(v2))
                {
                    case 1:
                        mod.hasUpdate = true;
                        ModLoader.HasUpdateModList.Add(mod);
                        if (mv.versions[i].mod_type == 4 || mv.versions[i].mod_type == 6)
                            ModLoader.ModSelfUpdateList.Add(mod.ID);
                        break;
                    case -1:
                        if (mv.versions[i].mod_type == 6)
                        {
                            mod.hasUpdate = true;
                            ModLoader.ModSelfUpdateList.Add(mod.ID);
                            ModLoader.HasUpdateModList.Add(mod);
                        }

                        break;
                }

                if (mod.metadata != null)
                {
                    if (mod.metadata.rev != mv.versions[i].mod_rev) ModLoader.Instance.MetadataUpdateList.Add(mod.ID);
                }
                else
                {
                    ModLoader.Instance.MetadataUpdateList.Add(mod.ID);
                }
            }
            catch (Exception e)
            {
                ModConsole.Error($"Failed to read update info for mod {mv.versions[i].mod_id}");
                ModConsole.Error($"{e.Message}");
                Console.WriteLine(e);
            }

        ModMenu.instance.UI.transform.GetChild(0).GetComponent<ModMenuView>().RefreshTabs();
    }

    internal static void ReadRefUpdateInfo(RefVersions mv)
    {
        ModLoader.RefSelfUpdateList = new List<string>();
        ModLoader.HasUpdateRefList = new List<References>();
        if (mv == null) return;

        for (var i = 0; i < mv.versions.Count; i++)
            try
            {
                var refs = ModLoader.Instance.ReferencesList.Where(x => x.AssemblyID.Equals(mv.versions[i].ref_id))
                    .FirstOrDefault();
                if (refs == null) continue;
                refs.UpdateInfo = mv.versions[i];
                var v1 = new Version(mv.versions[i].ref_version);
                var v2 = new Version(refs.AssemblyFileVersion);
                switch (v1.CompareTo(v2))
                {
                    case 1:
                        ModLoader.HasUpdateRefList.Add(refs);
                        if (mv.versions[i].ref_type == 1) ModLoader.RefSelfUpdateList.Add(refs.AssemblyID);
                        break;
                }
            }
            catch (Exception e)
            {
                ModConsole.Error($"Failed to read update info for reference {mv.versions[i].ref_id}");
                ModConsole.Error($"{e.Message}");
                Console.WriteLine(e);
            }

        ModMenu.instance.UI.transform.GetChild(0).GetComponent<ModMenuView>().RefreshTabs();
    }

    /* internal static void LoadMetadata(Mod mod)
     {
         string metadataFile = ModLoader.GetMetadataFolder($"{mod.ID}.json");
         if (File.Exists(metadataFile))
         {
             string s = File.ReadAllText(metadataFile);
             return JsonConvert.DeserializeObject<MSCLData>(s);
         }
         return null;
     }*/
}
#endif