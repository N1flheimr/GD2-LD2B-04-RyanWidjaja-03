using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace NifuDev
{
    public class DashLeftUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI DashCountText;
        [SerializeField] private PlayerController player;

        private void Start()
        {
            DashCountText.text = player.GetDashesLeft_().ToString("0") + "/3";

            player.OnDashesRefilled += PlayerController_OnDashesRefilled;
            player.OnDashesUsed += PlayerController_OnDashesUsed;
        }

        private void PlayerController_OnDashesUsed()
        {
            UpdateDashCountText();
        }

        private void PlayerController_OnDashesRefilled()
        {
            UpdateDashCountText();
        }

        private void UpdateDashCountText()
        {
            DashCountText.text = player.GetDashesLeft_().ToString("0") + "/3";
        }

        private void OnDestroy()
        {
            player.OnDashesRefilled -= PlayerController_OnDashesRefilled;
            player.OnDashesUsed -= PlayerController_OnDashesUsed;
        }
    }
}

