using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

namespace NifuDev
{
    public class BestTimeManager : MonoBehaviour
    {
        public static BestTimeManager Instance { get; private set; }

        [SerializeField] private List<float> levelBestTimeList = new();

        private void Awake()
        {

        }

        void Start()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                if (PlayerPrefs.HasKey(SceneUtility.GetScenePathByBuildIndex(i)))
                {
                    levelBestTimeList.Add(PlayerPrefs.GetFloat(SceneUtility.GetScenePathByBuildIndex(i)));
                }
                else
                {
                    PlayerPrefs.SetFloat(SceneUtility.GetScenePathByBuildIndex(i), 999.9f);
                    levelBestTimeList.Add(PlayerPrefs.GetFloat(SceneUtility.GetScenePathByBuildIndex(i)));
                }
            }

            Goal.OnGoalReached += Goal_OnGoalReached;
        }

        private void Goal_OnGoalReached(float newTime)
        {
            if (CheckIsBestTime(newTime))
            {
                SaveBestTime(newTime);
            }
        }

        public bool CheckIsBestTime(float time)
        {
            if (time < levelBestTimeList[SceneManager.GetActiveScene().buildIndex])
            {
                return true;
            }
            return false;
        }

        public void SaveBestTime(float newBestTime)
        {
            PlayerPrefs.SetFloat(SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex), newBestTime);
            levelBestTimeList[SceneManager.GetActiveScene().buildIndex] = newBestTime;
        }
        private void OnDestroy()
        {
            Goal.OnGoalReached -= Goal_OnGoalReached;
        }

        public List<float> GetLevelBestTimeList()
        {
            return levelBestTimeList;
        }
    }
}

