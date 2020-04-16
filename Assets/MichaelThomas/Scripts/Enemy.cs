using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Agent
{
    override protected Vector3 CombineWander()
    {
        Vector3 newVec = Vector3.zero;
        return new Vector3(0,0,0);
    }
}
