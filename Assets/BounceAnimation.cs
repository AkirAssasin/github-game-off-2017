using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAnimation : MonoBehaviour {

	public float swing;
	public float bounce;

	public float baseY;

	public float animationRate;
	private float t;

	void Update () {

		t += Time.deltaTime * animationRate;
		transform.localPosition = new Vector3(0,baseY + Mathf.Abs(Mathf.Sin(t)) * bounce,0);
		transform.localEulerAngles = new Vector3(0,0,Mathf.Cos(t) * swing);

	}
		
}
