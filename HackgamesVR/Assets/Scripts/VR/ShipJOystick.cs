using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class ShipJOystick : MonoBehaviour {
	public void OnHandHoverBegin(Hand hand) {
		hand.ShowGrabHint();
	}

	public void OnHandHoverEnd(Hand hand) {
		hand.HideGrabHint();
	}
}
