using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStateMachine : MonoBehaviour
{
    enum States
    {
        PursuingAmmo,
        PursuingHealth,
        Attacking,
        PursuingEnemy,
        Wandering
    };

    States currentState = States.Wandering;

    void TickState(Agent agent)
    {
        switch (currentState)
        {
            case States.PursuingHealth:
                PursuingAmmo(agent);
                break;
            case States.PursuingAmmo:
                PursuingHealth(agent);
                break;
            case States.Attacking:
                Attacking(agent);
                break;
            case States.PursuingEnemy:
                PursuingEnemy(agent);
                break;
            case States.Wandering:
                Wandering(agent);
                break;
        }
    }

    void PursuingAmmo(Agent agent)
    {
        //Goto Ammo
    }

    void PursuingHealth(Agent agent)
    {
        //Goto Health
    }

    void Attacking(Agent agent)
    {
        //
    }

    void PursuingEnemy(Agent agent)
    {

    }

    void Wandering(Agent agenet)
    {

    }

    void UpdateState(Agent agent)
    {
        if (!agent.HasAmmo())
        {
            currentState = States.PursuingAmmo;
        }
        else if(agent.GetHealth() < 20)
        {
            currentState = States.PursuingHealth;
        }
        else if(agent.GetAgentsInSight().Count > 0)
        {
            currentState = States.Attacking;
        }
    }
}
