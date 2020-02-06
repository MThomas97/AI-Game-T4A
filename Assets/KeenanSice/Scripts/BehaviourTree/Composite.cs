using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Composite : Behaviour
{
    public void AddChild(Behaviour behaviour)
    {
        children.Insert(0, behaviour);
    }

    public void RemoveChild(Behaviour behaviour)
    {
        children.Remove(behaviour);
    }

    public void ClearChildren()
    {
        children.Clear();
    }

    protected List<Behaviour> children;
}
