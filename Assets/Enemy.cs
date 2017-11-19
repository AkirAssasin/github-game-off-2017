using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {

	private Player player;

	// FOLLOW
	public bool followPlayer;
	public float followAngleSpeed;
	public float minimumDistance;
	public float stillDistance;
	public float maximumDistance;

	// HIT N' TURN
	public float collideTurnAngle;
	public float constantTurnAngle;

	// MISC.
	public float movementSpeed;

	// WEAPONS
	public float weaponSwingSpeed;
	public int offensiveWeapon;
	public int defensiveWeapon;

	// CURRENTS
	public float moveAngle;
	public bool wasFollowingPlayer;

	public LayerMask sightRaycastMask;

	void Start () {
		EntityInitialize();
		player = Player.main;
	}

	void Update () {

		float dt = Time.deltaTime;

		if (player == null) {
			player = Player.main;
		} else {

			float currentSpeed = movementSpeed;

			if (followPlayer) {

				RaycastHit2D blocked = Physics2D.Linecast(position,player.position,sightRaycastMask);

				if (!blocked) {

					Vector2 delta = player.position - position;
					float distanceToPlayer = delta.magnitude;

					if (distanceToPlayer <= maximumDistance) {

						if (!wasFollowingPlayer) {
							wasFollowingPlayer = true;
							moveAngle = Vector2ToAngle(delta);
						}

						moveAngle = Mathf.MoveTowardsAngle(moveAngle,Vector2ToAngle(delta),followAngleSpeed * dt);

						if (distanceToPlayer <= stillDistance) {
							if (distanceToPlayer < minimumDistance) {
								currentSpeed = -movementSpeed;
							} else {
								currentSpeed = 0;
							}
						}
					} else {
						wasFollowingPlayer = false;
						currentSpeed = 0;
					}

				} else {
					wasFollowingPlayer = false;
					currentSpeed = 0;
				}

			}

			EntityUpdate(RadianToVector2(moveAngle * Mathf.Deg2Rad) * currentSpeed);

		}
	}

	Vector2 RadianToVector2 (float radian) {
		return new Vector2(Mathf.Cos(radian),Mathf.Sin(radian));
	}

	float Vector2ToAngle (Vector2 vector2) {
		return Mathf.Atan2(vector2.y,vector2.x) * Mathf.Rad2Deg;
	}

}
