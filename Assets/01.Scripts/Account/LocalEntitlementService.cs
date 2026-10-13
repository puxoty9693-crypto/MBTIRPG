using UnityEngine;

public class LocalEntitlementService : IEntitlementService
{
    public const int BaseSlot = 1;

    public int DebugExtraSlots;

    public int GetUnlockedSlotCount() => BaseSlot + Mathf.Max(0, DebugExtraSlots);
}
