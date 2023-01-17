using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NifuDev
{
    public class GameManager : MonoBehaviour
    {

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
    }
}

