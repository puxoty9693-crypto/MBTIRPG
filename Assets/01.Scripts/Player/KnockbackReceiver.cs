using UnityEngine;

public interface IKnockbackable 
{
    void ApplyKnockback(Vector2 velocity, float duration);
}


[RequireComponent(typeof(Health))]
public class KnockbackReceiver : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] float resistance = 1f; // 1= 그대로 밀림, 0 = 안 밀림 (보스는 0)
    [SerializeField] float upForce = 3f; //살짝 위로 띄우는 힘
    [SerializeField] float duration = 0.15f; //조작 불가 시간

    Health health;
    Rigidbody2D rb;
    IKnockbackable handler;

    private void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        handler = GetComponent<IKnockbackable>();
    }

    private void OnEnable() => health.OnDamaged += HandleDamaged;
    private void OnDisable() => health.OnDamaged -= HandleDamaged;

    void HandleDamaged(float amount, DamageInfo info) 
    {
        if (info.knockback <= 0f || resistance <= 0f || health.IsDead) return;

        float dir = info.hitDirection.x != 0f ? Mathf.Sign(info.hitDirection.x) : 1f;
        Vector2 velocity = new Vector2(dir * info.knockback, upForce) * resistance;

        if (handler != null) handler.ApplyKnockback(velocity, duration);
        else if (rb != null) rb.linearVelocity = velocity;
    }
}
