using System.Collections.Generic;
using UnityEngine;
using System;

namespace NifuDev
{
    public class Goal : MonoBehaviour
    {
        public static event Action OnGoalReached;
        [SerializeField] private StopwatchUI stopwatchUI;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                stopwatchUI.StopStopwatch();
                OnGoalReached?.Invoke();
            }
        }
    }
}

