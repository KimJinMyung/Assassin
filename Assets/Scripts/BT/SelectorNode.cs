using System.Collections.Generic;

public class SelectorNode : IBTNode
{
    List<IBTNode> _childNodeList;

    public SelectorNode(List<IBTNode> childNodeList)
    {
        _childNodeList = childNodeList;
    }

    public IBTNode.EBTNodeState Evaluate()
    {
        if (_childNodeList == null)
        {
            return IBTNode.EBTNodeState.Fail;
        }

        foreach (var child in _childNodeList)
        {
            var childState = child.Evaluate();
            switch (childState)
            {
                case IBTNode.EBTNodeState.Running:
                    return IBTNode.EBTNodeState.Running;
                case IBTNode.EBTNodeState.Success:
                    return IBTNode.EBTNodeState.Success;
            }
        }

        return IBTNode.EBTNodeState.Fail;
    }
}
