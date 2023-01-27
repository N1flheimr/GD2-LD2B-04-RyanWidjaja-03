using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NifuDev
{
    public class PlayerController : MonoBehaviour
    {
        public event Action OnDashesRefilled;
        public event Action OnDashesUsed;

        private static PlayerController _instance;
        public static PlayerController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayerController();
                }
                return _instance;
            }
        }
        [SerializeField] private PlayerData data;

        private PlayerInput controls;

        #region COMPONENTS
        private Rigidbody2D rb_;
        [SerializeField] private SpriteRenderer playerSprite;
        [SerializeField] private Transform wallCheckTransform;
        private Animator animator;
        #endregion

        #region STATE PARAMETERS
        private bool isGrounded_;
        private bool isJumping_;
        private bool isSliding_;
        private bool isDashing_;
        private bool isDashAttacking_;
        private bool isJumpCut_;

        private bool isWallJumping_;
        private bool isOnRightWall_;
        private bool isOnLeftWall_;

        private bool wasOnGround = true;
        #endregion

        //Dash
        private int dashesLeft_;
        private bool dashRefilling_;
        private Vector2 _lastDashDir;

        //WallJump
        private int lastWallJumpDir_;
        private float wallJumpStartTime_;
        //public bool useWallJump = true;

        #region TIMER
        private float lastOnGroundTime;
        private float lastOnWallTime;
        private float lastOnWallRightTime;
        private float lastOnWallLeftTime;
        private float LastPressedDashTime;
        private float lastPressedJumpTime;
        #endregion

        #region INPUT PARAMETERS
        private Vector2 moveInput_;
        #endregion

        #region CHECK PARAMETERS
        [Header("Ground Checks")]
        [SerializeField] private Transform groundCheckPoint_;
        [SerializeField] private Vector2 groundCheckSize_;
        [Space(5)]
        [Header("Wall Checks")]
        [SerializeField] private Transform frontWallCheckPoint_;
        [SerializeField] private Transform backWallCheckPoint_;
        [SerializeField] private Vector2 wallCheckSize_;
        #endregion

        #region Layers & Tags
        [Header("Layers & Tags")]
        [SerializeField] private LayerMask groundLayer_;
        [Space(5)]
        #endregion

        private bool isFacingRight;

        private void Awake()
        {
            if (rb_ == null)
                rb_ = GetComponent<Rigidbody2D>();

            controls = new PlayerInput();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            isFacingRight = true;
            SetGravityScale(data.gravityScale);

            controls.PlayerControlOnGround.Move.performed += ctx => StartAction();
        }

        private void Update()
        {
            #region TIMERS
            lastOnGroundTime -= Time.deltaTime;
            lastOnWallTime -= Time.deltaTime;
            lastOnWallRightTime -= Time.deltaTime;
            lastOnWallLeftTime -= Time.deltaTime;
            lastPressedJumpTime -= Time.deltaTime;
            LastPressedDashTime -= Time.deltaTime;
            #endregion

            moveInput_.x = Input.GetAxisRaw("Horizontal");
            moveInput_.y = Input.GetAxisRaw("Vertical");

            #region GENERAL CHECKS
            if (moveInput_.x != 0)
            {
                CheckDirectionToFace(moveInput_.x > 0.01f);
                if (!isJumping_ && isGrounded_)
                {
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.X))
            {
                OnDashInput();
            }
            #endregion

            #region PHYSICS CHECKS
            if (!isDashing_ && !isJumping_)
            {
                wasOnGround = isGrounded_;
                //Ground Check
                if (Physics2D.OverlapBox(groundCheckPoint_.position, groundCheckSize_, 0, groundLayer_))
                {
                    lastOnGroundTime = data.coyoteTime;
                    isGrounded_ = true;
                }
                else
                {
                    isGrounded_ = false;
                }
                if (!wasOnGround && isGrounded_)
                {
                    animator.SetTrigger("LandingTrigger");
                    animator.SetBool("IsJumpFalling", false);
                    Debug.Log("Test");
                }
                //Right Wall Check
                if (((Physics2D.OverlapBox(frontWallCheckPoint_.position, wallCheckSize_, 0, groundLayer_) && isFacingRight)
                        || (Physics2D.OverlapBox(backWallCheckPoint_.position, wallCheckSize_, 0, groundLayer_) && !isFacingRight)))
                {
                    lastOnWallRightTime = data.coyoteTime;
                }
                //Left Wall Check
                if (((Physics2D.OverlapBox(frontWallCheckPoint_.position, wallCheckSize_, 0, groundLayer_) && !isFacingRight)
                    || (Physics2D.OverlapBox(backWallCheckPoint_.position, wallCheckSize_, 0, groundLayer_) && isFacingRight)))
                {
                    lastOnWallLeftTime = data.coyoteTime;
                }

                lastOnWallTime = Mathf.Max(lastOnWallLeftTime, lastOnWallRightTime);
            }
            #endregion

            #region JUMP
            OnJump();

            if (rb_.velocity.y < 0 && isJumping_)
            {
                isJumping_ = false;
                animator.SetBool("IsJumping", false);
            }
            if (rb_.velocity.y > 0 && isJumping_)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsJumpFalling", false);
            }

            if (rb_.velocity.y < 0 && !isGrounded_)
            {
                animator.SetBool("IsJumpFalling", true);
            }

            if (isWallJumping_ && Time.time - wallJumpStartTime_ > data.wallJumpTime)
            {
                isWallJumping_ = false;
            }

            if (!isDashing_)
            {
                if (CanJump() && lastPressedJumpTime > 0)
                {
                    isJumping_ = true;
                    isWallJumping_ = false;
                    isJumpCut_ = false;
                    Jump();
                }

                else if (CanWallJump() && lastPressedJumpTime > 0)
                {
                    isWallJumping_ = true;
                    isJumping_ = false;
                    isJumpCut_ = false;

                    wallJumpStartTime_ = Time.time;
                    lastWallJumpDir_ = (lastOnWallRightTime > 0) ? -1 : 1;

                    animator.SetBool("IsJumping", false);
                    WallJump(lastWallJumpDir_);
                }
            }

            //Jump Cut
            if (Input.GetKeyUp(KeyCode.Space))
            {
                OnJumpUp();
            }
            #endregion

            #region DASH CHECKS
            if (CanDash() && LastPressedDashTime > 0)
            {
                Sleep(data.dashSleepTime);

                if (moveInput_ != Vector2.zero)
                {
                    _lastDashDir = moveInput_;
                }
                else
                {
                    _lastDashDir = isFacingRight ? Vector2.right : Vector2.left;
                }

                isDashing_ = true;
                isJumping_ = false;
                isJumpCut_ = false;

                animator.SetBool("IsJumping", false);
                StartCoroutine(nameof(StartDash), _lastDashDir);
            }

            #endregion

            #region SLIDE CHECKS

            if (CanSlide() && ((lastOnWallLeftTime > 0 && moveInput_.x < 0) ||
                    (lastOnWallRightTime > 0 && moveInput_.x > 0)))
            {
                isSliding_ = true;
            }
            else
            {
                isSliding_ = false;
            }

            #endregion

            #region Gravity
            if (!isDashAttacking_)
            {
                if (isSliding_)
                {
                    SetGravityScale(0.2f);
                }
                else if (rb_.velocity.y < 0)
                {
                    SetGravityScale(data.gravityScale * data.fallGravityMult);
                    rb_.velocity = new Vector2(rb_.velocity.x, Mathf.Max(rb_.velocity.y, -data.fallClamp_));
                }
                else if (isJumpCut_)
                {
                    SetGravityScale(data.gravityScale * data.jumpCutGravityMult);
                    rb_.velocity = new Vector2(rb_.velocity.x, Mathf.Max(rb_.velocity.y, -data.fallClamp_));
                }
                else if (isJumping_ && Mathf.Abs(rb_.velocity.y) < data.jumpApexTimeThreshold)
                {
                    SetGravityScale(data.gravityScale * data.jumpApexGravityMult);
                    Debug.Log("Apex");
                }
                else
                {
                    SetGravityScale(data.gravityScale);
                }
            }
            else
            {
                SetGravityScale(0);
            }

            #endregion



            if (lastOnGroundTime > 0 && Mathf.Abs(moveInput_.x) < 0.01f)
            {
                float amount = Mathf.Min(Mathf.Abs(rb_.velocity.x), Mathf.Abs(data.frictionAmount));
                amount *= Mathf.Sign(rb_.velocity.x);
                rb_.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
            }
        }

        private void FixedUpdate()
        {
            #region INPUT HANDLER
            if (!isDashing_)
            {
                if (isWallJumping_)
                {
                    Run(data.wallJumpRunLerp);
                }
                else
                {
                    Run(1);
                }
            }

            else if (isDashAttacking_)
            {
                Run(data.dashEndRunLerp);
            }
            #endregion

            if (isSliding_)
            {
                Slide();
            }
        }

        private void StartAction()
        {

        }

        private void Run(float lerpAmounts)
        {
            float targetSpeed = moveInput_.x * data.moveSpeed_;
            targetSpeed = Mathf.Lerp(rb_.velocity.x, targetSpeed, lerpAmounts);

            float accelRate;

            if (lastOnGroundTime > 0)
            {
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.acceleration_ : data.deccelaration_;
            }
            else
            {
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.acceleration_ * data.accelInAir : data.deccelaration_ * data.deccelInAir;
            }

            if ((isJumping_ || isWallJumping_) && Mathf.Abs(rb_.velocity.y) < data.jumpApexTimeThreshold)
            {
                accelRate *= data.jumpApexAccelerationMult;
                targetSpeed *= data.jumpApexMaxSpeedMult;
            }

            float speedDiff = targetSpeed - rb_.velocity.x;

            float movement = speedDiff * accelRate;
            animator.SetFloat("Speed", Mathf.Abs(moveInput_.x));
            rb_.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }

        private void Jump()
        {
            lastPressedJumpTime = 0;
            lastOnGroundTime = 0;

            #region Perform Jump
            float force = data.jumpForce_;
            if (rb_.velocity.y < 0)
                force -= rb_.velocity.y;

            rb_.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            #endregion
        }

        private bool CanWallJump()
        {
            return lastPressedJumpTime > 0 && lastOnWallTime > 0 && lastOnGroundTime <= 0 &&
                (!isWallJumping_ || (lastOnWallRightTime > 0 && lastWallJumpDir_ == 1)
                || (lastOnWallLeftTime > 0 && lastWallJumpDir_ == -1));
        }

        private bool CanWallJumpCut()
        {
            return isWallJumping_ && rb_.velocity.y > 0f;
        }

        private void WallJump(int dir)
        {
            lastPressedJumpTime = 0;
            lastOnGroundTime = 0;
            lastOnWallRightTime = 0;
            lastOnWallLeftTime = 0;

            Vector2 force = new Vector2(data.wallJumpForce.x, data.wallJumpForce.y);
            force.x *= dir;

            if (Mathf.Sign(rb_.velocity.x) != Mathf.Sign(force.x))
            {
                force.x -= rb_.velocity.x;
            }
            if (rb_.velocity.y < 0)
            {
                force.y -= rb_.velocity.y;
            }

            rb_.AddForce(force, ForceMode2D.Impulse);
        }
        private void OnDashInput()
        {
            LastPressedDashTime = data.dashInputBufferTime;
        }

        private bool CanDash()
        {
            //if (!isDashing_ && dashesLeft_ < dashAmount && lastOnGroundTime > 0 && !dashRefilling_)
            //{
            //    StartCoroutine(nameof(RefillDash), 1);
            //}

            return dashesLeft_ > 0;
        }

        //private IEnumerator RefillDash(int amount)
        //{
        //    dashRefilling_ = true;
        //    yield return Helpers.GetWaitForSeconds(dashRefillTime);
        //    dashRefilling_ = false;
        //    dashesLeft_ = Mathf.Min(dashAmount, dashesLeft_ + 1);
        //}

        public void RefillDash()
        {
            dashesLeft_++;
            dashesLeft_ = Mathf.Min(data.dashAmount, dashesLeft_);
            OnDashesRefilled?.Invoke();
        }

        private IEnumerator StartDash(Vector2 dir)
        {

            lastOnGroundTime = 0;
            LastPressedDashTime = 0;

            float startTime = Time.time;

            dashesLeft_--;

            OnDashesUsed?.Invoke();
            isDashAttacking_ = true;

            SetGravityScale(0);

            while (Time.time - startTime <= data.dashAttackTime)
            {
                rb_.velocity = dir.normalized * data.dashSpeed;

                yield return null;
            }

            startTime = Time.time;

            isDashAttacking_ = false;

            SetGravityScale(data.gravityScale);

            rb_.velocity = data.dashEndSpeed * dir.normalized;

            while (Time.time - startTime <= data.dashEndTime)
            {
                yield return null;
            }

            isDashing_ = false;
        }

        private bool CanJump()
        {
            return lastOnGroundTime > 0 && !isJumping_;
        }

        public void OnJump()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                lastPressedJumpTime = data.jumpInputBufferTime;
            }
        }

        public void OnJumpUp()
        {
            if (CanJumpCut() || CanWallJumpCut())
            {
                isJumpCut_ = true;
            }
        }

        private bool CanJumpCut()
        {
            return isJumping_ && rb_.velocity.y > 0;
        }

        private void JumpCut()
        {
            rb_.AddForce(Vector2.down * rb_.velocity * (1f - data.jumpCutGravityMult), ForceMode2D.Impulse);
        }

        private bool CanSlide()
        {
            if (lastOnWallTime > 0 && !isJumping_ && !isWallJumping_ && !isDashing_ && lastOnGroundTime <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void Slide()
        {
            if (data.useSlideForce)
            {
                float speedDif = data.slideSpeed - rb_.velocity.y;
                float movement = speedDif * -data.slideAccel;

                movement =
                    Mathf.Clamp(
                        movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

                rb_.AddForce(movement * Vector2.up);
            }
            else
            {
                rb_.velocity = -data.slideVelocity;
            }
        }

        public void SetGravityScale(float scale)
        {
            rb_.gravityScale = scale;
        }

        private void Sleep(float duration)
        {
            StartCoroutine(nameof(PerformSleep), duration);
        }

        private IEnumerator PerformSleep(float duration)
        {
            Time.timeScale = 0;
            yield return Helpers.GetWaitForSecondsRealTime(duration);
            Time.timeScale = 1;
        }

        private void Turn()
        {
            playerSprite.flipX = !playerSprite.flipX;

            Vector3 wallCheckScale = wallCheckTransform.localScale;
            wallCheckScale.x *= -1f;
            wallCheckTransform.localScale = wallCheckScale;

            isFacingRight = !isFacingRight;
        }

        #region CHECK METHODS
        public void CheckDirectionToFace(bool isMovingRight)
        {
            if (isMovingRight != isFacingRight)
                Turn();
        }
        #endregion

        public int GetDashesLeft_()
        {
            return dashesLeft_;
        }

        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheckPoint_.position, groundCheckSize_);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(frontWallCheckPoint_.position, wallCheckSize_);
            Gizmos.DrawWireCube(backWallCheckPoint_.position, wallCheckSize_);
        }
    }
}