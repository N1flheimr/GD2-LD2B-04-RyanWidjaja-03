using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NifuDev
{
    public class ClearTimeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI clearTimeText;
        [SerializeField] private TextMeshProUGUI bestTimeText;
        void Start()
        {
            Goal.OnGoalReached += Goal_OnGoalReached;
        }

        private void Goal_OnGoalReached(float stopwatchTime)
        {
            TimeSpan time;
            time = TimeSpan.FromSeconds(stopwatchTime);
            clearTimeText.text = time.ToString(@"mm\:ss\:ff");

            TimeSpan bestTime;
            bestTime = TimeSpan.FromSeconds(BestTimeManager.Instance.GetLevelBestTimeList()[SceneManager.GetActiveScene().buildIndex]);
            bestTimeText.text = bestTime.ToString(@"mm\:ss\:ff");
        }

        private void OnDestroy()
        {
            Goal.OnGoalReached -= Goal_OnGoalReached;
        }
    }
}

