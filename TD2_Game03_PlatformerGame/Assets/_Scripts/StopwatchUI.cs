using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

namespace NifuDev
{
    public class StopwatchUI : MonoBehaviour
    {
        private float currentTime;
        private bool isActive;
        private TimeSpan time;
        [SerializeField] private TextMeshProUGUI stopwatchText;

        private void Start()
        {
            currentTime = 0;
            stopwatchText.text = time.TotalMinutes.ToString("00") + ":" + time.TotalSeconds.ToString("00") + ":" + time.TotalMilliseconds.ToString("00");
            isActive = true;
        }

        private void Update()
        {
            if (isActive && GameState.Run == GameManager.Instance.GetCurrentState())
            {
                currentTime += Time.deltaTime;
            }

            time = TimeSpan.FromSeconds(currentTime);
            stopwatchText.text = time.ToString(@"mm\:ss\:ff");
        }

        public void StartStopwatch()
        {
            isActive = true;
        }
        public void StopStopwatch()
        {
            isActive = false;
        }

        public float GetCurrentTime()
        {
            return currentTime;
        }
    }
}

