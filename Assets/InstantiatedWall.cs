using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatedWall : MonoBehaviour {

	private Transform _transform;
	private Collider2D _collider;

	public void Initialize (Vector3 _position, Vector3 _scale) {

		if (_transform == null) _transform = transform;
		_transform.position = _position;
		_transform.localScale = _scale;

		if (_collider == null) _collider = GetComponent<Collider2D>();
		_collider.enabled = true;

		/*
		GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.25f,1f),Random.Range(0.25f,1f),Random.Range(0.25f,1f));
		GetComponent<SpriteRenderer>().enabled = true;
		*/

	}

	public void Disable () {
		_collider.enabled = false;
		//GetComponent<SpriteRenderer>().enabled = false;
	}

}
