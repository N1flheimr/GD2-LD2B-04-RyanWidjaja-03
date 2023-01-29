using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Pause,
    RunStart,
    RunEnd
}

namespace NifuDev
{
    public class GameStateManager : MonoBehaviour
    {
        private static GameStateManager _instance;
        public static GameStateManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameStateManager();
                }
                return _instance;
            }
        }
    }
}

