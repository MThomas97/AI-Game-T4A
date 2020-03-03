using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

//This class controls the thread
public class ThreadQueuer : MonoBehaviour
{
    //Singleton
    private static ThreadQueuer instance;

    private void Awake()
    {
        instance = this;
    }

    public static ThreadQueuer GetInstance()
    {
        return instance;
    }

    //The maximum number of threads open simultaneous
    public const int MaxThreads = 3;

    private List<PathThread> currentJobs;
    private List<PathThread> todoJobs;

    private void Start()
    {
        currentJobs = new List<PathThread>();
        todoJobs = new List<PathThread>();
    }

    private void Update()
    {
        int i = 0;

        while(i < currentJobs.Count)
        {
            if (currentJobs[i].jobDone)
                currentJobs.RemoveAt(i);
            else
                i++;
        }

        if(todoJobs.Count > 0 && currentJobs.Count < MaxThreads)
        {
            PathThread job = todoJobs[0];
            todoJobs.RemoveAt(0);
            currentJobs.Add(job);

            //Start a new thread

            ThreadStart newThread = delegate
            {
                job.path = PathFinding.CalculatePath(job.startPos, job.targetPos, out string output);
                job.jobDone = true;
            };

            Thread jobThread = new Thread(newThread);
            jobThread.Start();

        }
    }



    public void RequestPathFind(Vector2Int startPos, Vector2Int targetPos, string output)
    {
        PathThread newJob = new PathThread(startPos, targetPos, output);
        todoJobs.Add(newJob);
    }
}
