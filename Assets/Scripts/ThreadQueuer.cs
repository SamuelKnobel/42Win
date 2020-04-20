//-----------------------------------------------------------------------
// <copyright file="ThreadQueuer.cs" company="Quill18 Productions">
//     Copyright (c) Quill18 Productions. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

public class ThreadQueuer : MonoBehaviour 
{


    public bool ThredsRunning
    {
        get
        {
            if (runningThreds.Count > 0)
            {
                return true;
            }
            else
                return false;
        }
    }
    public bool thredsRunning;


    static List<Action> functionsToRunInMainThread;
    static List<Thread> runningThreds;

    private void Awake()
    {
        runningThreds = new List<Thread>();
    }
    void Start()
    {
        functionsToRunInMainThread = new List<Action>();

    }
   public  float breackTime = 0;
    void Update()
    {
        breackTime = runningThreds.Count;
        thredsRunning = ThredsRunning;
        //// Update() always runs in the main thread

        //while (functionsToRunInMainThread.Count > 0)
        //{
        //    // Grab the first/oldest function in the list
        //    Action someFunc = functionsToRunInMainThread[0];
        //    functionsToRunInMainThread.RemoveAt(0);
        //    // Now run it
        //    someFunc();
        //}
        if (runningThreds.Count > 0|| breackTime > 60)
        {
            if (!runningThreds[0].IsAlive)
            {
                runningThreds.RemoveAt(0);
            }
        }
    }

    public static void StartThreadedFunction( Action someFunction ) // Action is a short form of delegate that takes no parameters
    {
        
        Thread t = new Thread( new ThreadStart( someFunction ) );
        runningThreds.Add(t);
        t.Start();


    }

    public static void QueueMainThreadFunction( Action someFunction )
    {
        // We need to make sure that someFunction is running from the main thread
        functionsToRunInMainThread.Add(someFunction);
    }


}
