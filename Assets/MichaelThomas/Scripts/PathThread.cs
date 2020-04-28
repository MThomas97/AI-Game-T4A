using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathThread
{
    public Vector2Int startPos;
    public Vector2Int targetPos;

    public Node path;

    public volatile bool jobDone = false;

    public string output;

    //Sets the start pos and end pos and string
    public PathThread(Vector2Int posStart, Vector2Int posEnd, string stringOutput)
    {
        startPos = posStart;
        targetPos = posEnd;
        output = stringOutput;
    }
}
