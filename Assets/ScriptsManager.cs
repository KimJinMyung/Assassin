using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptsManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> GameObject;

    private void Start()
    {
        foreach (GameObject go in GameObject)
        {
            Instantiate(go);
        }
    }
}
