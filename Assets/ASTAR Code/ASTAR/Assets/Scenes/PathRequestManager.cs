using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

//Put on AStar
//Manages multiple Request so the game wont freeze when it calculates simultanious every path
public class PathRequestManager : MonoBehaviour {

    Queue<PathResult> results = new Queue<PathResult>();

    //Access stuff from the static method
    static PathRequestManager instance;
    Pathfinding pathfinding;


    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    private void Update()
    {
        //if t here is items in the queue
        if (results.Count > 0)
        {
            int itemsInqueue = results.Count;
            lock (results)
            {
                for (int i = 0; i < itemsInqueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    //if a unit requests a path it needs a pathrequest
    public static void RequestPath(PathRequest request)
    {
        //start a new thread
        ThreadStart threadStart = delegate
        {
            //method to call on the new thread
            instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
        };
        //start the thread
        threadStart.Invoke();
    }

    //when path is finished,         //when its called return the result to whoever requested the path so we need callback -> originalRequest
    public void FinishedProcessingPath(PathResult result)
    {
        //if it runs simultaniously and wont to add to the results = problem -> lock the result queue for only one thread
        lock (results) { 
        //add the result to the queue
        results.Enqueue(result);
        }
    }



}

//we need to go on the main thread before OnpathFound (callaback)
//add all relevant information (callback, path, successpool) to the queue, inside update method in the main thread we cann get that stuff out of the queue and get that callback
//store that information in a variable
public struct PathResult
{
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}

public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
    {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
    }
}