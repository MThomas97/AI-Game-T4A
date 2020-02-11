using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTestAgent : MonoBehaviour
{
    BehaviourTree bt;

    void SetupBehaviour()
    {
        bt = new BehaviourTree(
            new Sequence(
                new Condition(CanMoveForward, 
                    new KSAction(MoveForward),
                    new KSAction(RotateLeft)
                )
            )
        );
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupBehaviour();
    }

    // Update is called once per frame
    void Update()
    {
        bt.Tick();
    }

    bool CanMoveForward()
    {
        Vector3 newPosition = transform.position + transform.right * Time.deltaTime;
        return (newPosition.x >= 0 && newPosition.y >= 0 && newPosition.x < 9 && newPosition.y < 9);
    }

    void RotateLeft()
    {
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * 45));
    }

    bool IsEnemyInSight()
    {
        //Loop through enemies
        //Store all that are in set view angle

        //Loop through enemies in view angle
        //Check if nothing is between our line of sight

        //Return true if we can see atleast 1 enemy, we should probs cache too.

        return false;
    }


    void MoveForward()
    {
        return;
        transform.localPosition += transform.right * Time.deltaTime;
    }
}
