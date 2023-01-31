using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NifuDev
{
    public class BlurEffect : MonoBehaviour
    {
        [SerializeField] VolumeProfile volumeProfile;

        private void Awake()
        {
            volumeProfile = GetComponent<VolumeProfile>();
        }
    }
}

