using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

#if VR_VERSION
using Valve.VR;
using Valve.VR.InteractionSystem;
#endif

public class ShipJoystick : MonoBehaviour {
	public Action OnTriggerPress;
	public Action OnTriggerRelease;

#if VR_VERSION
	[Header("VR controls")] [Space]
	public Interactable interactable;

	[SerializeField] SteamVR_Action_Boolean lockAction;
	[SerializeField] SteamVR_Action_Boolean triggerAction;

	Hand handHoverLocked = null;
	bool driving;
#else
	[Header("Keyboard controls")] [Space]
	[SerializeField] string axisX = "";
	[SerializeField] string axisY = "";
	[SerializeField] string axisTrigger = "";
#endif

	[SerializeField] float xOffset = 45.0f;

	Vector3 bounds1 = new Vector3(-90, 0, -90);
	Vector3 bounds2 = new Vector3(0, 0, 90);
	Vector2 value = Vector3.zero;

#if UNITY_EDITOR
	private void OnValidate() {
#if VR_VERSION
		if (interactable == null)
			interactable = GetComponent<Interactable>();
#endif
	}
#endif

	void OnDisable() {
#if VR_VERSION
		if (handHoverLocked) {
			handHoverLocked.HideGrabHint();
			handHoverLocked.HoverUnlock(interactable);
			handHoverLocked = null;
	}
#endif
	}

	private void Update() {
#if VR_VERSION
		if (interactable != null && interactable.attachedToHand) {
			SteamVR_Input_Sources hand = interactable.attachedToHand.handType;
			if (triggerAction[hand].state) {
				//TODO: call OnTriggerPress only 1 time
				OnTriggerPress?.Invoke();
			}
			else {
				OnTriggerRelease?.Invoke();
			}
		}
		else {
			OnTriggerRelease?.Invoke();
		}
#else
		if(axisTrigger != "") {
			if (Input.GetAxisRaw(axisTrigger) >= 0.5f) {
				//TODO: call OnTriggerPress only 1 time
				OnTriggerPress?.Invoke();
			}
			else {
				OnTriggerRelease?.Invoke();
			}
		}
#endif
	}

	public Vector2 GetValue() {
		if (!isActiveAndEnabled)
			return value;

#if VR_VERSION
		return value;
#else
		if (axisX != "")
			value.x = Input.GetAxisRaw(axisX);
		if (axisY != "")
			value.y = Input.GetAxisRaw(axisY);
		return value;
#endif
	}

#if VR_VERSION
	private IEnumerator HapticPulses(Hand hand, float flMagnitude, int nCount) {
		if (hand != null) {
			int nRangeMax = (int)Util.RemapNumberClamped(flMagnitude, 0.0f, 1.0f, 100.0f, 900.0f);
			nCount = Mathf.Clamp(nCount, 1, 10);

			for (ushort i = 0; i < nCount; ++i) {
				ushort duration = (ushort)UnityEngine.Random.Range(100, nRangeMax);
				hand.TriggerHapticPulse(duration);
				yield return new WaitForSeconds(.01f);
			}
		}
	}

	private void OnHandHoverBegin(Hand hand) {
		hand.ShowGrabHint();
	}

	private void OnHandHoverEnd(Hand hand) {
		hand.HideGrabHint();

		if (driving && hand) {
			StartCoroutine(HapticPulses(hand, 1.0f, 10));
		}

		driving = false;
		handHoverLocked = null;
	}

	private GrabTypes grabbedWithType;
	private void HandHoverUpdate(Hand hand) {
		GrabTypes startingGrabType = hand.GetGrabStarting();
		bool isGrabEnding = hand.IsGrabbingWithType(grabbedWithType) == false;

		if (grabbedWithType == GrabTypes.None && startingGrabType != GrabTypes.None) {
			grabbedWithType = startingGrabType;

			hand.AttachObject(gameObject, startingGrabType, Hand.AttachmentFlags.DetachFromOtherHand);
			hand.HoverLock(interactable);
			handHoverLocked = hand;

			driving = true;

			hand.HideGrabHint();
		}
		else if (grabbedWithType != GrabTypes.None && isGrabEnding) {
			hand.DetachObject(gameObject);
			hand.HoverUnlock(interactable);
			handHoverLocked = null;
			driving = false;
			grabbedWithType = GrabTypes.None;

			transform.localEulerAngles = new Vector3(-45f, 0, 0);
			value.x = 0;
			value.y = 0;
		}

		if (interactable.attachedToHand) {
			SteamVR_Input_Sources handIn = interactable.attachedToHand.handType;
			if (lockAction[handIn].state) {
				hand.DetachObject(gameObject);
				hand.HoverUnlock(interactable);
				driving = false;
				grabbedWithType = GrabTypes.None;
				return;
			}
		}

		if (driving && isGrabEnding == false && hand.hoveringInteractable == this.interactable) {
			transform.localRotation = ClampRotation(hand.transform.localRotation, bounds1, bounds2);

			//Update ship angle
		}
	}

	public Quaternion ClampRotation(Quaternion q, Vector3 boundsMin, Vector3 boundsMax) {
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
		angleX = Mathf.Clamp(angleX, boundsMin.x, boundsMax.x);
		q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

		float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
		angleY = Mathf.Clamp(angleY, boundsMin.y, boundsMax.y);
		q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

		float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
		angleZ = Mathf.Clamp(angleZ, boundsMin.z, boundsMax.z);
		q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

		value.x = (angleX + 45f) / 45f;
		value.y = angleZ / -90.0f;

		return q;
	}
#endif
}
