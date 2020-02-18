using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnScreenDebug : MonoBehaviour
{
    public static Text[] pathfindingDebugs = new Text[] { };

    public Text[] pathfindingDebugsNonStatic;

    // Start is called before the first frame update
    void Start()
    {
        pathfindingDebugs = pathfindingDebugsNonStatic;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
