using System;

namespace Knight.Framework.GameSave
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GameSaveAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class GameSaveFileAttribute : Attribute
    {
        public string FileName;

        public GameSaveFileAttribute(string rFileName)
        {
            this.FileName = rFileName;
        }
    }
}
