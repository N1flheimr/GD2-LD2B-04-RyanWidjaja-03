using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace NifuDev
{
    public class DashLeftUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dashCountText;
        [SerializeField] private PlayerController player;

        private void Start()
        {
            dashCountText.text = player.GetDashesLeft_().ToString("0") + "/3";

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
            dashCountText.text = player.GetDashesLeft_().ToString("0") + "/3";
            if (player.GetDashesLeft_() == 3)
            {
                dashCountText.color = Color.red;
            }
            else
            {
                dashCountText.color = Color.white;
            }
        }

        private void OnDestroy()
        {
            player.OnDashesRefilled -= PlayerController_OnDashesRefilled;
            player.OnDashesUsed -= PlayerController_OnDashesUsed;
        }
    }
}

