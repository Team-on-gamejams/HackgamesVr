using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class ShipJoystick : MonoBehaviour {
	[SerializeField] Interactable interactable;

	[SerializeField] float xOffset = 45.0f;

	private Hand handHoverLocked = null;
	private bool driving = false;

	Vector3 bounds1 = new Vector3(-90, 0, -90);
	Vector3 bounds2 = new Vector3(0, 0, 90);
	Vector2 value = Vector3.zero;

#if UNITY_EDITOR
	private void OnValidate() {
		if (interactable == null)
			interactable = GetComponent<Interactable>();
	}
#endif

	void OnDisable() {
		if (handHoverLocked) {
			handHoverLocked.HideGrabHint();
			handHoverLocked.HoverUnlock(interactable);
			handHoverLocked = null;
		}
	}

	public Vector2 GetValue() {
		return value;
	}

	private IEnumerator HapticPulses(Hand hand, float flMagnitude, int nCount) {
		if (hand != null) {
			int nRangeMax = (int)Util.RemapNumberClamped(flMagnitude, 0.0f, 1.0f, 100.0f, 900.0f);
			nCount = Mathf.Clamp(nCount, 1, 10);

			for (ushort i = 0; i < nCount; ++i) {
				ushort duration = (ushort)Random.Range(100, nRangeMax);
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
}
