using System.Collections;
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
                Invoke(nameof(RestartScene), 0.12f);
                
                SoundManager.PlaySound(SoundManager.SoundType.PreparationScreenSpace);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (currentState)
                {
                    case GameState.Preparation:
                        UpdateGameState(GameState.Run);

                        SoundManager.PlaySound(SoundManager.SoundType.PreparationScreenSpace);
                        break;
                    case GameState.Result:
                        SoundManager.PlaySound(SoundManager.SoundType.PreparationScreenSpace);
                        Invoke(nameof(LoadNextLevel), 0.12f);
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "SelectLevelScene")
            {
                if (GameObject.FindGameObjectWithTag("GameMusic").TryGetComponent<AudioSource>(out var audioSource))
                {
                    Destroy(audioSource.gameObject);
                    Debug.Log("Destroyed");
                }
                SoundManager.PlaySound(SoundManager.SoundType.PreparationScreenSpace);
                SceneManager.LoadScene("SelectLevelScene");
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
            int currentScene = SceneManager.GetActiveScene().buildIndex;

            int sceneCount = 4;
            if (currentScene < sceneCount - 1)
            {
                SceneManager.LoadScene(currentScene + 1);
            }
            else
            {
                if (GameObject.FindGameObjectWithTag("GameMusic").TryGetComponent<AudioSource>(out var audioSource))
                {
                    Destroy(audioSource.gameObject);
                    Debug.Log("Destroyed");
                }
                SceneManager.LoadScene("SelectLevelScene");
            }
        }

    }
}

