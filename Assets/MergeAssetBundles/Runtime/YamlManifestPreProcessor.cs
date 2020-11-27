using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using YamlDotNet.Serialization;

namespace AssetBundleMerger
{
    public static class Extensions
    {
        public static string[] Split(this string toSplit, string splitOn) {
            return toSplit.Split(new string[] { splitOn }, StringSplitOptions.None);
        }
    }
    public class YamlManifestPreProcessor
    {
        // public class CustomInfoNode: INodeDeserializer
        // {
        //     public bool Deserialize(IParser reader, Type expectedType, 
        //         Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        //     {
        //         value = null;
        //         if (expectedType == typeof(YamlAssetBundleManifestInfoList))
        //         {
        //             value = new YamlAssetBundleManifestInfoList();
        //             return true;
        //         }
        //
        //         return false;
        //     }
        // }
        //
        // public class CustomDependencyNode: INodeDeserializer
        // {
        //     public bool Deserialize(IParser reader, Type expectedType, 
        //         Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        //     {
        //         value = null;
        //         if (expectedType == typeof(YamlAssetBundleDependency))
        //         {
        //             value = new YamlAssetBundleDependency();
        //
        //             return true;
        //         }
        //
        //         return false;
        //     }
        // }
        

        private static YamlAssetBundleManifest ParseYamlFiles(string yaml)
        {
            var deserializer = new DeserializerBuilder();
            // Type typeOfInfocontainer = typeof(YamlAssetBundleManifestInfoList);
            // Type typeOfInfo = typeof(YamlAssetBundleManifestInfo);
            // Type typeOfInfoDep = typeof(YamlAssetBundleDependency);
            //
            //
            // for (int i = 0; i < 999; i++)
            // {
            //     deserializer.WithTagMapping("Info_" + i.ToString(), typeOfInfocontainer);
            //     deserializer.WithTagMapping("Info_" + i.ToString(), typeOfInfo);
            //     deserializer.WithTagMapping("Dependency_" + i.ToString(), typeOfInfoDep);
            // }
            // deserializer.WithNodeDeserializer(new  CustomInfoNode());
            // deserializer.WithNodeDeserializer(new  CustomDependencyNode());
            //
            var deserializerInterface = deserializer.Build();

            return deserializerInterface.Deserialize<YamlAssetBundleManifest>(yaml);
        }

        public static YamlAssetBundleManifest ParseString(string FullString)
        {
            var strArray = FullString.Split('I');
            var number = strArray.Length;
            strArray = null;
            StringBuilder builder = new StringBuilder(FullString);

            const string InfoString = "Info_{0}:";
            const string DependencyString = "Dependency_{0}:";

            for (int i = 0; i < number; i++)
            {
                builder.Replace(string.Format(InfoString, i), "-");
                builder.Replace(string.Format(DependencyString, i), "-");
            }
            builder.Replace("{}", "");
            return ParseYamlFiles(builder.ToString());
        }
        
        public static string ParseToString(AssetBundleBuildsManifest FullString)
        {
            var serilizer = new SerializerBuilder();
            serilizer.EmitDefaults();
            var serializedInstance = serilizer.Build();
            string result = serializedInstance.Serialize(FullString);
            
            var number = FullString.AssetBundleManifest.AssetBundleInfos.Count;
            
            StringBuilder builder = new StringBuilder(result);

            const string spaceForInfo = "  -";
            const string spaceForDependency = "    -";

            const string InfoString = "  Info_{0}:\n   ";
            const string DependencyString = "     Dependency_{0}:\n     ";
            
            builder.Replace(spaceForDependency, string.Format(DependencyString, 0));
            builder.Replace(spaceForInfo, string.Format(InfoString, 0));
            builder.Replace("[]","{}");

            result = builder.ToString();
            const string InfoStringConst = "Info_{0}:";
            const string InfoStringConst0 = "Info_0:";
            
            const string DependencyStringConst = "Dependency_{0}:";
            const string DependencyStringConst0 = "Dependency_0:";
            
            var splitBy = result.Split(InfoStringConst0);
            builder.Clear();
            string[] anotherSplitByDependency = null;
            for (int i = 0; i < splitBy.Length; i++)
            {
                anotherSplitByDependency = splitBy[i].Split(DependencyStringConst0);
                if (anotherSplitByDependency.Length < 1)
                {
                    builder.Append(splitBy[i]);
                    continue;
                }

                if (i != 0)
                {
                    builder.Append(string.Format(InfoStringConst, i-1));
                }
                for (int j = 0; j < anotherSplitByDependency.Length; j++)
                {
                    builder.Append(anotherSplitByDependency[j]);
                    if ((j + 1) >= anotherSplitByDependency.Length)
                    {
                        continue;
                    }

                    builder.Append(string.Format(DependencyStringConst, j));

                }
            }


            return builder.ToString();
        }
        
        /*
ManifestFileVersion: 0
CRC: 4271548926
AssetBundleManifest:
  AssetBundleInfos:
    Info_0:
      Name: shared
      Dependencies: {}
    Info_1:
      Name: hotellobbyfivestar
      Dependencies:
        Dependency_0: shared
    Info_2:
      Name: plantsafety_workshop
      Dependencies:
        Dependency_0: shared
                    
CRC: 4271548926
ManifestFileVersion: 1
AssetBundleManifest:
  AssetBundleInfos:
  - Name: shared
    Dependencies: []
  - Name: hotellobbyfivestar
    Dependencies:
    - shared
  - Name: plantsafety_workshop
    Dependencies:
    - shared
  - Name: plantsafety_factory
    Dependencies:
    - shared
  - Name: plantsafety_workshop_module3
    Dependencies:
    - shared

         */

    }
}