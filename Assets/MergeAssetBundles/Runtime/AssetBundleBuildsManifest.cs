using System;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;

namespace AssetBundleMerger
{
    [Serializable]
    public class AssetBundleBuildsManifest
    {
        [SerializeField] 
        public int ManifestFileVersion = 1;
        [SerializeField] 
        public uint CRC = 4271548926;
        [SerializeField] 
        public YamlAssetBundleManifestInfoContainer AssetBundleManifest = null;
        
        [NonSerialized, YamlIgnore]
        private List<string> allAssetBundles = new List<string>();
        [NonSerialized, YamlIgnore]
        private Dictionary<string, string[]> allDependencies = new Dictionary<string, string[]>();
        [NonSerialized, YamlIgnore]
        private Dictionary<string, Hash128> allHash = new Dictionary<string, Hash128>();

        public string[] GetAllAssetBundles()
        {
            return allAssetBundles.ToArray();
        }
        public string[] GetAllDependencies(string name)
        {
            string[] val = null;
            allDependencies.TryGetValue(name, out val);
            return val;
        }
        public Hash128 GetAssetBundleHash(string name)
        {
            Hash128 hash = new Hash128();
            allHash.TryGetValue(name, out hash);
            return hash;
        }

        public void AddUnityManifest(YamlAssetBundleManifest manifest)
        {
            if (manifest == null) { return; }
            string[] inOriginList = manifest.GetAllAssetBundles();
            
            foreach (var origin in inOriginList)
            {
                if (allHash.ContainsKey(origin))
                {
                    continue;
                }

                allAssetBundles.Add(origin);
                allDependencies.Add(origin, manifest.GetAllDependencies(origin));
                allHash.Add(origin, manifest.GetAssetBundleHash(origin));
            }
        }

        public void BuildManifest()
        {
            AssetBundleManifest = new YamlAssetBundleManifestInfoContainer();
            foreach (var assetBundle in allAssetBundles)
            {
                var newOne = new YamlAssetBundleManifestInfo();
                newOne.Name = assetBundle;
                newOne.Dependencies.AddRange(allDependencies[assetBundle]);
                AssetBundleManifest.AssetBundleInfos.Add(newOne);
            }
        }
    }
}