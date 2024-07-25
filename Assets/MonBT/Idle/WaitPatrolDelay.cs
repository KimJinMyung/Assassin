using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitPatrolDelay : MonoBehaviour
{
    [SerializeField] BehaviorTree tree;

    // Start is called before the first frame update
    void OnEnable()
    {
        tree.SetVariableValue("PatrolDelayTime", Random.Range(1.5f, 3f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
