using System.Collections.Generic;
using UnityEngine;

namespace NifuDev
{
    public class Goal : MonoBehaviour
    {
        [SerializeField] private StopwatchUI stopwatchUI;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                stopwatchUI.StopStopwatch();
            }
        }
    }
}

