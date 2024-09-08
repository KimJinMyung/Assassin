using EventEnum;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LockOn_UI : MonoBehaviour
{
    [SerializeField] private GameObject _lockOnIconPrefab;

    private Canvas _thisCanvas;
    private List<Transform> LockOnTargetList = new List<Transform>();

    private Dictionary<Transform, Image> monsterIcons = new Dictionary<Transform, Image>();

    private void Awake()
    {
        _thisCanvas = GetComponent<Canvas>();

        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        EventManager<MonsterEvent>.Binding<List<Transform>>(true, MonsterEvent.ChangedLockOnIconEnableList, ChangedLockOnIconEnableList);
    }

    private void RemoveEvent()
    {
        EventManager<MonsterEvent>.Binding<List<Transform>>(false, MonsterEvent.ChangedLockOnIconEnableList, ChangedLockOnIconEnableList);
    }

    private void ChangedLockOnIconEnableList(List<Transform> LockOnAbleMonsterList)
    {
        LockOnTargetList.Clear();
        LockOnTargetList = new List<Transform>(LockOnAbleMonsterList);
    }

    private void Update()
    {
        foreach (var monster in LockOnTargetList)
        {
            if (monster == null) continue;

            if (!monsterIcons.ContainsKey(monster))
            {
                GameObject newIcon;
                if (!ActivateInactiveChild(transform, out newIcon))
                {
                    newIcon = Instantiate(_lockOnIconPrefab, transform);
                }
                newIcon.gameObject.SetActive(true);
                Image _lockOnIcon = newIcon.GetComponent<Image>();
                monsterIcons.Add(monster, _lockOnIcon);
            }

            MonsterLockOnUIPrint(monster, monsterIcons[monster]);           
        }

        if (monsterIcons.Count <= 0) return;

        List<Transform> removeList = new List<Transform>();
        foreach (var monster in monsterIcons)
        {
            if (!LockOnTargetList.Contains(monster.Key))
            {
                removeList.Add(monster.Key);
                monster.Value.enabled = false;
            }
        }

        foreach (var monster in removeList)
        {
            if (monster.gameObject.layer == LayerMask.NameToLayer("LockOnAble")) return;
            monsterIcons[monster].gameObject.SetActive(false);
            monsterIcons.Remove(monster);
        }
    }

    bool ActivateInactiveChild(Transform parent, out GameObject newIcon)
    {
        foreach (Transform child in parent)
        {
            if (!child.gameObject.activeSelf)
            {
                newIcon = child.gameObject;
                return true;
            }
        }
        newIcon = null;
        return false;
    }

    private void MonsterLockOnUIPrint(Transform _target, Image icon)
    {
        Vector3 ScreenPosition;

        if (_target.CompareTag("RopePoint")) ScreenPosition = Camera.main.WorldToScreenPoint(_target.position);
        else ScreenPosition = Camera.main.WorldToScreenPoint(_target.position + Vector3.up);

        if (ScreenPosition.z > 0)
        {
            Vector2 canvasPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _thisCanvas.transform as RectTransform,
                ScreenPosition,
                _thisCanvas.worldCamera,
                out canvasPosition);

            icon.rectTransform.anchoredPosition = canvasPosition;
            icon.enabled = true;

            IconColorChanged(_target.gameObject, icon);
        }
        else
        {
            icon.enabled = false;
        }                
    }

    private void IconColorChanged(GameObject target, Image _lockOnIcon)
    {
        if (target.layer == LayerMask.NameToLayer("LockOnTarget"))
        {
            _lockOnIcon.color = Color.green;
        }
        else if (target.layer == LayerMask.NameToLayer("LockOnAble"))
        {
            _lockOnIcon.color = Color.white;
        }
        else if (target.layer == LayerMask.NameToLayer("Incapacitated"))
        {
            _lockOnIcon.color = Color.red;
        }
        else if (target.layer == LayerMask.NameToLayer("RopePoint"))
        {
            _lockOnIcon.color = Color.magenta;
        }
        else _lockOnIcon.enabled = false;
    }
}
