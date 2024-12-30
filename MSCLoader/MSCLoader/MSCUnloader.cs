#if !Mini
using System;
using System.Collections.Generic;
using System.Linq;

// Unload All changes when back to main menu
// Just destroy everything (without this script) and load again.
// No need to reset game for some mods.
namespace MSCLoader;

internal class MSCUnloader : MonoBehaviour
{
    internal static Queue<string> dm_pcon;
    private bool doReset;
    internal bool reset;

    private void Update()
    {
        if (doReset && !Application.isLoadingLevel) //if menu is fully loaded.
        {
            var gos = FindObjectsOfType<GameObject>();
            for (var i = 0; i < gos.Length; i++)
            {
                if (gos[i].name == "MSCUnloader")
                    continue;
                Destroy(gos[i]);
            }

            var gosAll = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(x => !x.activeInHierarchy && x.transform.parent == null).ToArray();
            for (var i = 0; i < gos.Length; i++)
                if (LoadAssets.assetNames.Contains(gosAll[i].name.ToLower()))
                    Destroy(gosAll[i]);

            PlayMakerGlobals.Instance.Variables.FindFsmBool("SongImported").Value = false; //stupid variable name.

            ModLoader.unloader = false;
            ModLoader.returnToMainMenu = true;
            ModLoader.loaderPrepared = false;
            ModLoader.initCalled = false;
            Application.LoadLevel(Application.loadedLevelName);
            doReset = false;
        }
    }

    internal void MSCLoaderReset()
    {
        if (!reset) //to avoid endless loop
        {
            if (ModLoader.devMode && ModMenu.dm_pcon.GetValue())
            {
                dm_pcon = new Queue<string>(ModConsole.console.controller.scrollback);
                dm_pcon.Enqueue($"{Environment.NewLine}{Environment.NewLine}");
            }

            reset = true;
            doReset = true;
        }
    }
}
#endif