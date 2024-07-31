using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptsManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> GameObject;

    public List<GameObject> GameObjectList { get { return GameObject; } }

    private void Start()
    {
        foreach (GameObject go in GameObject)
        {
            Instantiate(go);
        }
    }
}
