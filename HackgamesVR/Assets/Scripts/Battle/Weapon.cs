using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour {
	public int reloads = 0;
	public int shootedProjectiles = 0;

	public bool IsShooting = false;

	[Header("Projectile")]
	[Space]
	[SerializeField] Projectile projectilePrefab = null;
	[SerializeField] Transform[] shootPos = null;
	[SerializeField] float shootingCooldown = 0.15f;

	[Header("Reloading")]
	[Space]
	[SerializeField] byte clipSize = 10;
	[SerializeField] float reloadTime = 1.5f;

	[Header("UI")]
	[Space]
	[SerializeField] bool isNeedUI = false;
	[SerializeField] TextMeshProUGUI clipTextField = null;
	[SerializeField] TextMeshProUGUI reloadTextField = null;

	[Header("Refs")]
	[Space]
	[SerializeField] Rigidbody parentPb = null;

	float currShootingCooldown = 0;
	byte currShootPos = 0;
	byte currClip = 0;
	Coroutine reloadRoutine = null;
	Projectile currProjectile;

	private void Awake() {
		currClip = clipSize;

		if (isNeedUI) {
			reloadTextField.gameObject.SetActive(false);
			clipTextField.text = new string('|', currClip);
		}
	}

	private void Update() {
		currShootingCooldown += Time.deltaTime;
		if (IsShooting)
			Shoot();
	}

	public bool IsReloading() {
		return reloadRoutine != null;
	}

	public void Reload() {
		if (reloadRoutine != null)
			return;

		++reloads;
		int dots = 0;
		reloadRoutine = StartCoroutine(ReloadRoutine());

		if (isNeedUI) {
			reloadTextField.gameObject.SetActive(true);
		}

		IEnumerator ReloadRoutine() {
			float currTime = 0.0f;

			while (currTime < reloadTime) {
				currTime += Time.deltaTime + 0.25f;
				if (isNeedUI)
					reloadTextField.text = "Reloading" + (dots != 0 ? new string('.', dots) : "");
				yield return new WaitForSeconds(0.25f);
				++dots;
				if (dots >= 4)
					dots = 0;
			}


			currClip = clipSize;
			reloadRoutine = null;

			if (isNeedUI) {
				clipTextField.text = new string('|', currClip);
				reloadTextField.gameObject.SetActive(false);
			}
		}
	}

	public void InterruptReload() {
		if (reloadRoutine == null)
			return;

		StopCoroutine(reloadRoutine);
		reloadRoutine = null;

		if (isNeedUI) {
			reloadTextField.gameObject.SetActive(true);
		}
	}

	public bool IsClipEmpty() {
		return currClip <= 0;
	}

	public bool IsClipFull() {
		return currClip == clipSize;
	}

	public void ResetCooldown() {
		currShootingCooldown = shootingCooldown;
	}

	void Shoot() {
		if (currClip > 0) {
			if (IsReloading())
				InterruptReload();

			if (currShootingCooldown >= shootingCooldown) {
				currShootingCooldown = 0;

				currProjectile = Instantiate(projectilePrefab, shootPos[currShootPos].position, transform.rotation, transform).GetComponent<Projectile>();
				LaunchBullet();

				--currClip;

				if (isNeedUI) {
					if (currClip != 0)
						clipTextField.text = new string('|', currClip);
					else
						clipTextField.text = " ";
				}

				if (currClip <= 0) {
					Reload();
				}
			}
		}
	}

	void LaunchBullet() {
		if (currProjectile == null)
			return;
		++shootedProjectiles;

		currProjectile.transform.SetParent(null);
		//if (isPlayerWeapon) {
		//	Vector3 mousePos = GameManager.Instance.mouse.mousePosWorld - shootPos[currShootPos].position;
		//	float mouseRot = (Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg + 270) % 360;

		//	if (Mathf.Abs(mouseRot - transform.rotation.eulerAngles.z) <= 60)
		//		currProjectile.transform.rotation = Quaternion.Euler(0, 0, mouseRot);
		//}
		//else if (isAutoTargetPlayer) {
		//	if (GameManager.Instance.player != null)
		//		currProjectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, GameManager.Instance.player.transform.position - transform.position);
		//}
		currProjectile.transform.rotation = transform.rotation;
		currProjectile.Shoot(parentPb.velocity);

		if (++currShootPos >= shootPos.Length)
			currShootPos = 0;

		currProjectile = null;
	}

	public void AppleReloadUpgrade(byte clipAdd, float reloadSpeedChange) {
		clipSize += clipAdd;
		if(reloadTime > reloadSpeedChange)
			reloadTime -= reloadSpeedChange;
		Reload();
	}

	public void AppleSpeedUpgrade(float speed) {
		projectilePrefab.ApplySpeedUpgrade(speed);
	}

	public void AppleDamageUpgrade(float dmg) {
		projectilePrefab.ApplyDamageUpgrade(dmg);
	}
}
