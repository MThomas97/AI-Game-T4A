using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decision : MonoBehaviour
{
    public virtual Action MakeDecision(Agent agentMakingDecision)
    {
        Debug.Log("This is the decision node base class.");
        return null;
    }

    public Decision trueNode, falseNode;
    public Action action;
}
