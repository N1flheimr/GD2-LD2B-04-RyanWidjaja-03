using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace NifuDev
{
    public class CinemachineShake : MonoBehaviour
    {
        public static CinemachineShake Instance { get; private set; }

        private CinemachineVirtualCamera _cinemachineVCam;

        private float shakeTimer;
        private float shakeTimerTotal;
        private float startingIntensity;


        private void Awake()
        {
            _cinemachineVCam = GetComponent<CinemachineVirtualCamera>();
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Update()
        {
            if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;

                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
           _cinemachineVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain =
                    Mathf.Lerp(startingIntensity, 0f, (1f - (shakeTimer / shakeTimerTotal)));
            }
        }

        public void ShakeCamera(float intensity, float time)
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                _cinemachineVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

            startingIntensity = intensity;
            shakeTimer = time;
            shakeTimerTotal = time;
        }
    }
}

