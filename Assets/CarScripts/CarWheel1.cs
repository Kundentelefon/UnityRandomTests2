using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Put on WheelMesh
public class CarWheel : MonoBehaviour {

    public WheelCollider targetWheel;

    private Vector3 wheelPosition = new Vector3();
    private Quaternion wheelRotation = new Quaternion();

	// Update is called once per frame
	void Update () {
        //Get Rotation and Position of the Collider and apply to Mesh
        targetWheel.GetWorldPose(out wheelPosition, out wheelRotation);
        transform.position = wheelPosition;
        transform.rotation = wheelRotation;
	}
}
