using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {
	[SerializeField] GameObject[] meteors;
	[SerializeField] AudioClip dieSound;
	[SerializeField] AudioClip dieSoundRare;
	[SerializeField] AchievmentUnlocker unlocker;

	private void Awake() {
		Instantiate(meteors.Random(), transform.position, Quaternion.Euler(Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f)), transform);
	}

	void Die() {
		if(Random.Range(0, 100) <= 10) {
			AudioManager.Instance.Play(dieSoundRare, transform, channel: AudioManager.AudioChannel.Sound);
			unlocker.Unlock();
		}
		else {
			AudioManager.Instance.Play(dieSound, transform, channel: AudioManager.AudioChannel.Sound);
		}
		GameFlow.instance.OnAsteroidDie();
		Destroy(gameObject);
	}
}
