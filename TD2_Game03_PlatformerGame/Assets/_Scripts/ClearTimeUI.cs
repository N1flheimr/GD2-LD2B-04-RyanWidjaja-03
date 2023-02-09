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
        [SerializeField] private Image medalImage;
        void Start()
        {
            Goal.OnGoalReached += Goal_OnGoalReached;
        }

        private void Goal_OnGoalReached(float stopwatchTime)
        {
            TimeSpan time;
            time = TimeSpan.FromSeconds(stopwatchTime);
            clearTimeText.text = time.ToString(@"mm\:ss\:ff");

            if (stopwatchTime < LevelManager.Instance.GetGoldMetalTime())
            {
                medalImage.sprite = ImageManager.GetMedalSprite(ImageManager.MedalType.Gold);
            }
            else if (stopwatchTime < LevelManager.Instance.GetSilverMetalTime() && stopwatchTime > LevelManager.Instance.GetGoldMetalTime())
            {
                medalImage.sprite = ImageManager.GetMedalSprite(ImageManager.MedalType.Silver);
            }
            if (stopwatchTime < LevelManager.Instance.GetBronzeMetalTime())
            {
                medalImage.sprite = ImageManager.GetMedalSprite(ImageManager.MedalType.Bronze);
            }

            bestTimeText.text = BestTimeManager.Instance.GetBestTimeString();
        }

        private void OnDestroy()
        {
            Goal.OnGoalReached -= Goal_OnGoalReached;
        }
    }
}

