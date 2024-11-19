using System;
using Knight.Framework.GameSave;
using Nino.Core;

namespace Game
{
    [NinoType]
    [GameSave]
    public partial class GameSaveAccount : GameSaveData
    {
        [NinoMember(0)]
        public string AccountName { get; set; }  
        [NinoMember(1)]
        public int PlayerLevel { get; set; }
        [NinoMember(2)]
        public int PlayerExp { get; set; } 
    }
}
