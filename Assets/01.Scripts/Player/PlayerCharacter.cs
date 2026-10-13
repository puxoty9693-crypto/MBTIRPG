using UnityEngine;
using System;
using Unity.VisualScripting;


[RequireComponent(typeof(PlayerMotor), typeof(Health))]
public class PlayerCharacter : MonoBehaviour
{
    PlayerMotor motor;
    PlayerAttack attack;
    Health health;
    Rigidbody2D rb;

    public Health Health => health;
    public bool IsDead => health != null && health.IsDead;
    public event Action<PlayerCharacter> OnDied;

    private void Awake()
    {
        motor = GetComponent<PlayerMotor>();
        attack = GetComponent<PlayerAttack>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();

        // 대시 중에는 Health 무적판정
        health.InvincibleCheck = () => motor.IsInvincible;
    }

    private void OnEnable()
    {
        PlayerManager.Register(this);
        health.Ondied += HandleDied;
    }

    private void OnDisable()
    {
        PlayerManager.Unregister(this);
        health.Ondied -= HandleDied;
    }

    private void HandleDied() 
    {
        motor.enabled = false;
        if (attack != null) attack.enabled = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        GameLog.Log("Player", $"{name} 사망");
        OnDied?.Invoke(this);
        PlayerManager.NotifyPlayerDied(this);
    }
}
