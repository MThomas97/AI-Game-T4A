using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSelector : Selector
{
    public void AddCondition(Behaviour condition)
    {
        children.Insert(0, condition);
    }

    public void AddAction(Behaviour action)
    {
        children.Add(action);
    }

    protected override Status Update()
    {
        List<Behaviour>.Enumerator prevChild = currentChild;

        //KS - Don't really like this, need to come back and rethink
        List<Behaviour>.Enumerator nextChild = currentChild;
        bool isCurrentLast = nextChild.MoveNext();

        base.Init();

        Status result = base.Update();

        if(!isCurrentLast && currentChild.Current != prevChild.Current)
        {
            prevChild.Current.Abort();
        }

        return result;
    }
}
