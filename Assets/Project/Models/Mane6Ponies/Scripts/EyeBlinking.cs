using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBlinking : MonoBehaviour {

	public float secondsBetweenBlinks = 5f;
	public float blinkSpeed = 10f;
	public SkinnedMeshRenderer ponyBody;
	public string eyeBlendShapeName = "Happy_eyes2R+Happy_eyes2L";

	private float minBlend = 0f;
	private float maxBlend = 100f;
	private float currBlend = 0f;
	private bool isBlinking = false;
	private bool isUnblinking = false;

	// Use this for initialization
	void Start () {
		// Blink every x seconds
		float initialBlinkOffset = Random.Range(1, 5.0f);
		InvokeRepeating("Blink", secondsBetweenBlinks + initialBlinkOffset, secondsBetweenBlinks);
	}
		
	void ChangeBlend(float delta) {
		currBlend+=delta;
		int index = ponyBody.sharedMesh.GetBlendShapeIndex(eyeBlendShapeName); 
		ponyBody.SetBlendShapeWeight (index, currBlend);
	}
	
	// Update is called once per frame
	void Update () {
		if (isBlinking)
			ChangeBlend (blinkSpeed);
		if (isUnblinking)
			ChangeBlend (-blinkSpeed);

		if (currBlend >= maxBlend) {
			isBlinking = false;
			isUnblinking = true;
		}

		if (currBlend <= minBlend)
			isUnblinking = false;
	}

	void Blink () {
		isBlinking = true;
	}
}
