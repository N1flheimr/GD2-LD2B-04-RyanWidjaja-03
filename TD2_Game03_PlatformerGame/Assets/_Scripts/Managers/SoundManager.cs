using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NifuDev
{
    public static class SoundManager
    {
        public enum SoundType
        {
            PlayerFootsteps,
            Jump,
            JumpLanding,
            Dash,
            EnemyPop,
            LevelSelectClick,
            PreparationScreenSpace,
            DashRefill,
            GoalReached
        }

        private static Dictionary<SoundType, float> soundTimerDictionary;

        public static void Initialize()
        {
            soundTimerDictionary = new Dictionary<SoundType, float>();
            soundTimerDictionary[SoundType.PlayerFootsteps] = 0f;
        }

        public static void PlaySound(SoundType sound)
        {
            if (CanPlaySound(sound))
            {
                GameObject soundGameObject = new GameObject("Sound");
                AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
                audioSource.PlayOneShot(GetAudioClip(sound));
            }
        }
        public static void PlaySound(SoundType sound, Vector3 position)
        {
            if (CanPlaySound(sound))
            {
                GameObject soundGameObject = new GameObject("Sound");
                soundGameObject.transform.position = position;
                AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
                audioSource.clip = GetAudioClip(sound);
                audioSource.maxDistance = 100f;
                audioSource.spatialBlend = 1f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.dopplerLevel = 0f;
                audioSource.Play();
            }
        }
        public static void PlaySound(SoundType sound, float volume)
        {
            if (CanPlaySound(sound))
            {
                GameObject soundGameObject = new GameObject("Sound");
                AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
                audioSource.clip = GetAudioClip(sound);
                audioSource.volume = volume;
                audioSource.Play();
            }
        }

        private static bool CanPlaySound(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.PlayerFootsteps:
                    if (soundTimerDictionary.ContainsKey(soundType))
                    {
                        float lastTimePlayed = soundTimerDictionary[soundType];
                        float playerMoveTimerMax = 0.45f;
                        if (lastTimePlayed + playerMoveTimerMax < Time.time)
                        {
                            soundTimerDictionary[soundType] = Time.time;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                default:
                    return true;
            }
        }
        private static AudioClip GetAudioClip(SoundType sound)
        {
            foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.Instance.soundAudioClipArray)
            {
                if (soundAudioClip.soundType == sound)
                {
                    return soundAudioClip.audioClip;
                }
            }
            return null;
        }
    }
}