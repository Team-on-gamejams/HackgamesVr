using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MotherShip : MonoBehaviour {
	[SerializeField] Rigidbody rb;
	[SerializeField] Vector3 speed = Vector3.right;
	[SerializeField] AudioClip dieSound;

#if UNITY_EDITOR
	private void OnValidate() {
		if (rb == null)
			rb = GetComponent<Rigidbody>();
	}
#endif

	private void Update() {
		Vector3 tmp = Vector3.zero;
		rb.velocity = Vector3.SmoothDamp(rb.velocity, transform.TransformDirection(speed), ref tmp, 0.1f);;
	}

	void Die() {
		GameFlow.instance.OnLoseGame(true);
		AudioManager.Instance.Play(dieSound, transform, channel: AudioManager.AudioChannel.Sound);
		for(int i = 1; i <= 4; ++i) {
			LeanTween.delayedCall(Random.Range(0.1f + i * 0.3f, 0.4f + i * 0.3f), () => {
				AudioManager.Instance.Play(dieSound, channel: AudioManager.AudioChannel.Sound);
			});
		}
		Destroy(gameObject);
	}
}
