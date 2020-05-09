using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour {

	[SerializeField] float moveSpeed = 4.0f;
	[SerializeField] float rotateSpeed = 0.5f;

	[SerializeField] ShipJoystick leftJoystick;
	[SerializeField] ShipJoystick rightJoystick;

	[SerializeField] TrailRenderer[] trailsForward;
	[SerializeField] TrailRenderer[] trailsBack;

	[SerializeField] Rigidbody rb;

#if UNITY_EDITOR
	private void OnValidate() {
		if (rb == null)
			rb = GetComponent<Rigidbody>();
	}
#endif


	void Update() {
		Vector2 leftValue = leftJoystick.GetValue();
		Vector2 rightValue = rightJoystick.GetValue();

		Debug.Log($"{leftValue} {rightValue}");
		Debug.Log($"{rb.velocity} {rb.angularVelocity}");
		Vector3 tmp = Vector3.zero;

		foreach (var trail in trailsForward)
			trail.emitting = leftValue.x <= 0.0f;
		foreach (var trail in trailsBack)
			trail.emitting = leftValue.x > 0.0f;

		rb.velocity = Vector3.SmoothDamp(rb.velocity, transform.TransformDirection(new Vector3(leftValue.y * moveSpeed, 0, leftValue.x * moveSpeed)), ref tmp, 0.1f);
		rb.angularVelocity = transform.TransformDirection(new Vector3(rightValue.x * rotateSpeed, rightValue.y * rotateSpeed, 0));
	}
}
