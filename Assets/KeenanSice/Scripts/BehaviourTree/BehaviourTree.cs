using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree
{
    protected Behaviour root;

    public void Tick()
    {
        root.Tick();
    }

    public void AddRoot(Behaviour newRoot)
    {
        root = newRoot;
    }

}
