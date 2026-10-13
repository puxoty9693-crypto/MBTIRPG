using UnityEngine;

public static class EntitlementService
{
    

    static IEntitlementService provider;

    public static IEntitlementService Provider
    {
        get => provider ??= new LocalEntitlementService();
        set => provider = value;
    }

    public const int MaxSlots = 3;
    public static int UnlockedSlotCount => Mathf.Clamp(Provider.GetUnlockedSlotCount(), 1, MaxSlots);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() 
    {
        provider = null;
    }
}
