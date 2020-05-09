using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Valve.VR;
using Valve.VR.InteractionSystem;
public class HandWeapon : MonoBehaviour {
	[SerializeField] SteamVR_Action_Boolean shootAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Pistol", "Shoot");
	[SerializeField] Weapon weapon = null;
	[SerializeField] Interactable interactable = null;


#if UNITY_EDITOR
	private void OnValidate() {
		if (weapon == null)
			weapon = GetComponent<Weapon>();
		if (interactable == null)
			interactable = GetComponent<Interactable>();
	}
#endif

	void Update() {
		if (interactable.attachedToHand) {
			SteamVR_Input_Sources hand = interactable.attachedToHand.handType;
			weapon.IsShooting = shootAction[hand].state;
		}
		else {
			weapon.IsShooting = false;
		}
	}
}
