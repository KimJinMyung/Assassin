using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MonsterUI : MonoBehaviour
{
    [SerializeField] GameObject Prefab_MonsterHud;

    [Header("BossMonster")]
    [SerializeField] Image BossMonster_HPBar;
    [SerializeField] StaminaBar BossMonster_Stamina;
    [SerializeField] Text MonsterName;
    [SerializeField] Transform LifeCountIcon;

    List<Slot_MonsterHud> _monsterHUDSlotList = new List<Slot_MonsterHud>();

    private Canvas _thisCanvase;

    Image HP_BackGround;
    Image Stamina_Background, Stamina_Left, Stamina_Right;

    private void Awake()
    {
        _thisCanvase = GetComponentInParent<Canvas>();
        HP_BackGround = BossMonster_HPBar.transform.parent.GetComponent<Image>();
        Stamina_Background = BossMonster_Stamina.GetComponent<Image>();
        Stamina_Left = BossMonster_Stamina.StaminaBarLeft;
        Stamina_Right = BossMonster_Stamina.StaminaBarRight;
        MonsterManager.Instance.SetHUD(this);
    }

    private void OnEnable()
    {
        ViewBossHud(false);
    }

    private void OnDisable()
    {
        _monsterHUDSlotList.ForEach(e => DestroyImmediate(e.gameObject));
        _monsterHUDSlotList.Clear();
    }    

    public void CreateMonsterHUD(MonsterView monster)
    {
        var gObj = Instantiate(Prefab_MonsterHud, this.transform);
        var hud = gObj.GetComponent<Slot_MonsterHud>();
        if (hud == null)
            return;

        hud.BindMonster(monster, _thisCanvase);
        _monsterHUDSlotList.Add(hud);
    }

    MonsterView BossMonster;
    MonsterViewModel BossMonsterViewModel;

    private bool isViewBossMonsterHud;

    public void BindBossMonster(MonsterView monster)
    {
        BossMonster = monster;
        BossMonsterViewModel = BossMonster.vm;

        BossMonster_Stamina.SetMaxStamina(BossMonster._monsterData.MaxStamina);
        BossMonster_Stamina.SetCurrentStamina(BossMonsterViewModel.Stamina);        

        MonsterName.text = BossMonster._monsterData.Name;
    }

    public void OffMonsterHUD(MonsterView monster)
    {
        foreach (var slot in _monsterHUDSlotList)
        {
            if (slot._monster.monsterId == monster.monsterId)
            {
                slot.OnOffHud(false);
                slot.gameObject.SetActive(false);
            }
        }
    }

    public void BossMonsterHud_OnOff(bool onOff)
    {
        isViewBossMonsterHud = onOff;
    }

    private void Update()
    {
        if (BossMonster == null) return;

        ViewBossHud(isViewBossMonsterHud);

        BossMonster_HPBar.fillAmount = (BossMonsterViewModel.HP / BossMonster._monsterData.MaxHP);
        BossMonster_Stamina.SetCurrentStamina(BossMonsterViewModel.Stamina);
    }

    private void ViewBossHud(bool onOff)
    {
        Stamina_Background.enabled = onOff;
        Stamina_Left.enabled = onOff;
        Stamina_Right.enabled = onOff;
        BossMonster_Stamina.enabled = onOff;
        MonsterName.enabled = onOff;
        HP_BackGround.enabled = onOff;
        BossMonster_HPBar.enabled = onOff;

        if (!onOff) UpdateLifeCount(0);
        else UpdateLifeCount((int)BossMonsterViewModel.LifeCount);
    }

    private void UpdateLifeCount(int LifeCount)
    {
        int index = 0;
        foreach (Transform child in LifeCountIcon)
        {
            Image LifeCountIcon = child.GetComponent<Image>();

            if (LifeCountIcon == null) continue;

            if (index < LifeCount) LifeCountIcon.enabled = true;
            else LifeCountIcon.enabled = false;

            index++;
        }
    }
}
