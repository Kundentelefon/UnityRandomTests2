using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//put on Car
public class CarEngine : MonoBehaviour {

    
    public Transform path;
    public float maxSteerangle = 45f;
    public float turnSpeed = 2f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;
    public float maxMotorTorque = 80f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public Vector3 centerOfMass;

    //Change Texture for example Braking
    public bool isBreaking = false;
    public Texture2D textureNormal;
    public Texture2D textureBraking;
    public Renderer carRenderer; //The Renderer has the Texture

    public float maxBreakTorque = 150f;

    [Header("Sensors")]
    public float sensorLength = 3f;
    public Vector3 frontSensorPosition = new Vector3(0,0.2f,0.5f);
    public float frontsideSensorPosition = 0.2f;
    public float frontSensorAngle = 30;

    private List<Transform> nodes;
    private int currentNode = 0;
    private bool avoiding = false;
    private float targetSteerAngle = 0;

    //copy all Nodes from the existing Path
    void Start()
    {
        //Sets own center of mass to rigidbody
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        Transform[] pathTransform = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        //holt sich alle Transforms die nicht in der Liste sind
        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != path.transform)
            {
                nodes.Add(pathTransform[i]);
            }
        }
    }

    private void FixedUpdate()
    {
        //First Check everything and then decide what to do
        Sensors();
        ApplySteer();
        Drive();
        CheckWayPointDistance();
        LerpToSteerAngle();

        Braking();
    }

    //5 Sensor Raycast on front of the Car 
    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        //changesPosition when Car Rotates always on front of Car
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        //Wenn mehrere Hindernis getroffen werden
        float avoidMultiplier = 0;
        avoiding = false;

 
        
        //Draws Line to the next Object it is going to hit, front right sensor
        sensorStartPos += transform.right * frontsideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            //Schliesst Terrain wie zb Hügel aus
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                //negativ erhöht die Lenkung nach links um 0.5
                avoidMultiplier -= 1f;
            }
        }
             
        //Draws Line to the next Object it is going to hit, front right angle sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            //Schliesst Terrain wie zb Hügel aus
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 0.5f;
            }
        }

        //Draws Line to the next Object it is going to hit, front left sensor
        sensorStartPos -= transform.right * frontsideSensorPosition * 2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            //Schliesst Terrain wie zb Hügel aus
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += 1f;
            }
        }        

        //Draws Line to the next Object it is going to hit, front left angle sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            //Schliesst Terrain wie zb Hügel aus
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 0.5f;
            }
        }

        //Draws Line to the next Object it is going to hit, front center
        if(avoidMultiplier == 0) { 
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            //Schliesst Terrain wie zb Hügel aus
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                    if(hit.normal.x < 0)
                    {
                        avoidMultiplier = -1;
                    }
                    else
                    {
                        avoidMultiplier = 1;
                    }
            }
        }
        }

        //When avoiding
        if (avoiding)
        {
            targetSteerAngle = maxSteerangle * avoidMultiplier;
        }
    }

    //points the Wheelcollider to the waypoint
    private void ApplySteer()
    {
        if (avoiding) return;
        //Gets the difference from Car and next waypoint as Length
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) *maxSteerangle;
        targetSteerAngle = newSteer;


    }

    //AddEngineToWheel
    private void Drive()
    {
        //Berechnet derzeitige Geschwindigkeit aus der Drehzahl des Reifens
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

        //power of the car
        if (currentSpeed < maxSpeed && !isBreaking) { 
        wheelFL.motorTorque = maxMotorTorque;
        wheelFR.motorTorque = maxMotorTorque;
        }else
        {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }

    }

    //Wenn nahe am Waypoint springe zum nächsten
    private void CheckWayPointDistance()
    {
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < 0.5f)
        {
            //Next node, if its the last one take the first from the list
            if(currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
        }
    }

    private void Braking()
    {
        if (isBreaking)
        {
            carRenderer.material.mainTexture = textureBraking;
            wheelRL.brakeTorque = maxBreakTorque;
            wheelRR.brakeTorque = maxBreakTorque;
        }
        else
        {
            carRenderer.material.mainTexture = textureNormal;
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }
    }

    //smoothen Steering Angle
    private void LerpToSteerAngle()
    {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }
}
