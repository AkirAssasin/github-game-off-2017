using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public static Vector2 mousePosition;

	public Transform target;
	public float zDisplacement;
	public float lerpRate;
	private Transform _transform;

	private new Camera camera;

	public float halfwidth;
	public float halfheight;

	public float minimumX;
	public float minimumY;
	public float maximumX;
	public float maximumY;

	public Transform cursor;

	public MapGenerator mapGenerator;

	void Start () {
		_transform = transform;
		mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		camera = GetComponent<Camera>();

		halfwidth = camera.ViewportToWorldPoint(new Vector3(1,0,0)).x - _transform.position.x;
		halfheight = camera.ViewportToWorldPoint(new Vector3(0,1,0)).y - _transform.position.y;

		minimumX += halfwidth;
		minimumY += halfheight;
		maximumX -= halfwidth;
		maximumY -= halfheight;

		_transform.position = new Vector3(Mathf.Clamp(target.position.x,minimumX,maximumX),Mathf.Clamp(target.position.y,minimumY,maximumY),zDisplacement);
		Cursor.visible = false;

	}

	void FixedUpdate () {

		Vector3 focalPosition;

		mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		cursor.position = mousePosition;
		Vector3 targetPosition = Vector3.Lerp(target.position,mousePosition,0.5f);

		if (mapGenerator.GetFocalPoint(Mathf.RoundToInt(target.position.x),Mathf.RoundToInt(target.position.y), out focalPosition)) {

			targetPosition = Vector3.Lerp(targetPosition,focalPosition,0.5f);

		}

		Vector3 idealPosition = new Vector3(Mathf.Clamp(targetPosition.x,minimumX,maximumX),Mathf.Clamp(targetPosition.y,minimumY,maximumY),zDisplacement);
		_transform.position = Vector3.Lerp(_transform.position,idealPosition,Time.fixedDeltaTime * lerpRate);

	}

}
