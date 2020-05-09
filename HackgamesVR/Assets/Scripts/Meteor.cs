using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {
	[SerializeField] GameObject[] meteors;
	private void Awake() {
		Instantiate(meteors.Random(), transform.position, Quaternion.Euler(Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f)), transform);
	}

	void Die() {
		Destroy(gameObject);
	}
}
