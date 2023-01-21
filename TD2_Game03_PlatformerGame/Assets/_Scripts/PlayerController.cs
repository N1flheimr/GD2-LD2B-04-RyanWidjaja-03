using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private PlayerInput controls;

    #region COMPONENTS
    private Rigidbody2D rb_;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Transform wallCheckTransform;
    #endregion

    #region STATE PARAMETERS
    private bool isGrounded_;
    private bool isOnRightWall_;
    private bool isOnLeftWall_;
    private bool isJumping_;
    private bool isSliding_;

    private bool isDashing_;

    private bool isJumpCut_;

    //Dash
    private int dashesLeft_;
    private bool dashRefilling_;
    private Vector2 _lastDashDir;
    private bool isDashAttacking_ = false;


    private bool wasOnGround = true;

    private bool isWallJumping_;
    private int lastWallJumpDir_;
    private float wallJumpStartTime_;
    //public bool useWallJump = true;

    public float lastOnGroundTime { get; private set; }
    public float lastOnWallTime { get; private set; }
    public float lastOnWallRightTime { get; private set; }
    public float lastOnWallLeftTime { get; private set; }
    public float LastPressedDashTime { get; private set; }
    #endregion


    #region INPUT PARAMETERS
    private Vector2 moveInput_;
    public float lastPressedJumpTime { get; private set; }
    #endregion

    #region CHECK PARAMETERS
    [Header("Checks")]
    [SerializeField] private Transform groundCheckPoint_;
    [SerializeField] private Vector2 groundCheckSize_;
    [Space(5)]
    [SerializeField] private Transform frontWallCheckPoint_;
    [SerializeField] private Transform backWallCheckPoint_;
    [SerializeField] private Vector2 wallCheckSize_;
    #endregion

    #region Layers & Tags
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask groundLayer_;
    [Space(5)]
    #endregion

    [Header("Run Parameters")]
    [SerializeField] private float moveSpeed_;
    [SerializeField] private float velPower_;
    [SerializeField] private float acceleration_;
    [SerializeField] private float deccelaration_;

    [Range(0, 1f)]
    [SerializeField] private float accelInAir;
    [Range(0, 1f)]
    [SerializeField] private float deccelInAir;

    [Space(5)]

    [SerializeField] private float fallClamp_;
    [SerializeField] private float jumpCutGravityMult;

    [Range(0, 0.5f)] public float coyoteTime;
    [Range(0, 1.5f)] public float jumpBufferTime;

    [SerializeField] private float jumpApexTimeThreshold;

    [Range(0, 1f)]
    [SerializeField] private float jumpApexGravityMult;

    [SerializeField] private float jumpApexAccelerationMult;
    [SerializeField] private float jumpApexMaxSpeedMult;
    [Space(5)]

    [Header("Wall Jump Parameters")]
    [SerializeField] private Vector2 wallJumpForce;
    [Range(0, 1f)][SerializeField] private float wallJumpRunLerp;
    [Range(0, 0.5f)][SerializeField] private float wallJumpTime;
    [Space(5)]

    public float fallGravityMult;
    public float gravityScale;
    public float frictionAmount;
    public float jumpForce_;
    [Space(5)]

    [Header("Dash Parameters")]
    [Range(0, 0.5f)] public float dashInputBufferTime;
    [Range(0f, 1f)] public float dashEndRunLerp;

    [SerializeField] private int dashAmount;
    [SerializeField] private float dashRefillTime;
    [SerializeField] private float dashSleepTime;
    [SerializeField] private float dashAttackTime;
    [SerializeField] private float dashEndTime;

    [SerializeField] private float dashSpeed;
    [SerializeField] private Vector2 dashEndSpeed;
    [Space(5)]


    [Header("Slide Parameters")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideAccel;
    [SerializeField] private Vector2 slideVelocity;
    [SerializeField] private bool useSlideForce;

    private bool isFacingRight;

    private void Awake()
    {
        if (rb_ == null)
            rb_ = GetComponent<Rigidbody2D>();

        if (Instance == null)
        {
            Instance = this;
        }

        controls = new PlayerInput();
    }

    private void Start()
    {
        isFacingRight = true;
        SetGravityScale(gravityScale);

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
            //Ground Check
            if (Physics2D.OverlapBox(groundCheckPoint_.position, groundCheckSize_, 0, groundLayer_))
            {
                lastOnGroundTime = coyoteTime;
            }
            //Right Wall Check
            if (((Physics2D.OverlapBox(frontWallCheckPoint_.position, wallCheckSize_, 0, groundLayer_) && isFacingRight)
                    || (Physics2D.OverlapBox(backWallCheckPoint_.position, wallCheckSize_, 0, groundLayer_) && !isFacingRight)))
            {
                lastOnWallRightTime = coyoteTime;
            }
            //Left Wall Check
            if (((Physics2D.OverlapBox(frontWallCheckPoint_.position, wallCheckSize_, 0, groundLayer_) && !isFacingRight)
                || (Physics2D.OverlapBox(backWallCheckPoint_.position, wallCheckSize_, 0, groundLayer_) && isFacingRight)))
            {
                lastOnWallLeftTime = coyoteTime;
            }

            lastOnWallTime = Mathf.Max(lastOnWallLeftTime, lastOnWallRightTime);
        }
        #endregion

        #region JUMP
        OnJump();

        if (rb_.velocity.y < 0 && isJumping_)
        {
            isJumping_ = false;
        }

        if (isWallJumping_ && Time.time - wallJumpStartTime_ > wallJumpTime)
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
            Sleep(dashSleepTime);

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
                SetGravityScale(gravityScale * fallGravityMult);
                rb_.velocity = new Vector2(rb_.velocity.x, Mathf.Max(rb_.velocity.y, -fallClamp_));
            }
            else if (isJumpCut_)
            {
                SetGravityScale(gravityScale * jumpCutGravityMult);
                rb_.velocity = new Vector2(rb_.velocity.x, Mathf.Max(rb_.velocity.y, -fallClamp_));
            }
            else if (isJumping_ && Mathf.Abs(rb_.velocity.y) < jumpApexTimeThreshold)
            {
                SetGravityScale(gravityScale * jumpApexGravityMult);
            }
            else
            {
                SetGravityScale(gravityScale);
            }
        }
        else
        {
            SetGravityScale(0);
        }

        #endregion



        if (lastOnGroundTime > 0 && Mathf.Abs(moveInput_.x) < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb_.velocity.x), Mathf.Abs(frictionAmount));
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
                Run(wallJumpRunLerp);
            }
            else
            {
                Run(1);
            }
        }

        else if (isDashAttacking_)
        {
            Run(dashEndRunLerp);
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
        float targetSpeed = moveInput_.x * moveSpeed_;
        targetSpeed = Mathf.Lerp(rb_.velocity.x, targetSpeed, lerpAmounts);

        float accelRate;

        if (lastOnGroundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration_ : deccelaration_;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration_ * accelInAir : deccelaration_ * deccelInAir;
        }

        if ((isJumping_ || isWallJumping_) && Mathf.Abs(rb_.velocity.y) < jumpApexTimeThreshold)
        {
            accelRate *= jumpApexAccelerationMult;
            targetSpeed *= jumpApexMaxSpeedMult;
        }

        float speedDiff = targetSpeed - rb_.velocity.x;

        float movement = speedDiff * accelRate;
        rb_.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Jump()
    {
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;

        #region Perform Jump
        float force = jumpForce_;
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

        Vector2 force = new Vector2(wallJumpForce.x, wallJumpForce.y);
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
        LastPressedDashTime = dashInputBufferTime;
    }

    private bool CanDash()
    {
        if (!isDashing_ && dashesLeft_ < dashAmount && lastOnGroundTime > 0 && !dashRefilling_)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return dashesLeft_ > 0;
    }

    private IEnumerator RefillDash(int amount)
    {
        dashRefilling_ = true;
        yield return Helpers.GetWaitForSeconds(dashRefillTime);
        dashRefilling_ = false;
        dashesLeft_ = Mathf.Min(dashAmount, dashesLeft_ + 1);
    }

    private IEnumerator StartDash(Vector2 dir)
    {
        lastOnGroundTime = 0;
        LastPressedDashTime = 0;

        float startTime = Time.time;

        dashesLeft_--;
        isDashAttacking_ = true;

        SetGravityScale(0);

        while (Time.time - startTime <= dashAttackTime)
        {
            rb_.velocity = dir.normalized * dashSpeed;

            yield return null;
        }

        startTime = Time.time;

        isDashAttacking_ = false;

        SetGravityScale(gravityScale);

        rb_.velocity = dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= dashEndTime)
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
            lastPressedJumpTime = jumpBufferTime;
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
        rb_.AddForce(Vector2.down * rb_.velocity * (1f - jumpCutGravityMult), ForceMode2D.Impulse);
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
        if (useSlideForce)
        {
            float speedDif = slideSpeed - rb_.velocity.y;
            float movement = speedDif * -slideAccel;

            movement =
                Mathf.Clamp(
                    movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

            rb_.AddForce(movement * Vector2.up);
        }
        else
        {
            rb_.velocity = -slideVelocity;
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