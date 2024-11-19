using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Knight.Framework.Timeline
{
#if SKILL_TIME_LINE_HIDE_LOGIC_TRACK
    [HideInMenu]
#endif
    [TrackClipType(typeof(UniversalLogicEventPlayableAsset))]
    public class UniversalLogicEventTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<UniversalLogicEventMixerBehaviour>.Create(graph, inputCount);
        }
    }
}
