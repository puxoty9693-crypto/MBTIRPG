using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hurtbox : MonoBehaviour
{
    [SerializeField] Health owner; //비워두면 부모에서 자동으로 찾음

    public Health Owner => owner;

    private void Awake()
    {
        if (owner == null) owner = GetComponentInParent<Health>();
        if (owner == null) GameLog.Error("Combat", "Hurtbox에 연결된 Health가 없습니다.", this);
    }
}
