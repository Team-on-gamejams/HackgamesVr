using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour {

	[SerializeField] ShipJoystick leftJoystick;
	[SerializeField] ShipJoystick rightJoystick;

	[SerializeField] Rigidbody rb;

#if UNITY_EDITOR
	private void OnValidate() {
		if (rb == null)
			rb = GetComponent<Rigidbody>();
	}
#endif


	void Update() {
		if (leftJoystick.transform.localEulerAngles.x != 315f) {
			rb.velocity = Vector3.forward * 1.0f;
			Debug.Log("Moving");
		}
	}
}
