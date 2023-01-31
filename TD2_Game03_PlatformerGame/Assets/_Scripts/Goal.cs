using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace NifuDev
{
    public class Goal : MonoBehaviour
    {
        public static event Action<float> OnGoalReached;
        [SerializeField] private StopwatchUI stopwatchUI;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                stopwatchUI.StopStopwatch();
                OnGoalReached?.Invoke(stopwatchUI.GetCurrentTime());
                GameManager.Instance.UpdateGameState(GameState.Result);
            }
        }
    }
}

