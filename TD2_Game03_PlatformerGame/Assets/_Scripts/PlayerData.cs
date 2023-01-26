using System.Collections.Generic;
using UnityEngine;

namespace NifuDev
{
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "PlayerData")]
    public class PlayerData : ScriptableObject
    {
        [Header("Run Parameters")]
        public float moveSpeed_;
        public float acceleration_;
        public float deccelaration_;

        public float velPower_;
        public float frictionAmount;

        [Range(0, 1f)]
        public float accelInAir;
        [Range(0, 1f)]
        public float deccelInAir;

        [Space(5)]
        public float jumpForce_;
        public float fallClamp_;

        [Header("Assist")]
        [Range(0, 0.5f)]
        public float coyoteTime;
        [Range(0, 1.5f)]
        public float jumpInputBufferTime;
        [Range(0, 0.5f)]
        public float dashInputBufferTime;
        public float jumpApexTimeThreshold;
        [Space(5)]

        //JUMP APEX
        public float jumpApexMaxSpeedMult;
        public float jumpApexAccelerationMult;

        [Space(5)]

        [Header("Wall Jump Parameters")]
        public Vector2 wallJumpForce;
        [Range(0, 1f)]
        public float wallJumpRunLerp;
        [Range(0, 1f)]
        public float wallJumpTime;
        [Space(5)]

        [Header("Gravity Multiplier")]
        public float gravityScale;
        public float fallGravityMult;
        public float jumpCutGravityMult;
        [Range(0, 1f)]
        public float jumpApexGravityMult;
        [Space(5)]

        [Header("Dash Parameters")]
        [Range(0f, 1f)]
        public float dashEndRunLerp;

        public int dashAmount;
        public float dashRefillTime;
        public float dashSleepTime;
        public float dashAttackTime;
        public float dashEndTime;

        public float dashSpeed;
        public Vector2 dashEndSpeed;
        [Space(5)]


        [Header("Wall Slide Parameters")]
        public float slideSpeed;
        public float slideAccel;
        public Vector2 slideVelocity;
        public bool useSlideForce;
    }
}

