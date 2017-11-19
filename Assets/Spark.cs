using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : MonoBehaviour {

	public static List<Spark> pool = new List<Spark>();
	public bool inPool;

	public static Spark GetFromPool (GameObject fallback) {

		Spark result;

		if (pool.Count > 0) {
			result = pool[pool.Count - 1];
			pool.RemoveAt(pool.Count - 1);
		} else {
			result = ((GameObject)Instantiate(fallback)).GetComponent<Spark>();
		}

		return result;

	}

	public float increment;
	public float life;

	public float length;
	public float width;

	public Vector3 velocity;

	public SpriteRenderer sprite;

	public void Initialize (Vector3 _position, float _angle, float _duration, float _length, float _width, float _speed) {

		inPool = false;
		life = 0;

		length = _length;
		width = _width;

		transform.position = _position;
		transform.eulerAngles = new Vector3(0,0,_angle);
		increment = 1/_duration;

		transform.localScale = new Vector3(length,width,1);

		if (sprite == null) sprite = GetComponent<SpriteRenderer>();
		sprite.enabled = true;

		velocity = new Vector3(Mathf.Cos(_angle * Mathf.Deg2Rad) * _speed,Mathf.Sin(_angle * Mathf.Deg2Rad) * _speed,0);

	}

	void Update () {

		if (!inPool) {

			life += increment * Time.deltaTime;
			transform.localScale = new Vector3(length * (1 - life),width * (1 - life),1);
			transform.position += velocity * Time.deltaTime;
			sprite.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);

			if (life >= 1) {
				
				pool.Add(this);
				inPool = true;

				sprite.enabled = false;

			}

		}

	}
}
