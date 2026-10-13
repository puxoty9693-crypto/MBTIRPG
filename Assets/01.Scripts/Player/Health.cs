using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHP = 100f;
    [SerializeField, Range(0f, 1f)] float defenseReduction = 0f; //방어 감소율 (0.2 = 20% 덜 받음)
    [SerializeField] bool deactivateOnDeath = false; //적은 체크
    [SerializeField] float hitInvincibleTime = 0f; // 맞은 뒤 무적 시간 (플레이어 0.5, 적 0)

    float hitInvincibleTimer;

    public float MaxHP => maxHP;
    public float CurrentHP { get; private set; }
    public float DefenseReduction 
    {
        get => defenseReduction;
        set => defenseReduction = Mathf.Clamp01(value);
    }
    public bool IsDead { get; private set; }

    //무적 여부는 외부에서 연결
    public Func<bool> InvincibleCheck;

    public bool IsHitInvincible => hitInvincibleTimer > 0f;
    public bool IsInvincible => IsHitInvincible || (InvincibleCheck != null && InvincibleCheck());

    public event Action<float, DamageInfo> OnDamaged; // (받은 데미지, 공격 정보)
    public event Action Ondied;

    void Awake() 
    {
        CurrentHP = maxHP;
    }

    private void Update()
    {
        if (hitInvincibleTime > 0f) hitInvincibleTimer -= Time.deltaTime;
    }

    // CombatSystem에서만 호출
    public void ApplyDamage(float amount, DamageInfo info) 
    {
        if (IsDead) return;

        CurrentHP = Mathf.Max(0f, CurrentHP - amount);
        hitInvincibleTimer = hitInvincibleTime;
        OnDamaged?.Invoke(amount, info);

        if (CurrentHP <= 0f) 
        {
            IsDead = true;
            GameLog.Log("Combat", $"{name} 사망", this);
            Ondied?.Invoke();
            if (deactivateOnDeath) gameObject.SetActive(false);
        }
    }

    //재시작 /풀에서 다시 꺼낼 때
    public void Revive() 
    {
        IsDead = false;
        CurrentHP = maxHP;
        hitInvincibleTimer = 0f;
    }


    //클래스 데이터 / 영구 강화에서 최대 체력 설정할 때 사용
    public void SetMaxHP(float value, bool refill = true) 
    {
        maxHP = Mathf.Max(1f, value);
        if (refill) CurrentHP = maxHP;
        else CurrentHP = Mathf.Min(CurrentHP, maxHP);
    }
}
