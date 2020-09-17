using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFede : MonoBehaviour
{
	[Header("Stats")]
	public float speed = 500f;

	[Header("Ground Check")]
	public bool grounded = true;
	public Transform playerBottom;
	public LayerMask groundLayer;
	public float groundDistance = 0.05f;

	[Header("Explosion stuff")]
	public float flyForce = 300;
	private Vector3 flyDirection;

	private Rigidbody rig;
	private Vector3 movementInput = Vector3.zero;

	void Start()
	{
		rig = GetComponent<Rigidbody>();
		flyDirection = (Vector3.up + Vector3.right);
	}

	void Update()
	{
		grounded = Physics.Raycast(playerBottom.position, Vector3.down, groundDistance, groundLayer);

		movementInput = Vector3.zero;
		movementInput.x = Input.GetAxis("Horizontal");
		movementInput.z = Input.GetAxis("Vertical");

		if (Input.GetKeyDown(KeyCode.Space))
		{
			//rig.velocity = Vector3.zero; // acá fijate si querés que pase esto o que mantenga la vel
			rig.AddForce(flyDirection * flyForce);
			grounded = false;
		}
	}

	void FixedUpdate()
	{
		if (grounded)
		{
			Vector3 oldVel = rig.velocity;
			oldVel.x = movementInput.x * speed * Time.fixedDeltaTime;
			oldVel.z = movementInput.z * speed * Time.fixedDeltaTime;
			rig.velocity = oldVel;
		}
	}
}
