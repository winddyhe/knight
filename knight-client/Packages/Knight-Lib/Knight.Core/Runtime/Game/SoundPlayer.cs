using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight.Core
{
    public interface ISoundPlayer
    {
        void Initialize();
        void PlaySingle(string rAudioName);
        void PlaySingle(string rAudioName, bool bIsOverride);
        void PlayMulti(string rAudioName);
    }

    public class SoundPlayer
    {
        public static ISoundPlayer Instance { get; set; }
    }
}
