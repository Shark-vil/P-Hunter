using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLook : MonoBehaviour {

	public Transform headBone;
	public Transform target;
	public float lookSpeed = 10.0f;
	public float limitAngle = 60.0f;

	internal GameObject dumA; //dumA for following orientation of bone
	internal GameObject dumB; //dumB for point to target
	internal GameObject dumC; //dumC for following target
	internal bool activateTarget;

	// Use this for initialization
	void Start () {
		//dumA for following the orientation of bone
		dumA = new GameObject("dummyA for" + headBone.transform.name);
		// dumB for point to target
		dumB = new GameObject("dummyB for" + headBone.transform.name);
		//dumC for following target
		dumC = new GameObject("dummyC for" + headBone.transform.name);
		dumA.transform.position = headBone.transform.position;
		dumB.transform.position = headBone.transform.position;
		dumB.transform.localRotation = headBone.transform.root.localRotation;

		dumB.transform.parent = headBone.transform.root;
		dumC.transform.parent = headBone.transform.root;

		dumA.transform.position = headBone.transform.position;
		dumA.transform.parent = dumB.transform;
		dumA.transform.rotation = headBone.transform.rotation;

		activateTarget = true;
	}
	
	// LateUpdate is called at the end of every frame
	void LateUpdate () {
		if(target != null){
			dumB.transform.position = headBone.transform.position;
			dumC.transform.position = target.position;

			if(activateTarget){
				headBone.rotation = dumA.transform.rotation;

				var targetDirection = dumC.transform.localPosition - dumB.transform.localPosition;
				dumB.transform.localRotation = Quaternion.Slerp (dumB.transform.localRotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * lookSpeed);
				//Clamp limit rotation
				dumB.transform.localEulerAngles = new Vector3(ClampAngle(dumB.transform.localEulerAngles.x, -limitAngle, limitAngle), 
					ClampAngle(dumB.transform.localEulerAngles.y, -limitAngle, limitAngle), 
					ClampAngle(dumB.transform.localEulerAngles.z, -limitAngle, limitAngle));
			}
		}else
			if(target == null){
				dumB.transform.localRotation = Quaternion.Slerp (dumB.transform.localRotation, Quaternion.identity, Time.deltaTime);  
			}
	}

	// Xeno: Thanks aldonaletto for solving rotation producing negative integers
	float ClampAngle(float angle, float min, float max) {
		if (angle  < 90f || angle > 270f){       // if angle in the critic region...
			if (angle > 180f) angle -= 360f;  // convert all angles to -180..+180
			if (max > 180f) max -= 360f;
			if (min > 180f) min -= 360f;
		}    
		angle = Mathf.Clamp(angle, min, max);
		if (angle < 0f) angle += 360f;  // if angle negative, convert to 0..360
		return angle;
	}
}
