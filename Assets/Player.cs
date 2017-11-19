using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity {

	static public Player main;
	public float speed;

	// Use this for initialization
	void Start () {
		main = this;
		EntityInitialize();
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 velocity = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
		EntityUpdate(velocity.normalized * speed);
	}
}
