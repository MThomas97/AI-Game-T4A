﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BehaviourTreeHelperFunctions;

public class AgentBehaviour : MonoBehaviour
{
    BehaviourTree bt;

    //KS - Setup behaviour tree.
    void SetupBehaviour()
    {
        bt = behaviourTree(
            sequence(
                action(ac.CheckResetTargetTimer),
                selector(
                    condition(ac.HasAmmo,
                        selector(
                        action(ac.SetTargetToEnemyInSight),
                        action(ac.SetTargetToLastEnemySeen)
                        ),
                        action(ac.SetTargetToClosestActiveAmmo)
                    ),
                    action(ac.SetTargetToClosestActiveHealth),
                    action(ac.StartPatrol)
                ),
                condition(ac.HasTarget,
                    //KS - HasTarget = true
                    condition(ac.IsFacingTarget,
                        //KS - IsFacingTarget = true
                        condition(ac.IsTargetInRange,
                            //KS - IsTargetInRange = true
                            selector(
                                action(ac.AttackAgent),
                                action(ac.PickupCollectable),
                                action(ac.PatrolToNextTarget)
                            ),
                            //KS - IsTargetInRange = false
                            action(ac.MoveForward)
                        ),
                        //KS - IsFacingTarget = false
                        action(ac.RotateTowards)
                    ),
                    //KS - HasTarget = false
                    action() //KS - Do Nothing
                )
            )
        );
    }

    string btDebugOutput = "BT";
    AgentController ac;

    void Start()
    {
        ac = GetComponent<AgentController>();
        SetupBehaviour();
    }

    void Update()
    {
        btDebugOutput = "";
        bt.Tick(ref btDebugOutput, 0);
    }

    //KS - On screen debugging. For visualising the behaviour tree.
    void OnGUI()
    {
        const float widthPadding = 10;
        float labelHeight = (Screen.height / (float)World.agentTeams.Count);
        float heightOffset = labelHeight * ac.teamNumber;
        GUI.contentColor = World.playerColours[ac.teamNumber];

        GUI.skin.label.fontSize = Screen.width / 100;
        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        GUI.Label(new Rect(widthPadding, heightOffset, Screen.width * 0.5f, labelHeight), btDebugOutput);
    }
}
