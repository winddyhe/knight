//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Knight.Core;
using UnityEngine;
using UnityFx.Async;

namespace Knight.Framework.TinyMode
{
    public class SoundAssetLoader : TSingleton<SoundAssetLoader>
    {
        private Dict<string, AudioClip> mSoundPrefabs;

        private SoundAssetLoader()
        {
        }

        public void Load(string rABPath)
        {
            this.mSoundPrefabs = new Dict<string, AudioClip>();
            
            var rAllEffectsRequest = AssetLoader.Instance.LoadAllAssets(rABPath, typeof(AudioClip));
            if (rAllEffectsRequest.AllAssets == null)
            {
                Debug.LogError("Cannot find sound folder.");
                return;
            }

            var rAllAssets = rAllEffectsRequest.AllAssets;
            for (int i = 0; i < rAllAssets.Length; i++)
            {
                this.mSoundPrefabs.Add(rAllAssets[i].name, rAllAssets[i] as AudioClip);
            }
        }

        public AudioClip GetAudioClip(string rClipName)
        {
            AudioClip rAudioClip = null;
            this.mSoundPrefabs.TryGetValue(rClipName, out rAudioClip);
            return rAudioClip;
        }
    }
}
