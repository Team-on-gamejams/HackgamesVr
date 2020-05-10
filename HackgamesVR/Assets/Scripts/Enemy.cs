using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public Action onDie;

	[SerializeField] Rigidbody rb;

	public float speed = 100f;
	public float rotateSpeed = 10f;

	public float maxRadius;
	public float minRadius;


	[Header("This Refs")] [Space]
	public Weapon weapon;

	[Header("Refs")] [Space]
	public Transform player;
	public Transform mothership;

	private Vector3 moveSpot;
	private Transform moveTarget = null;

	float collisionStayTime = 0.0f;
	

#if UNITY_EDITOR
	private void OnValidate() {
		if (rb == null)
			rb = GetComponent<Rigidbody>();
	}
#endif

	private void Start() {
		SpotSearch();
	}

	private void Update() {
		Debug.DrawLine(transform.position, moveTarget != null ? moveTarget.position : moveSpot, moveTarget != null ? Color.green : Color.red, Time.deltaTime);
	}

	private void FixedUpdate() {
		Vector3 targetPos = moveTarget != null ? moveTarget.position : moveSpot;
		float usedSpeed = speed;

		Vector3 tmp = Vector3.zero;

		Vector3 targetDirection = targetPos - transform.position;
		float singleStep = rotateSpeed * Time.deltaTime;
		Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
		transform.rotation = Quaternion.LookRotation(newDirection);

		if(moveTarget != null) {
			if ((transform.position - targetPos).sqrMagnitude < 1600.0f) {		//40 * 40
				weapon.IsShooting = true;
				usedSpeed /= 4;
			}
			else if ((transform.position - targetPos).sqrMagnitude < 9.0f) {		// 3 * 3
				SpotSearch();
			}
		}
		else {
			if ((transform.position - targetPos).sqrMagnitude < 1.0f) {
				SpotSearch();
			}
		}

		rb.velocity = Vector3.SmoothDamp(rb.velocity, transform.forward.normalized * usedSpeed, ref tmp, 0.1f, speed, Time.deltaTime);

	}

	private void OnCollisionStay(Collision collision) {
		collisionStayTime += Time.deltaTime;
		weapon.IsShooting = true;

		if (collisionStayTime >= 1.0f) {
			collisionStayTime = -3.0f;		//Так і має бути. Хай жде додатково 3 секунду після зміни, їм дуже тяжко вилетіти після зіткнення
			SpotSearch();
		}
	}

	private void OnCollisionEnter(Collision collision) {
		collisionStayTime = 0.0f;
	}

	private void OnCollisionExit(Collision collision) {
		weapon.IsShooting = false;
	}

	void SpotSearch() {
		float p = UnityEngine.Random.Range(0.0f, 1.0f);
		if (p <= 0.33f) {
			moveSpot = UnityEngine.Random.insideUnitSphere * (maxRadius - minRadius);

			if (moveSpot.x < 0)
				moveSpot.x -= minRadius;
			else
				moveSpot.x += minRadius;
			if (moveSpot.y < 0)
				moveSpot.y -= minRadius;
			else
				moveSpot.y += minRadius;
			if (moveSpot.z < 0)
				moveSpot.z -= minRadius;
			else
				moveSpot.z += minRadius;

			moveSpot += mothership.position;
			moveTarget = null;
		}
		else if (p <= 0.66f) {
			moveSpot = Vector3.zero;
			moveTarget = mothership;
		}
		else {
			moveSpot = Vector3.zero;
			moveTarget = player;
		}
		weapon.IsShooting = false;
	}

	void Die() {
		Destroy(gameObject);
		onDie?.Invoke();
	}
}
