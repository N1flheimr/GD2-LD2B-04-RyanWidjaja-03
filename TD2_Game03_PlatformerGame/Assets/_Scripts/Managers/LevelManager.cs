using System.Collections.Generic;
using UnityEngine;

namespace NifuDev
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField] private float goldMedalTime;
        [SerializeField] private float silverMedalTime;
        [SerializeField] private float bronzeMedalTime;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public float GetGoldMetalTime()
        {
            return goldMedalTime;
        }

        public float GetSilverMetalTime()
        {
            return silverMedalTime;
        }

        public float GetBronzeMetalTime()
        {
            return bronzeMedalTime;
        }
    }
}

