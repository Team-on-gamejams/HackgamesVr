using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour {

	[SerializeField] float moveSpeed = 4.0f;
	[SerializeField] float rotateSpeed = 0.5f;

	[SerializeField] Weapon playerWeapon = null;

	[SerializeField] ShipJoystick left1Joystick;
	[SerializeField] ShipJoystick left2Joystick;
	[SerializeField] ShipJoystick right1Joystick;
	[SerializeField] ShipJoystick right2Joystick;

	[SerializeField] SteamVR_Action_Boolean shootAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("RightJoystick", "Shoot");

	[SerializeField] TrailRenderer[] trailsForward;
	[SerializeField] TrailRenderer[] trailsBack;

	[SerializeField] Rigidbody rb;

#if UNITY_EDITOR
	private void OnValidate() {
		if (rb == null)
			rb = GetComponent<Rigidbody>();
		if (playerWeapon == null)
			playerWeapon = GetComponent<Weapon>();
	}
#endif


	void Update() {
		Vector2 left1Value = left1Joystick.GetValue();
		Vector2 left2Value = left2Joystick.GetValue();

		Vector2 right1Value = right1Joystick.GetValue();
		Vector2 right2Value = right2Joystick.GetValue();


		foreach (var trail in trailsForward)
			trail.emitting = left1Value.x <= 0.0f;
		foreach (var trail in trailsBack)
			trail.emitting = left1Value.x > 0.0f;

		Vector3 tmp = Vector3.zero;
		rb.velocity = Vector3.SmoothDamp(rb.velocity, transform.TransformDirection(new Vector3(left1Value.y * moveSpeed, left2Value.x * moveSpeed, left1Value.x * moveSpeed)), ref tmp, 0.1f);
		rb.angularVelocity = transform.TransformDirection(new Vector3(right1Value.x * rotateSpeed, right1Value.y * rotateSpeed, right2Value.y * rotateSpeed));

		Interactable shootInteractable = right1Joystick.interactable;
		if(shootInteractable == null)
			shootInteractable = right2Joystick.interactable;
		if (shootInteractable != null && shootInteractable.attachedToHand) {
			SteamVR_Input_Sources hand = shootInteractable.attachedToHand.handType;
			playerWeapon.IsShooting = shootAction[hand].state;
		}
		else {
			playerWeapon.IsShooting = false;
		}
	}

	void Die() {
		//TODO: lose
	}
}
