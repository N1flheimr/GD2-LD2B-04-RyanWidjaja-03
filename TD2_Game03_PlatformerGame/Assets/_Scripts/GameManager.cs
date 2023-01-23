using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NifuDev
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                }

                return _instance;
            }
        }

        [SerializeField] private PlayerController player;
        [SerializeField] private Transform spawnTransform;

        private void Start()
        {
            if (!FindObjectOfType<PlayerController>())
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
            player = Instantiate(player, spawnPos, Quaternion.identity);
        }

        public PlayerController GetPlayerController()
        {
            return player;
        }
    }
}

