using System;

public class ActionNode : IBTNode
{
    Func<IBTNode.EBTNodeState> _onUpdate = null;

    public ActionNode(Func<IBTNode.EBTNodeState> onUpdate)
    {
        _onUpdate = onUpdate;
    }

    public IBTNode.EBTNodeState Evaluate()
    {
        if (_onUpdate == null)
        {
            return IBTNode.EBTNodeState.Fail;
        }
        else
        {
            return _onUpdate.Invoke();
        }
    }
}
