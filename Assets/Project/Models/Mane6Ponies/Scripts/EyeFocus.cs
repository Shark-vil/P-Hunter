using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// TODO: Make it so we can have multiple objects that can be focused on and then give a priority value/distance they need to be to be looked at (and we'll look at the closest one) to each,
// so that she'll look at you when you're approaching but an item in your hand/your hand when you put it near her
// TODO: Make a script to shrink and grow eyes?

public class EyeFocus : MonoBehaviour {
	public Transform eyeLevel;
	public Transform target;
	public Material leftEyeMaterial;
	public Material rightEyeMaterial;

	public bool invertHorizontal;
	public bool invertVertical;

	public float lookSpeed = 10.00f;
	public float crossEye = 0.0f;

	public bool derpyMode;
	public float derpAmount = 0.15f;

	// Values that work for ponies
	private float maxIrisPanHoriOutward = 0.35f;
	private float maxIrisPanHoriInward = -0.35f;
	private float maxIrisPanVertUp = 0.3f;
	private float maxIrisPanVertDown = -0.25f;

	private GameObject focusObj;
	// Use this for initialization
	void Start () {
		focusObj = new GameObject ("EyeTrackingObj");
		focusObj.transform.parent = eyeLevel;
		focusObj.transform.localRotation = eyeLevel.localRotation;

		// Offset eyeLevel transform
		eyeLevel.transform.Rotate (0,0,90);
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 uvAnimationRateR = new Vector2 (0.0f, 0.0f);
		Vector2 uvAnimationRateL = new Vector2 (0.0f, 0.0f);

		// Eyes start to focus on target if found	
		if(target != null){
			focusObj.transform.position = target.position;

			// Get angle from our head to other object
			var localTarget = eyeLevel.transform.InverseTransformPoint(target.position);
			// Get angle between -pi and pi
			// TODO: This should be between -maxAngle and maxAngle from HeadLook
			float targetAngleX = Mathf.Atan2(localTarget.x, localTarget.z) + Mathf.PI/2;
			float targetAngleY = Mathf.Atan2(localTarget.x, localTarget.y) + Mathf.PI/2;

			// Calculate angle currently tracked object is from "eyeLevel" in both the X and Y direction and offset eye
			// texture by that amount
			if(!invertHorizontal){
				uvAnimationRateL.x = Mathf.Lerp(maxIrisPanHoriInward, maxIrisPanHoriOutward, (targetAngleX + Mathf.PI/2)/Mathf.PI);
				uvAnimationRateR.x = Mathf.Lerp(maxIrisPanHoriInward, maxIrisPanHoriOutward, (targetAngleX + Mathf.PI/2)/Mathf.PI);
			}else{
				uvAnimationRateL.x = Mathf.Lerp(maxIrisPanHoriOutward, maxIrisPanHoriInward, (targetAngleX + Mathf.PI/2)/Mathf.PI);
				uvAnimationRateR.x = Mathf.Lerp(maxIrisPanHoriOutward, maxIrisPanHoriInward, (targetAngleX + Mathf.PI/2)/Mathf.PI);
			}

			if(!invertVertical){
				uvAnimationRateL.y = Mathf.Lerp(maxIrisPanVertDown, maxIrisPanVertUp, (targetAngleY + Mathf.PI/2)/Mathf.PI);
				uvAnimationRateR.y = Mathf.Lerp(maxIrisPanVertDown, maxIrisPanVertUp, (targetAngleY + Mathf.PI/2)/Mathf.PI);
			}else{
				uvAnimationRateL.y = Mathf.Lerp(maxIrisPanVertUp, maxIrisPanVertDown, (targetAngleY + Mathf.PI/2)/Mathf.PI);
				uvAnimationRateR.y = Mathf.Lerp(maxIrisPanVertUp, maxIrisPanVertDown, (targetAngleY + Mathf.PI/2)/Mathf.PI);
			}

			// Set the 2D texture offset
			uvAnimationRateL = new Vector2(Mathf.Clamp(uvAnimationRateL.x, -maxIrisPanHoriOutward, -maxIrisPanHoriInward) + crossEye + 0.15f, 
				Mathf.Clamp(uvAnimationRateL.y + (derpyMode ? derpAmount : 0.0f), maxIrisPanVertDown, maxIrisPanVertUp + (derpyMode ? derpAmount : 0.0f)));
			uvAnimationRateR = new Vector2(Mathf.Clamp(uvAnimationRateR.x, maxIrisPanHoriInward, maxIrisPanHoriOutward) - crossEye - 0.15f, 
				Mathf.Clamp(uvAnimationRateR.y - (derpyMode ? derpAmount : 0.0f), maxIrisPanVertDown - (derpyMode ? derpAmount : 0.0f), maxIrisPanVertUp ));
		}else{
			// Eyes Return to Original Position if target is not found
			uvAnimationRateR = Vector2.Lerp(uvAnimationRateR, Vector2.zero, Time.deltaTime);
			uvAnimationRateL = Vector2.Lerp(uvAnimationRateL, Vector2.zero, Time.deltaTime);
		}

		leftEyeMaterial.mainTextureOffset  = uvAnimationRateL;
		rightEyeMaterial.mainTextureOffset = uvAnimationRateR;
	}


}
