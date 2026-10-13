using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Health))]
public class HitFlash : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color flashColor = Color.white;
    [SerializeField] float flashTime = 0.1f;
    [SerializeField] bool blinkWhileInvincible = true;
    [SerializeField] float blinkInterval = 0.08f;

    Health health;
    Color originalColor;
    Coroutine flashRoutine;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (spriteRenderer == null ) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    private void OnEnable() => health.OnDamaged += HandleDamaged;
    private void OnDisable()
    {
        health.OnDamaged -= HandleDamaged;
        if (spriteRenderer != null) { spriteRenderer.color = originalColor; spriteRenderer.enabled = true; }
    }

    void Update()
    {
        if (spriteRenderer == null || !blinkWhileInvincible) return;
        if (health.IsHitInvincible && !health.IsDead)
            spriteRenderer.enabled = Mathf.FloorToInt(Time.time / blinkInterval) % 2 == 0;
        else if (!spriteRenderer.enabled)
            spriteRenderer.enabled = true;
    }

    void HandleDamaged(float amount, DamageInfo info) 
    {
        if (spriteRenderer == null || !isActiveAndEnabled) return;
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(Flash());
    }

    IEnumerator Flash() 
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashTime);
        spriteRenderer.color = originalColor;
        flashRoutine = null;
    }

}
