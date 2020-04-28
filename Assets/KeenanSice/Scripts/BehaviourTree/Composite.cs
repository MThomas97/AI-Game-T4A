using System.Collections.Generic;

public class Composite : Behaviour
{
    protected List<Behaviour> children = new List<Behaviour>();

    public Composite(params Behaviour[] newChildren)
    {
        for (int i = 0; i < newChildren.Length; i++)
        {
            AddChild(newChildren[i]);
        }
    }

    public void AddChild(Behaviour behaviour)
    {
        children.Add(behaviour);
    }

    public void RemoveChild(Behaviour behaviour)
    {
        children.Remove(behaviour);
    }

    public void ClearChildren()
    {
        children.Clear();
    }
}
