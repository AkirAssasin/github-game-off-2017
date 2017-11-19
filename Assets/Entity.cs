using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Entity : MonoBehaviour {

	private BounceAnimation anim;
	private new Rigidbody2D rigidbody;
	public SpriteRenderer sprite;

	[Header("Entity Settings")]
	public float movingAnimationRate;
	public float idleAnimationRate;
	public Vector2 position;

	public void EntityInitialize () {
		
		anim = GetComponentInChildren<BounceAnimation>();
		rigidbody = GetComponent<Rigidbody2D>();
		sprite = GetComponentInChildren<SpriteRenderer>();

		anim.animationRate = idleAnimationRate;
		position = rigidbody.position;

	}

	public void EntityUpdate (Vector2 velocity) {
		
		position = rigidbody.position;
		sprite.sortingOrder = -Mathf.RoundToInt(position.y * 10f);

		rigidbody.velocity = velocity;

		if (velocity == Vector2.zero) {
			anim.animationRate = idleAnimationRate;
		} else {
			anim.animationRate = movingAnimationRate;

			if (velocity.x != 0) sprite.flipX = velocity.x < 0;
		}

	}
}
