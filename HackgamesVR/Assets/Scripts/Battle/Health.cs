using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviour {
	static Camera camera = null;

	[SerializeField] float maxHp = 100;
	float currHp;

	[Space]
	[Header("UI")]
	[SerializeField] bool needShowOnInit = false;
	[SerializeField] bool needRotateSlider = false;
	[SerializeField] Transform sliderParent = null;
	[SerializeField] Slider bar = null;
	[SerializeField] TextMeshProUGUI hpTextField = null;

	[Space]
	[SerializeField] GameObject damageNumberPrefab = null;  //TODO: poll this instead of spawning

#if UNITY_EDITOR
	private void OnValidate() {
		if (bar == null) {
			sliderParent = GetComponentInChildren<Canvas>()?.transform;
			bar = GetComponentInChildren<Slider>();
			if (bar != null)
				needRotateSlider = true;
		}
	}
#endif

	void Awake() {
		camera = Camera.main;
		currHp = maxHp;
	}

	void Start() {
		if (bar != null) {
			bar.gameObject.SetActive(needShowOnInit);
			bar.minValue = 0.0f;
			bar.maxValue = maxHp;
			UpdateHpBar();
		}
		if (sliderParent == null)
			needRotateSlider = false;
		if (hpTextField)
			hpTextField.text = $"{Mathf.RoundToInt(currHp)}/{Mathf.RoundToInt(maxHp)}";
	}

	void Update() {
		if (needRotateSlider && bar.gameObject.activeSelf) {
			sliderParent.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
		}
	}

	public void ShowBar() {
		if (bar != null)
			bar.gameObject.SetActive(true);
	}

	public void HideBar() {
		if (bar != null)
			bar.gameObject.SetActive(false);
	}

	public void OnCollideWithProjectile(Projectile projectile) {
		if (currHp == 0)
			return;

		float takenDmg = projectile.Damage;
		currHp -= takenDmg;

		ShowDamageNumber(takenDmg, projectile.transform.position);

		if (!bar.gameObject.activeSelf)
			ShowBar();
		if (bar != null)
			UpdateHpBar();

		if (currHp <= 0) {
			currHp = 0;
			SendMessage("Die");
		}

		if (hpTextField)
			hpTextField.text = $"{Mathf.RoundToInt(currHp)}/{Mathf.RoundToInt(maxHp)}";
	}

	GameObject ShowDamageNumber(float damage, Vector3 pos) {
		//GameObject damageNumber = Instantiate(damageNumberPrefab, pos, Quaternion.identity, parent);

		//damageNumber.GetComponent<DamageNumber>().StartSequence(damage, isCrit);

		return /*damageNumber*/null;
	}

	void UpdateHpBar() {
		bar.value = currHp;
	}
}
