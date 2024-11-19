using Knight.Framework.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Knight.Framework.Timeline
{
#if SKILL_TIME_LINE_HIDE_EFFECT_TRACK
    [HideInMenu]
#endif
    /// <summary>
    /// A Track whose clips control time-related elements on a GameObject.
    /// </summary>
    [TrackClipType(typeof(UniversalEffectPlayableAsset), false)]
    public class UniversalEffectTrack : TrackAsset
    {
    }
}
