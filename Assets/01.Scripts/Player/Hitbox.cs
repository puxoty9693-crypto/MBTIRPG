using System.Collections.Generic;
using UnityEngine;


// 떄리는 판정의 영역
// 중복 타격 방지
public class Hitbox : MonoBehaviour
{
    [SerializeField] Vector2 offset = new Vector2(0.8f, 0f); // 캐릭터 중심에서 얼마나 앞에
    [SerializeField] Vector2 size = new Vector2(1.2f, 1f);
    [SerializeField] LayerMask targetMask; //플레이어 공격이면 Enemy

    readonly HashSet<Health> hitThisAttack = new HashSet<Health>();
    readonly List<Collider2D> results = new List<Collider2D>();
    ContactFilter2D filter;

    DamageInfo current;
    float activeTimer;
    int facing = 1;
    Health self;

    public bool IsActive => activeTimer > 0f;

    private void Awake()
    {
        self = GetComponentInParent<Health>(); // 자기 자신은 안 때리게
        filter = new ContactFilter2D();
        filter.useTriggers = true;
    }

    public void Activate(DamageInfo info, float activeTime, int facingDir) 
    {
        current = info;
        facing = facingDir >= 0 ? 1 : -1;
        activeTimer = activeTime;
        hitThisAttack.Clear();
        filter.SetLayerMask(targetMask);
        CheckHits(); // 누른 즉시 한 번 판정
    }

    private void FixedUpdate()
    {
        if (activeTimer <= 0f) return;
        activeTimer -= Time.fixedDeltaTime;
        CheckHits();
    }

    Vector2 Center => (Vector2)transform.position + new Vector2(offset.x * facing, offset.y);

    void CheckHits() 
    {
        results.Clear();
        Physics2D.OverlapBox(Center, size, 0f, filter, results);

        foreach (Collider2D col in results) 
        {
            if (!col.TryGetComponent(out Hurtbox hurtbox)) continue;

            Health target = hurtbox.Owner;
            if (target == null || target == self) continue;
            if (!hitThisAttack.Add(target)) continue; //이번 공격에 이마 맞은 대상

            DamageInfo info = current;
            info.hitDirection = new Vector2(facing, 0f);
            CombatSystem.ApplyHit(info, target);
        }
    }

    private void OnDrawGizmos()
    {
        //노란 테두리 = 공격 범위, 빨강 = 판정 켜짐
        Gizmos.color = IsActive ? Color.red : new Color(1f, 0.9f, 0f, 0.5f);
        Gizmos.DrawWireCube(Center, size);
    }
}
