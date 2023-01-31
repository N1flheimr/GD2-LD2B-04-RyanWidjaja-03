using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace NifuDev
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public static event Action<GameState> OnGameStateChanged;

        private GameState currentState;

        [SerializeField] private PlayerController player;
        [SerializeField] private Transform spawnTransform;

        private void Start()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            UpdateGameState(GameState.Preparation);

            if (player == null)
            {
                SpawnPlayer(spawnTransform.position);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartScene();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (currentState)
                {
                    case GameState.Preparation:
                        UpdateGameState(GameState.Run);
                        break;
                    case GameState.Result:
                        LoadNextLevel();
                        break;
                }
            }
        }

        public void UpdateGameState(GameState newState)
        {
            currentState = newState;

            switch (currentState)
            {
                case GameState.Preparation:
                    HandlePreparationState();
                    break;
                case GameState.Run:
                    HandleRunState();
                    break;
                case GameState.Result:
                    HandleResultState();
                    break;
            }

            OnGameStateChanged?.Invoke(newState);
        }

        private void HandlePreparationState()
        {
            player.enabled = false;
        }

        private void HandleRunState()
        {
            player.enabled = true;
        }
        private void HandleResultState()
        {
            player.enabled = false;
        }

        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void SpawnPlayer(Vector3 spawnPos)
        {
            if (player != null)
            {
                return;
            }
            else
            {
                player = Instantiate(player, spawnPos, Quaternion.identity);
            }
        }

        public PlayerController GetPlayerController()
        {
            return player;
        }

        public GameState GetCurrentState()
        {
            return currentState;
        }

        public void LoadNextLevel()
        {
            if (SceneManager.GetActiveScene().buildIndex <= SceneManager.sceneCount)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {

            }
        }
    }
}

