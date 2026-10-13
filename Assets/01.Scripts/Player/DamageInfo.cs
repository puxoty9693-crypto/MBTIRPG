using UnityEngine;

public struct DamageInfo
{
    public GameObject attacker;
    public float attackPower; // 공격자의 공격력
    public float multiplier; // 스킬 계수 (기본 공격 = 1)
    public Vector2 hitDirection; // 맞은 방향 (넉백에서 사용)
    public float knockback; // 넉백 세기

    public DamageInfo(GameObject attacker, float attackPower, float multiplier = 1f, float knockback = 0f) 
    {
        this.attacker = attacker;
        this.attackPower = attackPower;
        this.multiplier = multiplier;
        this.knockback = knockback;
        hitDirection = Vector2.zero;
    }
}
