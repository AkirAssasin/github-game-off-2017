using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrderPosition : MonoBehaviour {

	private SpriteRenderer _renderer;
	private Transform _transform;

	void Start () {
		_renderer = GetComponent<SpriteRenderer>();
		_transform = transform;
	}

	void Update () {
		_renderer.sortingOrder = -Mathf.RoundToInt(_transform.position.y * 10f);
	}
}
