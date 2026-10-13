using UnityEngine;
using System.Collections;
using System;

public enum DashType { Dash, Teleport }


[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMotor : MonoBehaviour, IKnockbackable
{
    //이동
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float acceleration = 70f;  //초당 속도 증가량
    [SerializeField] float deceleration = 90f;  //입력 됐을 때 감속량
    [SerializeField, Range(0f, 1f)] float airControl = 0.7f;

    //점프
    [SerializeField] float jumpVelocity = 15f;
    [SerializeField] float coyoteTime = 0.1f;     // 발판에서 떨어진 직후에도 점프 허용
    [SerializeField] float jumpBufferTime = 0.1f; //착지 직전에 누른 점프 기억
    [SerializeField, Range(0f, 1f)] float jumpCutMultiplier = 0.5f; // 버튼 뗴면 상승 속도 감소 (1= 끔)

    //중력
    [SerializeField] float gravityScale = 3.5f;
    [SerializeField] float fallGravity = 1.6f;
    [SerializeField] float maxFallSpeed = 22f;

    //대시
    [SerializeField] float dashSpeed = 22f;
    [SerializeField] float dashDuration = 0.15f;
    [SerializeField] DashType dashType = DashType.Dash;  // 마법사 = Teleport
    [SerializeField] float teleportDistance = 4f;
    [SerializeField] float dashCooldown = 0.6f;
    [SerializeField] float dashInvincibleExtra = 0.05f;
    [SerializeField] int maxAirDashes = 1;

    //지면 판정
    [SerializeField] LayerMask groundMask;  // 그라운드 체크
    [SerializeField] LayerMask platformMask;
    [SerializeField] float groundCheckHeight = 0.08f;
    [SerializeField] float platformDropTime = 0.25f; //발판 통과 시간


    Rigidbody2D rb;
    Collider2D col;
    IInputSource input;

    float moveX;
    bool jumpHeld;
    float coyoteTimer;
    float jumpBufferTimer;
    bool isJumping;
    bool downHeld;
    bool dashRequested;

    float dashTimer;
    float dashCooldownTimer;
    float invincibleTimer;
    int airDashesUsed;
    int dashDir = 1;

    float dropTimer;
    float knockbackTimer;

    public bool IsGrounded { get; private set; }
    public bool IsDashing => dashTimer > 0f;
    public bool IsKnockedBack => knockbackTimer > 0f;
    public bool IsInvincible => invincibleTimer > 0f;
    public float DashCooldownRatio => dashCooldown <= 0f ? 0f : Mathf.Clamp01(dashCooldownTimer /  dashCooldown);

    public event Action OnJump;
    public event Action OnDash;

    public int Facing { get; private set; } = 1;
    public Vector2 Velocity => rb.linearVelocity;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public DashType DashMode { get => dashType; set => dashType = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        input = GetComponent<IInputSource>();
        if (input == null) GameLog.Error("Player", "인풋소스 컴포넌트 없음", this);

        rb.gravityScale = gravityScale;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Update()
    {
        if (input == null) return;

        moveX = input.MoveX;
        jumpHeld = input.JumpHeld;
        downHeld = input.DownHeld;
        if (input.DashPressed) dashRequested = true;
        if (input.JumpPressed) jumpBufferTimer = jumpBufferTime;
        else jumpBufferTimer -= Time.deltaTime;

        if (!IsDashing)
        {
            if (moveX > 0.01f) Facing = 1;
            else if (moveX < -0.01f) Facing = -1;
        }

    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        TickTimers(dt);
        Vector2 v = rb.linearVelocity;

        CheckGround(v.y);

        if (IsGrounded) { coyoteTimer = coyoteTime; isJumping = false; airDashesUsed = 0; }
        else coyoteTimer -= dt;

        if (dashRequested) 
        {
            dashRequested = false;
            if (CanDash()) StartDash();
        }

        if (IsDashing) 
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0f);
            return;
        }

        if (IsKnockedBack) 
        {
            rb.gravityScale = gravityScale;
            return;
        }

        float target = moveX * moveSpeed;
        float rate = Mathf.Abs(target) > 0.01f ? acceleration : deceleration;
        if (!IsGrounded) rate *= airControl;
        v.x = Mathf.MoveTowards(v.x, target, rate*dt);

        // 발판 하강 (아래 + 점프)
        if (jumpBufferTimer > 0f && downHeld && IsGrounded && TryDropThroughPlatform())
        {
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        //점프 (버퍼 + 코요테)
        else if (jumpBufferTimer > 0f && coyoteTimer > 0f) 
        {
            v.y = jumpVelocity;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            isJumping = true;
            IsGrounded = false;
            OnJump?.Invoke();
            GameLog.Log("Player", "Jump");
        }
        
        // 버튼 뗴면 낮은 점프
        if (isJumping && !jumpHeld && v.y > 0f) 
        {
            v.y *= jumpCutMultiplier;
            isJumping = false;
        }

        rb.gravityScale = v.y < 0f ? gravityScale * fallGravity : gravityScale;
        if (v.y < -maxFallSpeed) v.y = -maxFallSpeed;

        rb.linearVelocity = v;

    }

    void TickTimers(float dt) 
    {
        if (dashTimer > 0f) dashTimer -= dt;
        if (dashCooldownTimer > 0f) dashCooldownTimer -= dt;
        if (invincibleTimer > 0f) invincibleTimer -= dt;
        if (dropTimer > 0f) dropTimer -= dt;
        if (knockbackTimer > 0f) knockbackTimer -= dt;
    }

    bool CanDash() 
    {
        if (IsDashing || IsKnockedBack || dashCooldownTimer > 0f) return false;
        if (!IsGrounded && airDashesUsed >= maxAirDashes) return false;
        return true;
    }
    
    void StartDash() 
    {
        dashDir = Mathf.Abs(moveX) > 0.01f ? (int)Mathf.Sign(moveX) : Facing;
        Facing = dashDir;

        dashCooldownTimer = dashCooldown;

        if (dashType == DashType.Teleport)
        {
            Teleport();
            dashTimer = 0f;                                  // 이동 구간 없음
            invincibleTimer = 0.1f + dashInvincibleExtra;    // 순간이동 직후 짧은 무적
        }
        else
        {
            dashTimer = dashDuration;
            invincibleTimer = dashDuration + dashInvincibleExtra;
        }

        if (!IsGrounded) airDashesUsed++;

        isJumping = false;
        jumpBufferTimer = 0f;

        OnDash?.Invoke();
        GameLog.Log("Player", "Dash");
    }

    public void ApplyKnockback(Vector2 velocity, float duration) 
    {
        if (IsDashing) return;
        knockbackTimer = duration;
        isJumping = false;
        rb.gravityScale = gravityScale;
        rb.linearVelocity = velocity;
    }

    bool TryDropThroughPlatform() 
    {
        Bounds b = col.bounds;
        Vector2 center = new Vector2(b.center.x, b.min.y);
        Vector2 size = new Vector2(b.size.x * 0.9f, groundCheckHeight);

        if (Physics2D.OverlapBox(center, size, 0f, groundMask & ~platformMask) != null) return false;

        Collider2D platform  = Physics2D.OverlapBox(center, size, 0f, groundMask & platformMask);
        if (platform == null) return false;

        StartCoroutine(IgnorePlatform(platform));
        GameLog.Log("Player", "Drop through platform");
        return true;
    }

    IEnumerator IgnorePlatform(Collider2D platform) 
    {
        dropTimer = platformDropTime;
        IsGrounded = false;
        Physics2D.IgnoreCollision(col, platform, true);
        yield return new WaitForSeconds(platformDropTime);
        if (platform != null) Physics2D.IgnoreCollision(col, platform, false);
    }
     
    void CheckGround(float vy) 
    {
        //위로 올라가는 중에는 접지 아님
        if (vy > 0.01f) { IsGrounded = false; return; }

        Bounds b = col.bounds;
        Vector2 center = new Vector2(b.center.x, b.min.y);
        Vector2 size = new Vector2(b.size.x * 0.9f, groundCheckHeight);
        LayerMask mask = dropTimer > 0f ? (LayerMask)(groundMask & ~platformMask) : groundMask;
        IsGrounded = Physics2D.OverlapBox(center, size, 0f, mask) != null;
    }

    void OnDrawGizmosSelected() 
    {
        var c = GetComponent<Collider2D>();
        if (c == null) return;
        Bounds b = c.bounds;
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(new Vector3(b.center.x, b.min.y), new Vector3(b.size.x * 0.9f, groundCheckHeight));
        
    }

    void Teleport()
    {
        // 벽 앞에서 멈추도록 콜라이더 크기만큼 BoxCast (발판은 무시)
        Bounds b = col.bounds;
        Vector2 dir = new Vector2(dashDir, 0f);
        RaycastHit2D hit = Physics2D.BoxCast(b.center, b.size * 0.95f, 0f, dir, teleportDistance, groundMask & ~platformMask);
        float dist = hit ? Mathf.Max(0f, hit.distance - 0.02f) : teleportDistance;

        rb.position += dir * dist;
        rb.linearVelocity = Vector2.zero;
        GameLog.Log("Player", $"Teleport {dist:F1}");
    }


}
