using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

namespace NifuDev
{
    public class BestTimeManager : MonoBehaviour
    {
        private TimeSpan time;
        [SerializeField] private List<float> levelBestTimeList = new List<float>();
        [SerializeField] private float bestTime;
        [SerializeField] private TextMeshProUGUI bestTimeText;

        void Start()
        {
            levelBestTimeList.Add(PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name, 0f));
            time = TimeSpan.FromSeconds(PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name, 0f));
            bestTimeText.text = time.ToString(@"mm\:ss\:ff");
        }

        private void CheckBestTime()
        {

        }
        private void UpdateBestTimeText()
        {
            TimeSpan time = TimeSpan.FromSeconds(bestTime);
            bestTimeText.text = time.ToString(@"mm\:ss\:ff");
        }

        private void SaveBestTime()
        {

        }
    }
}

