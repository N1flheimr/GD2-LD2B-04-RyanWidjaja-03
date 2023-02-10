using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NifuDev
{
    public class GameAssets : MonoBehaviour
    {
        public static GameAssets Instance { get; private set; }

        public SoundAudioClip[] soundAudioClipArray;

        public Image[] imageArray;

        [System.Serializable]
        public class SoundAudioClip
        {
            public SoundManager.SoundType soundType;
            public AudioClip audioClip;
        }

        [System.Serializable]
        public class Image
        {
            public ImageManager.MedalType medalType;
            public Sprite Sprite;
        }


        private void Awake()
        {
            Instance = this;

            SoundManager.Initialize();
        }
    }
}