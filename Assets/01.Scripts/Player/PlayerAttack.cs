using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Hitbox hitbox;
    [SerializeField] float attackPower = 16f;           // 검사 기준
    [SerializeField] float attackCooldown = 0.35f;      // 공격 속도
    [SerializeField] float activeTime = 0.1f;           // 판정 유지 시간
    [SerializeField] float attackBuffer = 0.15f;        // 쿨타임 끝나기 직전에 누른 공격 기억
    [SerializeField] float knockbackForce = 6f;         // 적을 밀어내는 힘


    IInputSource input;
    PlayerMotor motor;
    float cooldownTimer;
    float bufferTimer;

    public float AttackPower { get => attackPower; set => attackPower = value; }
    public float Cooldown { get => attackCooldown; set => attackCooldown = value; }
    public event Action OnAttack;

    private void Awake()
    {
        input = GetComponent<IInputSource>();
        motor = GetComponent<PlayerMotor>();
        if (hitbox == null) hitbox = GetComponentInChildren<Hitbox>();
        if (hitbox == null) GameLog.Error("Player", "Hitbox가 없습니다. 자식오브젝트에 Hitbox를 붙이세요.", this);
    }

    void Update()
    {
        if (input == null || hitbox == null) return;
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (input.AttackPressed) bufferTimer = attackBuffer;
        else bufferTimer -= Time.deltaTime;

        if (bufferTimer > 0f && cooldownTimer <= 0f && !motor.IsDashing && !motor.IsKnockedBack) 
        {
            bufferTimer = 0f;
            cooldownTimer = attackCooldown;

            var info = new DamageInfo(gameObject, attackPower, 1f, knockbackForce);
            hitbox.Activate(info, activeTime, motor.Facing);

            OnAttack?.Invoke();
            GameLog.Log("Player", "Attack");
        }
    }
}
