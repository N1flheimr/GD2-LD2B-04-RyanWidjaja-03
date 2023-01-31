using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace NifuDev
{
    public class ClearTimeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI clearTimeText;
        void Start()
        {
            Goal.OnGoalReached += Goal_OnGoalReached;
        }

        private void Goal_OnGoalReached(float stopwatchTime)
        {
            TimeSpan time;
            time= TimeSpan.FromSeconds(stopwatchTime);
            clearTimeText.text = time.ToString(@"mm\:ss\:ff");
        }

        private void OnDestroy()
        {
            Goal.OnGoalReached -= Goal_OnGoalReached;
        }
    }
}

