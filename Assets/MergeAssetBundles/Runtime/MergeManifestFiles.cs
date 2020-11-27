using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace AssetBundleMerger
{
    [Serializable]
    public class YamlAssetBundleManifest
    {
        public int ManifestFileVersion = 0;
        public uint CRC = 4271548926;
        public YamlAssetBundleManifestInfoContainer AssetBundleManifest = new YamlAssetBundleManifestInfoContainer();
        
        public Hash128 GetAssetBundleHash(string name)
        {
            return Hash128.Parse(name);
        }
        public string[] GetAllAssetBundles()
        {
            List<string> result = new List<string>();
            foreach (var info in AssetBundleManifest.AssetBundleInfos)
            {
                result.Add(info.Name);
            }
            return result.ToArray();
        }

        public string[] GetAllDependencies(string bundle)
        {
            var findBundleAndDependency = 
                AssetBundleManifest.AssetBundleInfos
                    .Find((another) =>
                    {
                        return another.Name.ToLowerInvariant().Equals(bundle.ToLowerInvariant());
                    });
            string[] depends = new string[]{};
            if (findBundleAndDependency != null && findBundleAndDependency.Dependencies != null)
            {
                depends =  findBundleAndDependency.Dependencies.ToArray();
            }
            return depends;
        }
    }

    [Serializable]
    public class YamlAssetBundleManifestInfoContainer
    {
        public List<YamlAssetBundleManifestInfo> AssetBundleInfos = new List<YamlAssetBundleManifestInfo>();
    }
    
    [Serializable]
    public class YamlAssetBundleManifestInfo 
    {
        public string Name;
        // serialized name of field should end with index []
        public List<string> Dependencies = new List<string>();
        
    }

    public class MergeManifestFiles : MonoBehaviour
    {
        [FormerlySerializedAs("FUllPathTOBundles")] 
        [SerializeField] 
        private List<string> FullPathToManifests = new List<string>();
        [SerializeField] 
        private string outputDir;
        
        
        [ContextMenu("Merge")]
        private void Merge()
        {
            AssetBundleBuildsManifest mergedManifest = new AssetBundleBuildsManifest();
            AssetBundleManifest unityManifest = null;
            
            var allManifests = new List<YamlAssetBundleManifest>();
            
            foreach (var varGetFrom in FullPathToManifests)
            {
                var any = LoadManifestFromFolder(varGetFrom);
                if (any != null)
                {
                    allManifests.Add(any);
                }
            }
            
            foreach (var manifest in allManifests)
            {
                mergedManifest.AddUnityManifest(manifest);
            }

            int randomIndex = Random.Range(0, allManifests.Count-1);
            var randomOne = allManifests[randomIndex];
            mergedManifest.CRC = randomOne.CRC;
            mergedManifest.ManifestFileVersion = randomOne.ManifestFileVersion;
            mergedManifest.BuildManifest();

            var stringToSave = YamlManifestPreProcessor.ParseToString(mergedManifest);
            Debug.LogWarning(stringToSave);
        }

        private static YamlAssetBundleManifest LoadManifestFromFolder(string path)
        {
            string loadTextFromPath = null;
            if (!FileAPITools.Exists(path))
            {
                Debug.LogWarning("Not exitst : " + path);

                return null;
            }
            
            loadTextFromPath = FileAPITools.ReadFile(path);
            if (string.IsNullOrEmpty(loadTextFromPath))
            {
                return null;
            }
            return YamlManifestPreProcessor.ParseString(loadTextFromPath);
        }
    }
}
