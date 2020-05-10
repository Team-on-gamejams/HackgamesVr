using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour {
	public float movedDist = 0.0f;
	public bool isProcessInput = true;

	[Header("Moving")] [Space]
	[SerializeField] float moveSpeed = 4.0f;
	[SerializeField] float rotateSpeed = 0.5f;

	[Header("Inputs")] [Space]
	[SerializeField] ShipJoystick left1Joystick;
	[SerializeField] ShipJoystick left2Joystick;
	[SerializeField] ShipJoystick right1Joystick;
	[SerializeField] ShipJoystick right2Joystick;
	[SerializeField] SteamVR_Action_Boolean shootAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("RightJoystick", "Shoot");

	[Header("Particles")] [Space]
	[SerializeField] TrailRenderer[] trailsForward;
	[SerializeField] TrailRenderer[] trailsBack;

	[Header("UI")] [Space]
	[SerializeField] TextMeshProUGUI speedTextField = null;
	[SerializeField] TextMeshProUGUI timeTextField = null;

	[Header("Refs")] [Space]
	[SerializeField] Weapon playerWeapon = null;

	[Header("This refs")] [Space]
	[SerializeField] Rigidbody rb;

#if UNITY_EDITOR
	private void OnValidate() {
		if (rb == null)
			rb = GetComponent<Rigidbody>();
		if (playerWeapon == null)
			playerWeapon = GetComponent<Weapon>();
	}
#endif

	private void Awake() {
		Application.targetFrameRate = 60;
	}

	void Update() {
		if (!isProcessInput)
			return;

		Vector2 left1Value = left1Joystick.GetValue();
		Vector2 left2Value = left2Joystick.GetValue();

		Vector2 right1Value = right1Joystick.GetValue();
		Vector2 right2Value = right2Joystick.GetValue();

		if(left2Value.x != 0) {
			foreach (var trail in trailsForward)
				trail.emitting = true;
			foreach (var trail in trailsBack)
				trail.emitting = true;
		}
		else {
			foreach (var trail in trailsForward)
				trail.emitting = left1Value.x <= 0.0f;
			foreach (var trail in trailsBack)
				trail.emitting = left1Value.x > 0.0f;
		}

		//if(Mathf.Abs(left1Value.y) >= 0.45f && Mathf.Abs(right1Value.y) >= 0.45f && Mathf.Sign(left1Value.y) == Mathf.Sign(right1Value.y)) {
		//	right2Value.y = Mathf.Sign(left1Value.y);
		//	left1Value = Vector2.zero;
		//	right1Value = Vector2.zero;
		//}

		Vector3 tmp = Vector3.zero;
		Vector3 targetVelocity = transform.TransformDirection(new Vector3(left1Value.y * moveSpeed, left2Value.x * moveSpeed, left1Value.x * moveSpeed));
		if(targetVelocity != Vector3.zero)
			rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref tmp, 0.1f);
		rb.angularVelocity = transform.TransformDirection(new Vector3(right1Value.x * rotateSpeed, right1Value.y * rotateSpeed, -right2Value.y * rotateSpeed));
		movedDist += rb.velocity.magnitude;


		speedTextField.text = "Speed: " + rb.velocity.magnitude.ToString("0") + "m/s";
		timeTextField.text = DateTime.Now.ToShortTimeString();

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

	public void ApplyEngineUpgrade(float _moveSpeed) {
		moveSpeed += _moveSpeed;
	}

	void Die() {
		GameFlow.instance.OnLoseGame(false);
	}
}
