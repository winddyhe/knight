//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core;
using System.IO;
using Core.WindJson;
using Framework;

namespace Game.Knight
{
    public enum GPCSymbolType
    {
        Unknown = 0,    // 未知
        ObjStart,       // '{'
        ObjEnd,         // '}'
        ArgsStart,      // '('
        ArgsEnd,        // ')'
        ArgsSplit,      // ','
        ElementSplit,   // ';'
        Identifer,      // 标识符
        Arg,            // 值类型: 整数、实数、字符串、true、false、null
    }
    
    //[SBGroup("GamePlayConfig")]
    public partial class GPCSymbolItem //: SerializerBinary
    {
        public string           Value;
        public GPCSymbolType    Type;

        public override string ToString()
        {
            return string.Format("({0}, {1})", Type, Value);
        }
    }

    //[SBGroup("GamePlayConfig")]
    public partial class GPCSymbolElement //: SerializerBinary
    {
        public GPCSymbolItem       Identifer;
        public List<GPCSymbolItem> Args;

        public GPCSymbolElement()
        {
        }

        public GPCSymbolElement(GPCSymbolItem rIdentifer, List<GPCSymbolItem> rArgs)
        {
            this.Identifer = rIdentifer;
            this.Args = new List<GPCSymbolItem>(rArgs);
        }

        public override string ToString()
        {
            string rResult = this.Identifer.ToString();
            for (int i = 0; i < this.Args.Count; i++)
            {
                rResult += ", " + this.Args[i].ToString();
            }
            return rResult;
        }

        public List<string> ToArgs()
        {
            var rArgs = new List<string>();
            for (int i = 0; i < this.Args.Count; i++)
            {
                rArgs.Add(this.Args[i].Value);
            }
            return rArgs;
        }
    }

    //[SBGroup("GamePlayConfig")]
    public partial class GPCSymbolObject //: SerializerBinary
    {
        public GPCSymbolElement        Head;
        public List<GPCSymbolElement>  Bodies;
    }

    //[SBGroup("GamePlayConfig")]
    public partial class GPCSkillConfig //: SerializerBinary
    {
        //[SBIgnore]
        public static GPCSkillConfig Instance { get { return Singleton<GPCSkillConfig>.GetInstance(); } }

        public Dict<int, List<GPCSymbolObject>> ActorSkills;

        #region Loading...
        public void Load_Local(string rLocalAssetPath)
        {
            string rSkillListPath = rLocalAssetPath + "/SkillList.txt";
            string rSkillListConfig = File.ReadAllText(rSkillListPath);

            this.ActorSkills = new Dict<int, List<GPCSymbolObject>>();

            JsonNode rJsonArray = JsonParser.Parse(rSkillListConfig);
            for (int i = 0; i < rJsonArray.Count; i++)
            {
                int nSkillID = int.Parse(rJsonArray[i].Value);

                string rSkillPath = rLocalAssetPath + "/" + nSkillID + ".txt";
                string rSkillConfig = File.ReadAllText(rSkillPath);

                var rParser = new GamePlayComponentParser(rSkillConfig);
                var rSymbolObjs = rParser.Parser();

                this.ActorSkills.Add(nSkillID, rSymbolObjs);
            }

            string rBinaryPath = UtilTool.PathCombine(rLocalAssetPath.Replace("Text", "Binary"), "SkillConfig.bytes");
            using (var fs = new FileStream(rBinaryPath, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var br = new BinaryWriter(fs))
                {
                    //this.Serialize(br);
                }
            }
        }

        /// <summary>
        /// 异步加载技能配置
        /// </summary>
        public Coroutine Load(string rConfigABPath, string rConfigName)
        {
            return CoroutineManager.Instance.Start(Load_Async(rConfigABPath, rConfigName));
        }

        /// <summary>
        /// 卸载资源包
        /// </summary>
        public void Unload(string rConfigABPath)
        {
            AssetLoadManager.Instance.UnloadAsset(rConfigABPath);
        }

        private IEnumerator Load_Async(string rConfigABPath, string rConfigName)
        {
            var rAssetRequesst = AssetLoadManager.Instance.LoadAsset(rConfigABPath, rConfigName);
            yield return rAssetRequesst;
            if (rAssetRequesst.asset == null) yield break;

            TextAsset rConfigAsset = rAssetRequesst.asset as TextAsset;
            if (rConfigAsset == null) yield break;

            using (var ms = new MemoryStream(rConfigAsset.bytes))
            {
                using (var br = new BinaryReader(ms))
                {
                    //this.Deserialize(br);
                }
            }
        }
        #endregion

        public List<GPCSymbolObject> GetActorSkill(int nSkillID)
        {
            List<GPCSymbolObject> rSymbolObjs = null;
            this.ActorSkills.TryGetValue(nSkillID, out rSymbolObjs);
            return rSymbolObjs;
        }
    }
}