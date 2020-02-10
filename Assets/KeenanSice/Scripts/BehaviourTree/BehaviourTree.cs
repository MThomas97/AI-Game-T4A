using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree
{
    protected Behaviour root;

    public BehaviourTree(Behaviour newRoot) => root = newRoot;

    public void Tick()
    {
        root.Tick();
    }

    public void SetRoot(Behaviour newRoot)
    {
        root = newRoot;
    }

}
