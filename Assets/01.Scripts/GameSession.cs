using UnityEngine;

public static class GameSession
{
    public static int SelectedSlot = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RestiState() 
    {
        SelectedSlot = 0;
    }
}
