using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class PlayerView : MonoBehaviour
{
    private PlayerData platerData;
    private PlayerViewModel vm;
    private void OnEnable()
    {
        if(vm == null)
        {
            vm = new PlayerViewModel();
            vm.PropertyChanged += OnPropertyChanged;
            vm.RegisterPlayerHPChanged(true);
            vm.RegisterPlayerStaminaChanged(true);
        }

        SetPlayerInfo();

        Debug.Log(platerData.HP);
        Debug.Log(platerData.Stamina);
        Debug.Log("=====================");
        Debug.Log(vm.HP);
        Debug.Log(vm.Stamina);
    }

    private void OnDisable()
    {
        if(vm != null )
        {
            vm.RegisterPlayerStaminaChanged(false);
            vm.RegisterPlayerHPChanged(false);
            vm.PropertyChanged -= OnPropertyChanged;
            vm = null;
        }
    }

    private void SetPlayerInfo()
    {
        var player = DataManager.Instance.GetPlayerData(0);
        if (player == null) return;

        platerData = player.Clone();
        vm.RequestPlayerHPChanged(player.Clone().HP);
        vm.RequestPlayerStaminaChanged(player.Clone().Stamina);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {

    }
}
