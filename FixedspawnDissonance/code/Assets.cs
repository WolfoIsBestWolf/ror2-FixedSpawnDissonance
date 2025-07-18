﻿using BepInEx;
using System.IO;
using System.Linq;
using UnityEngine;

namespace VanillaArtifactsPlus
{
    //Taken from EnFucker, Thank you EnFucker
    internal static class Assets
    {
        public static AssetBundle Bundle;
        public static PluginInfo PluginInfo;
        public static string Folder = "ArtifactsVanilla";

        internal static void Init(PluginInfo info)
        {
            PluginInfo = info;

            if (!Directory.Exists(GetPathToFile(Folder)))
            {
                Debug.LogWarning(Folder + " | Folder does not exist");
                Folder = "plugins\\" + Folder;
                if (!Directory.Exists(GetPathToFile(Folder)))
                {
                    Debug.LogWarning(Folder + " | Folder does not exist");
                }
            }

            if (Directory.Exists(GetPathToFile(Folder + "\\Languages")))
            {
                On.RoR2.Language.SetFolders += SetFolders;
            }
            else
            {
                Debug.LogWarning(Folder + "\\Languages | Folder does not exist");
            }
            if (Directory.Exists(GetPathToFile(Folder + "\\AssetBundles")))
            {
                Bundle = AssetBundle.LoadFromFile(GetPathToFile(Folder + "\\AssetBundles", "artifacts_vanilla"));
            }
            else
            {
                Debug.LogWarning(Folder + "\\AssetBundles | Folder does not exist");
            }
        }

        private static void SetFolders(On.RoR2.Language.orig_SetFolders orig, RoR2.Language self, System.Collections.Generic.IEnumerable<string> newFolders)
        {
            var dirs = System.IO.Directory.EnumerateDirectories(Path.Combine(GetPathToFile(Folder + "\\Languages")), self.name);
            orig(self, newFolders.Union(dirs));
        }

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(PluginInfo.Location);
            }
        }
        internal static string GetPathToFile(string folderName)
        {
            return Path.Combine(assemblyDir, folderName);
        }
        internal static string GetPathToFile(string folderName, string fileName)
        {
            return Path.Combine(assemblyDir, folderName, fileName);
        }
    }
}