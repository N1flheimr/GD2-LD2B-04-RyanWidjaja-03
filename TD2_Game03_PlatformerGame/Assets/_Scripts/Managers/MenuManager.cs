using System.Collections.Generic;
using UnityEngine;

namespace NifuDev
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject _startScreenPanel;
        [SerializeField] private GameObject _resultScreenPanel;

        private void Awake()
        {
            GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        }
        private void Update()
        {
  
        }

        private void GameManager_OnGameStateChanged(GameState state)
        {
            _startScreenPanel.SetActive(state == GameState.Preparation);
            _resultScreenPanel.SetActive(state == GameState.Result);
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
        }
    }
}

