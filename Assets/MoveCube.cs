using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("CustomCube")]
public class MoveCube : Action
{
    [SerializeField] SharedTransform cubeTrasnform;
    public float speed;
    public SharedFloat speed2;
    public override void OnStart()
    {
        cubeTrasnform.Value = gameObject.transform;
        cubeTrasnform.Value = Owner.transform;
    }

    public override TaskStatus OnUpdate()
    {
        cubeTrasnform.Value.Translate(Vector3.forward * speed);
        return TaskStatus.Running;

    }
}


[TaskCategory("CustomCube")]
public class MoveCube2 : Action
{
    [SerializeField] SharedTransform cubeTrasnform;
    [SerializeField] SharedCubeData cubeData;
    public float speed;
    public SharedFloat speed2;
    public override void OnStart()
    {
        cubeData.Value = GetComponent<CubeData>();
        cubeTrasnform.Value = gameObject.transform;
        cubeTrasnform.Value = Owner.transform;
    }

    public override TaskStatus OnUpdate()
    {
        cubeData.Value.abc = 20f;
        cubeTrasnform.Value.Translate(Vector3.forward * 0.1f);
        return TaskStatus.Success;

    }
}

[TaskCategory("CustomCube")]
public class MoveCube3 : Action
{
    [SerializeField] SharedTransform cubeTrasnform;
    public float speed;
    public SharedFloat speed2;
    public override void OnStart()
    {
        cubeTrasnform.Value = gameObject.transform;
        cubeTrasnform.Value = Owner.transform;
    }

    public override TaskStatus OnUpdate()
    {
        cubeTrasnform.Value.Translate(Vector3.forward * 5);
        return TaskStatus.Failure;

    }
}

[TaskCategory("CustomCube")]
public class CheckDistance : Conditional
{
    [SerializeField] SharedTransform cubeTrasnform;
    public SharedFloat distance;
    public override void OnStart()
    {
        cubeTrasnform.Value = gameObject.transform;
        cubeTrasnform.Value = Owner.transform;
    }

    public override TaskStatus OnUpdate()
    {
        if(cubeTrasnform.Value.position.z > distance.Value)
        {
            return TaskStatus.Failure;
        }
        return TaskStatus.Success;
        

    }
}