using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Put on AStar
//Manages multiple Request so the game wont freeze when it calculates simultanious every path
public class PathRequestManager : MonoBehaviour {

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    //Access stuff from the static method
    static PathRequestManager instance;
    Pathfinding pathfinding;
    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    //Action will store methods until its called
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        //After the creation of a nuew PathRequest
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        //Adds request to the que
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    //if we are currently already process a path, if not ask the Pathfinding script to process the next one
    void TryProcessNext () {
        //if we are not processing a path and queue is not empty
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            //Dequeue takes the first item out of queue
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    //when path is finished
    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
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
}
