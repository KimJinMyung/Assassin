using System.Collections.Generic;

public class SequenceNode : IBTNode
{
    List<IBTNode> _childNodeList;
    int _currentChild;

    public SequenceNode(List<IBTNode> childNodeList)
    {
        _childNodeList = childNodeList;
        _currentChild = 0;
    }

    public IBTNode.EBTNodeState Evaluate()
    {
        if (_childNodeList == null || _childNodeList.Count == 0)
            return IBTNode.EBTNodeState.Fail;

        for (; _currentChild < _childNodeList.Count; _currentChild++)
        {
            var childState = _childNodeList[_currentChild].Evaluate();
            switch (childState)
            {
                case IBTNode.EBTNodeState.Running:
                    return IBTNode.EBTNodeState.Running;
                case IBTNode.EBTNodeState.Success:
                    continue;
                case IBTNode.EBTNodeState.Fail:
                    _currentChild = 0;
                    return IBTNode.EBTNodeState.Fail;
            }
        }

        _currentChild = 0;
        return IBTNode.EBTNodeState.Success;
    }
}
