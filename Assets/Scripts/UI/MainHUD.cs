using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class MainHUD : MonoBehaviour
{
    [SerializeField] private Image HP_Bar;
    [SerializeField] private StaminaBar StaminaBar;
    [SerializeField] private Transform playerLife;

    PlayerUI_ViewModel ui_Viewmodel;

    private void OnEnable()
    {
        if (ui_Viewmodel == null)
        {
            ui_Viewmodel = new PlayerUI_ViewModel();
            ui_Viewmodel.PropertyChanged += OnPropertyChanged;
            ui_Viewmodel.RegisterPlayerHPChanged(true);
            ui_Viewmodel.RegisterPlayerMaxHPChanged(true);
            ui_Viewmodel.RegisterPlayerStaminaChanged(true);
            ui_Viewmodel.RegisterPlayerMaxStaminaChanged(true);
            ui_Viewmodel.RegisterPlayerLifeCountChanged(true);
        }
    }

    private void OnDisable()
    {
        if (ui_Viewmodel != null)
        {
            ui_Viewmodel.RegisterPlayerHPChanged(false);
            ui_Viewmodel.RegisterPlayerMaxHPChanged(false);
            ui_Viewmodel.RegisterPlayerStaminaChanged(false);
            ui_Viewmodel.RegisterPlayerMaxStaminaChanged(false);
            ui_Viewmodel.RegisterPlayerLifeCountChanged(false);
            ui_Viewmodel.PropertyChanged -= OnPropertyChanged;
            ui_Viewmodel = null;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ui_Viewmodel.HP):
                HP_Bar.fillAmount = (float)ui_Viewmodel.HP / ui_Viewmodel.MaxHP;
                break;
            case nameof(ui_Viewmodel.MaxHP):
                HP_Bar.rectTransform.sizeDelta += new Vector2(ui_Viewmodel.MaxHP - 100, 0);
                break;
            case nameof(ui_Viewmodel.MaxStamina):
                StaminaBar.SetMaxStamina(ui_Viewmodel.MaxStamina);
                break;
            case nameof(ui_Viewmodel.Stamina):
                StaminaBar.SetCurrentStamina(ui_Viewmodel.Stamina);
                break;
            case nameof(ui_Viewmodel.LifeCount):
                int index = 1;
                foreach (Transform child in playerLife)
                {
                    if (index > ui_Viewmodel.LifeCount)
                    {
                        child.gameObject.SetActive(false);
                        continue;
                    }

                    child.gameObject.SetActive(true);
                    index++;
                }
                break;
        }
    }
}
