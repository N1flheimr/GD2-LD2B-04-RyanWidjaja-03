using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace NifuDev
{
    public class Goal : MonoBehaviour
    {
        public static event Action<float> OnGoalReached;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                StopwatchUI.Instance.StopStopwatch();
                OnGoalReached?.Invoke(StopwatchUI.Instance.GetCurrentTime());
                GameManager.Instance.UpdateGameState(GameState.Result);
            }
        }
    }
}

