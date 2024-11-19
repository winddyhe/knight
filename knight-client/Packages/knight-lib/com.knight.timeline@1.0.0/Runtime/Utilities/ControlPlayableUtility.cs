using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Knight.Framework.Timeline
{
    public static class ControlPlayableUtility
    {
        public static bool DetectCycle(
            ControlPlayableAsset asset, PlayableDirector director, HashSet<PlayableDirector> set = null)
        {
            if (director == null || asset == null || !asset.updateDirector)
                return false;

            if (set == null)
                set = new HashSet<PlayableDirector>();

            if (set.Contains(director))
                return true;

            var gameObject = asset.sourceGameObject.Resolve(director);
            if (gameObject == null)
                return false;

            set.Add(director);

            foreach (var subDirector in GetComponent<PlayableDirector>(gameObject, asset.searchHierarchy))
            {
                foreach (var childAsset in GetPlayableAssets(subDirector))
                {
                    if (DetectCycle(childAsset, subDirector, set))
                        return true;
                }
            }

            set.Remove(director);

            return false;
        }

        internal static IList<T> GetComponent<T>(GameObject gameObject, bool searchHierarchy)
        {
            var components = new List<T>();
            if (gameObject != null)
            {
                if (searchHierarchy)
                {
                    gameObject.GetComponentsInChildren<T>(true, components);
                }
                else
                {
                    gameObject.GetComponents<T>(components);
                }
            }
            return components;
        }

        public static IEnumerable<ControlPlayableAsset> GetPlayableAssets(PlayableDirector director)
        {
            var timeline = director != null ? (director.playableAsset as TimelineAsset) : null;
            if (timeline != null)
            {
                foreach (var t in timeline.GetOutputTracks())
                {
                    var controlTrack = t as ControlTrack;
                    if (controlTrack != null)
                    {
                        foreach (var c in t.GetClips())
                        {
                            var asset = c.asset as ControlPlayableAsset;
                            if (asset != null)
                                yield return asset;
                        }
                    }
                }
            }
        }
    }
}
