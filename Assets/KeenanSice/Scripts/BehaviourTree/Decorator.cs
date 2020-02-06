using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decorator : Behaviour
{
    protected Behaviour child;

    public Decorator(Behaviour newChild) => child = newChild;
}
