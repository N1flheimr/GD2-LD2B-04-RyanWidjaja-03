using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NifuDev
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private int levelNumber;

        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OpenScene);
        }

        public void OpenScene()
        {
            SceneManager.LoadScene("Level " + levelNumber);
        }
    }
}

