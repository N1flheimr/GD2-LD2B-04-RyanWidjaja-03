using System.Collections.Generic;
using UnityEngine;

namespace NifuDev
{
    public enum GameState
    {
        Preparation,
        Run,
        Result
    }

    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }
    }
}

