using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedCubeData : SharedVariable<CubeData>
    {
        public static implicit operator SharedCubeData(CubeData value) { return new SharedCubeData { mValue = value }; }
    }
}