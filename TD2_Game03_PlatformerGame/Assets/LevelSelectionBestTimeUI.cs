using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


namespace NifuDev
{
    public class LevelSelectionBestTimeUI : MonoBehaviour
    {
        [SerializeField] private List<TextMeshProUGUI> bestTimeTMProList;

        private void Start()
        {

            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                if (BestTimeManager.Instance.GetLevelBestTimeList()[i] >= 995f)
                {
                    bestTimeTMProList[i - 1].text = "NOT CLEARED";
                    bestTimeTMProList[i - 1].color = Color.gray;
                    continue;
                }
                bestTimeTMProList[i - 1].text = BestTimeManager.Instance.GetBestTimeString(i);
            }
        }
    }
}