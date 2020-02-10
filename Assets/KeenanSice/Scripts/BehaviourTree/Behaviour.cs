﻿//#define KSDEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour
{
    public virtual void Init()
    {
#if KSDEBUG
        Debug.Log("Behaviour Initialised");
#endif
    }

    public virtual void Terminate(Status newStatus)
    {
#if KSDEBUG
        Debug.Log("Behaviour Terminated");
#endif

        status = newStatus;
    }

    public Status Tick()
    {
        if (status != Status.Running)
        {
            Init();
        }

        status = Update();

        if (status != Status.Running)
        {
            Terminate(status);
        }

        return status;
    }

    public virtual Status Update()
    {
        return Status.Invalid;
    }


    public Status GetStatus()
    {
        return status;
    }

    private Status status = Status.Invalid;

    public bool IsTerminated()
    {
        return (status == Status.Failure || status == Status.Success);
    }

    public bool IsRunning()
    {
        return (status == Status.Running);
    }

    public void Abort()
    {
        Terminate(Status.Aborted);
        status = Status.Aborted;
    }


}

public enum Status
{
    Invalid,
    Running,
    Failure,
    Success,
    Aborted,
}