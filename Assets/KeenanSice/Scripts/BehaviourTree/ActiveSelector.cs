using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSelector : Selector
{
    public ActiveSelector(params Behaviour[] newChildren) : base(newChildren) { }

    public void AddCondition(Behaviour condition)
    {
        children.Insert(0, condition);
    }

    public void AddAction(Behaviour action)
    {
        children.Add(action);
    }

    public override Status Update(ref string outDebug, int branchDepth)
    {
        List<Behaviour>.Enumerator prevChild = currentChild;

        base.Init();

        Status result = base.Update(ref outDebug, branchDepth);

        if(currentChild.Current != children[children.Count-1] && currentChild.Current != prevChild.Current)
        {
            prevChild.Current.Abort();
        }

        return result;
    }
}
