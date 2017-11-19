using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Chakram : MonoBehaviour {

	private new Rigidbody2D rigidbody;
	private new Collider2D collider;
	private new SpriteRenderer renderer;
	private Player player;

	public bool playerHolding;

	public float initialSpeed;
	public float pickupRange;

	public bool leftPickupRange;

	public float collisionSpeedCoefficient;

	public float moveAngle;
	public float moveSpeed;

	public float spinAnglePerSpeed;

	public Sprite playerUnarmed;
	public Sprite playerArmed;

	public GameObject sparkPrefab;

	public bool leftCollision;

	void Start () {

		rigidbody = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();
		renderer = GetComponent<SpriteRenderer>();
		player = Player.main;

		playerHolding = true;
		rigidbody.velocity = Vector2.zero;
		collider.enabled = false;
		renderer.enabled = false;

		leftCollision = true;

	}

	void Update () {

		if (Input.GetMouseButton(0) && playerHolding) {

			playerHolding = false;
			transform.position = player.transform.position;

			moveSpeed = initialSpeed;
			moveAngle = Vector2ToAngle(CameraController.mousePosition - player.position);

			collider.enabled = true;
			renderer.enabled = true;

			leftPickupRange = false;

		}

		if (!playerHolding) {
			transform.eulerAngles += Vector3.forward * Time.deltaTime * spinAnglePerSpeed * moveSpeed;
			renderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10f);
		}

	}
		
	void FixedUpdate () {

		float dt = Time.fixedDeltaTime;

		if (player == null) {
			player = Player.main;
		} else {
			player.sprite.sprite = playerHolding ? playerArmed : playerUnarmed;
		}

		if (!playerHolding) {

			if (Vector2.Distance(player.position,rigidbody.position) < pickupRange) {

				if (leftPickupRange) {

					playerHolding = true;

					moveSpeed = 0;

					collider.enabled = false;
					renderer.enabled = false;

				}

			} else {
				leftPickupRange = true;
			}

		}

		rigidbody.velocity = RadianToVector2(moveAngle * Mathf.Deg2Rad) * moveSpeed;

	}

	void OnCollisionEnter2D (Collision2D collision) {

		if (leftCollision) {
			ContactPoint2D[] contact = new ContactPoint2D[1];
			collision.GetContacts(contact);

			moveSpeed *= collisionSpeedCoefficient;
			float oldMoveAngle = moveAngle - 180f;
			moveAngle = ReflectAngle(moveAngle,contact[0].normal);

			float sparkDuration = 0.3f + ((moveSpeed/initialSpeed) * 0.4f);

			for (int i = 0; i < 2; i++) {
				Spark.GetFromPool(sparkPrefab).Initialize(contact[0].point,oldMoveAngle + Random.Range(-5f,5f),
					sparkDuration,
					(moveSpeed/initialSpeed) * Random.Range(0.5f,2f),0.2f,Random.Range(1f,2f));
			}

			for (int i = 0; i < 2; i++) {
				Spark.GetFromPool(sparkPrefab).Initialize(contact[0].point,moveAngle + Random.Range(-5f,5f),
					sparkDuration,
					(moveSpeed/initialSpeed) * Random.Range(0.5f,2f),0.2f,Random.Range(1f,2f));
			}
		}

		leftCollision = false;
			
	}

	void OnCollisionExit2D () {
		leftCollision = true;
	}

	float ReflectAngle (float angle, Vector2 normal) {
		return Vector2ToAngle(Vector2.Reflect(RadianToVector2(angle * Mathf.Deg2Rad),normal));
	}

	Vector2 RadianToVector2 (float radian) {
		return new Vector2(Mathf.Cos(radian),Mathf.Sin(radian));
	}

	float Vector2ToAngle (Vector2 vector2) {
		return Mathf.Atan2(vector2.y,vector2.x) * Mathf.Rad2Deg;
	}

}
