using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class PathManager : MonoBehaviour {

    //shows in Inspector
    public UnityEvent UnitEvents;

    //delegate is a reference to a or more method
    public delegate void OnMessageReceived();
    public event OnMessageReceived onComplete;

    private void Start()
    {
        onComplete += WriteMessage;
        onComplete();
    }

    private void Update()
    {       
        //UnitEvents.Invoke();
    }

    void WriteMessage()
    {
        Debug.Log("Message Received");
    }

}
