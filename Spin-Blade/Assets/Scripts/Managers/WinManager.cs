using UnityEngine;

public static class WinManager
{
    public const int WIN_FRAGMENTS_NEEDED = 3;
    public static int winFragements { get; private set; }

    public static void AddWinFragment(int amount)
    {
        winFragements += amount;
    }
}
