using Cinemachine;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.PowerslideKartPhysics.Scripts
{
    internal class CinemachineShakeHandler : MonoBehaviour
    {
        // this script will only help use add this component to the cinemachine to manage the shake 

        public void AttachShakeNoise(NoiseSettings shakeNoise)
        {
            this.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = shakeNoise;
        }
    }
}
