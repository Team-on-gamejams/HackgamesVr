using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Valve.VR;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
	public float Damage {
		get {
			float damage = UnityEngine.Random.Range(bulletDamage.x, bulletDamage.y);

			IsCritical = RandomEx.GetEventWithChance(criticalChance);
			if (IsCritical) {
				damage *= criticalMultiplier;
			}

			return damage;
		}
	}
	public bool IsCritical { get; set; } = false;

	[NonSerialized] public Vector3 collisionPoint;

	[Tag] [SerializeField] string ignoreTag = "Player";

	[Space]
	[MinMaxSlider(1, 100)] [SerializeField] Vector2 bulletDamage = new Vector2(8, 12);   //TODO: make dmg struct from this
	[SerializeField] float maxDistance = 20;
	[SerializeField] float bulletSpeed = 25;

	[Space]
	[SerializeField] byte criticalChance = 10;
	[SerializeField] float criticalMultiplier = 1.5f;

	[HideInInspector] [SerializeField] Rigidbody rb = null;

	float currDist = 0;
	Vector3 prevPos;
	bool isHit = false;

#if UNITY_EDITOR
	private void OnValidate() {
		if (rb == null)
			rb = GetComponent<Rigidbody>();
	}
#endif

	void Awake() {
		prevPos = transform.position;
	}

	void Update() {
		if (isHit)
			return;

		currDist += (transform.position - prevPos).magnitude;
		prevPos = transform.position;

		if (currDist >= maxDistance) {
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter(Collider collision) {
		if (collision.transform.tag == ignoreTag || collision.transform.tag == "Bullet" || collision.isTrigger || isHit)
			return;

		Health health = collision.transform.GetComponent<Health>();
		if (health == null)
			health = collision.transform.GetComponentInParent<Health>();


		if (health == null)
			return;

		isHit = true;
		collisionPoint = collision.ClosestPoint(transform.position);

		if (health)
			health.OnCollideWithProjectile(this);
		Destroy(gameObject);
	}

	public void Shoot(Vector3 shooterVelocity) {
		rb.velocity += transform.forward.normalized * bulletSpeed + shooterVelocity;
	}
}
