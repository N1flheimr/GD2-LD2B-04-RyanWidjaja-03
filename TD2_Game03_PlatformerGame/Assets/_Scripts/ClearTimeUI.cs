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

            TimeSpan goldTIme = TimeSpan.FromSeconds(LevelManager.Instance.GetGoldMetalTime());
            TimeSpan silverTime = TimeSpan.FromSeconds(LevelManager.Instance.GetSilverMetalTime());
            TimeSpan bronzeTime = TimeSpan.FromSeconds(LevelManager.Instance.GetBronzeMetalTime());

            if (time <= goldTIme)
            {
                medalImage.sprite = ImageManager.GetMedalSprite(ImageManager.MedalType.Gold);
                Debug.Log("Gold");
            }
            else if (time <= silverTime && time > goldTIme)
            {
                medalImage.sprite = ImageManager.GetMedalSprite(ImageManager.MedalType.Silver);
                Debug.Log("Silver");
            }
            else if (time <= bronzeTime && time > silverTime)
            {
                medalImage.sprite = ImageManager.GetMedalSprite(ImageManager.MedalType.Bronze);
                Debug.Log("Bronze");
            }

            else
            {
                medalImage.sprite = ImageManager.GetMedalSprite(ImageManager.MedalType.NoMedal);
                Debug.Log("NoMedal");
            }

            bestTimeText.text = BestTimeManager.Instance.GetBestTimeString();
        }

        private void OnDestroy()
        {
            Goal.OnGoalReached -= Goal_OnGoalReached;
        }
    }
}

