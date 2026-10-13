using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(Collider2D))]
public class ContactDamage : MonoBehaviour
{

    [SerializeField] float attackPower = 8f;
    [SerializeField] float knockback = 7f;
    [SerializeField] LayerMask targetMask; // Player
    
    readonly List<Collider2D> results = new List<Collider2D>();
    ContactFilter2D filter;
    Collider2D col;
    Health self;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        self = GetComponentInParent<Health>();
        filter = new ContactFilter2D();
        filter.useTriggers = true;
    }

    private void FixedUpdate()
    {
        if (self != null && self.IsDead) return;

        filter.SetLayerMask(targetMask);
        Bounds b = col.bounds;
        results.Clear();
        Physics2D.OverlapBox(b.center, b.size, 0f, filter, results);

        foreach (Collider2D c in results) 
        {
            if (!c.TryGetComponent(out Hurtbox hurtbox)) continue;
            Health target = hurtbox.Owner;
            if (target == null || target == self) continue;

            float dir = Mathf.Sign(hurtbox.transform.position.x - transform.position.x);
            var info = new DamageInfo(gameObject, attackPower, 1f, knockback);
            info.hitDirection = new Vector2(dir, 0f);
            CombatSystem.ApplyHit(info, target);
        }
    }
}
