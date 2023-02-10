using System.Collections;
using System.Security.Cryptography;
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

        IEnumerator OpenSceneCoroutine()
        {
            //if (GameObject.FindGameObjectWithTag("GameMusic").TryGetComponent<AudioSource>(out AudioSource bgmMusic))
            //{
            //    //bgmMusic.Play();
            //}

            SoundManager.PlaySound(SoundManager.SoundType.LevelSelectClick, 0.5f);
            yield return Helpers.GetWaitForSeconds(0.1f);
            SceneManager.LoadScene("Level " + levelNumber);
        }

        private void OpenScene()
        {
            StartCoroutine(nameof(OpenSceneCoroutine));
        }
    }
}

