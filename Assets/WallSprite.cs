using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSprite : MonoBehaviour {

	private Transform _transform;
	private SpriteRenderer _renderer;

	public void Initialize (Vector3 _position, Sprite _sprite) {

		if (_transform == null) _transform = transform;
		_transform.position = _position;

		if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
		_renderer.enabled = true;
		_renderer.sprite = _sprite;

		_renderer.sortingOrder = -Mathf.RoundToInt(_transform.position.y * 10f);

		/*
		GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.25f,1f),Random.Range(0.25f,1f),Random.Range(0.25f,1f));
		GetComponent<SpriteRenderer>().enabled = true;
		*/

	}

	public void Disable () {
		_renderer.enabled = false;
	}

}