using UnityEngine;

public static class UserIdService
{
    static IUserIdProvider provider;
    static string cacheId;

    public static IUserIdProvider Provider 
    {
        get => provider ??= new LocalGuidUserIdProvider();
        set { provider = value; cacheId = null; }
    }

    public static string UserId => cacheId ??= Provider.GetOrCreateUserId();
    
    public static void ClearCache() => cacheId = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() 
    {
        provider = null;
        cacheId = null;
    }
}
