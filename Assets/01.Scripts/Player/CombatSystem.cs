using UnityEngine;

public static class CombatSystem
{
   public static float CalculatDamage(float attackPower, float multiplier, float defenseReduction) 
   {
        float damage = attackPower * multiplier * (1f - Mathf.Clamp01(defenseReduction));
        return Mathf.Max(0f, damage);
   }

   public static bool ApplyHit(DamageInfo info, Health target) 
   {
        if (target == null || target.IsDead || target.IsInvincible) return false;

        float damage = CalculatDamage(info.attackPower, info.multiplier, target.DefenseReduction);
        target.ApplyDamage(damage, info);

        string attackerName = info.attacker != null ? info.attacker.name : "?";
        GameLog.Log("Combat", $"{attackerName} ->{target.name} : {damage:F1} (HP{target.CurrentHP:F0}/{target.MaxHP:F0}", target);
        return true;
   }
}
