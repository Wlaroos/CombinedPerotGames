using UnityEngine;

public class RefreshButton : MonoBehaviour
{
    public MineralManager mineralManager;

    private void OnMouseDown()
    {
        if (mineralManager != null)
            mineralManager.RespawnMinerals();
    }
}
