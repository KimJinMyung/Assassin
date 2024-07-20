using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalNode : IBTNode
{
    private Func<bool> _condition;
    private IBTNode _childNode;

    public ConditionalNode(Func<bool> condition, IBTNode childNode)
    {
        this._condition = condition;
        this._childNode = childNode;
    }

    public IBTNode.EBTNodeState Evaluate()
    {
        if(_childNode == null) return IBTNode.EBTNodeState.Fail;

        if (_condition())
        {
            return _childNode.Evaluate();
        }

        return IBTNode.EBTNodeState.Fail;
    }
}
