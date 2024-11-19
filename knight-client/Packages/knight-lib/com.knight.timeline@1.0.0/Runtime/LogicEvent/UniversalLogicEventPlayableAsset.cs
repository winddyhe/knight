using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Knight.Framework.Timeline
{
    public class UniversalLogicEventPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] public ScriptableObject EventContent;
        public virtual ClipCaps clipCaps { get { return ClipCaps.Blending; } }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<UniversalLogicEventPlayableBehaviour>.Create(graph);
        }
    }
}