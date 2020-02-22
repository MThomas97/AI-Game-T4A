﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Filter : Sequence
{
    public Filter(params Behaviour[] newChildren) : base(newChildren) { }

    public void AddCondition(Behaviour condition)
    {
        children.Insert(0, condition);
    }

    public void AddAction(Behaviour action)
    {
        children.Add(action);
    }

}
