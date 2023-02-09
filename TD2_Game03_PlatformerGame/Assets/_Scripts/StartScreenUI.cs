using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

namespace NifuDev
{
    public class StartScreenUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI bestTimeText;

        private void Start()
        {
            bestTimeText.text = BestTimeManager.Instance.GetBestTimeString();
        }
    }
}

