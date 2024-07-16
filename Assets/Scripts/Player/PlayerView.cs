using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class PlayerView : MonoBehaviour
{
    private PlayerData platerData;

    private void OnEnable()
    {
        SetPlayerInfo();
    }

    private void SetPlayerInfo()
    {
        var player = DataManager.Instance.GetPlayerData(0);
        if (player == null) return;

        platerData = player.Clone();
    }
}
