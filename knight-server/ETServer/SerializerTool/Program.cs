using System;
using ETHotfix;

namespace Knight.Core.Serializer.Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            UnitInfo rUnitInfo = new UnitInfo();
            Console.WriteLine(rUnitInfo.X + ", " + rUnitInfo.Z);

            SerializerBinaryEditor.Instance.Analysis();
        }
    }
}
