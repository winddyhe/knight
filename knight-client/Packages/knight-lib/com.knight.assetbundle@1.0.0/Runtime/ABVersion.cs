using System.Collections.Generic;
using Knight.Framework.Serializer;

namespace Knight.Framework.Assetbundle
{
    [SerializerBinary]
    [SBFileReadWrite]
    public partial class ABVersion
    {
        public Dictionary<string, ABEntry> Entries;
    }

    public static class ABVersionExpand
    {
        public static int GetABEntryVersion(this ABVersion rABVersion, string rABPath)
        {
            if (rABVersion == null || rABVersion.Entries == null) return 0;
            rABVersion.Entries.TryGetValue(rABPath, out var rABEntry);
            return rABEntry.Version;
        }
    }
}