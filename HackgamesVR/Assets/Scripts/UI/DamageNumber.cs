using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour {
	static Camera camera = null;


	readonly Color criticalColor = Color.red;
	readonly Color defaultColor = new Color(0.4f, 0.4f, 0.4f);

	[SerializeField] TextMeshPro damageTextField;

	void Awake() {
		camera = Camera.main;
	}

	private void Update() {
		transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
	}

	public void StartSequence(float damage, bool isCritical) {
		damageTextField.text = Mathf.RoundToInt(damage).ToString();
		damageTextField.color = isCritical ? criticalColor : defaultColor;
		damageTextField.fontSize = Mathf.Lerp(7, 15, damage / 20f);

		damageTextField.transform.localScale = Vector3.one * 0.7f;
		LeanTween.value(gameObject, damageTextField.transform.localScale.x, 1.0f, 0.1f)
			.setOnUpdate((float scale) => {
				damageTextField.transform.localScale = Vector3.one * scale;
			})
			.setOnComplete(() => {
				LeanTween.value(gameObject, damageTextField.transform.localScale.x, 0.7f, 0.3f)
				.setOnUpdate((float scale) => {
					damageTextField.transform.localScale = Vector3.one * scale;
				})
				.setOnComplete(() => {
					LeanTween.value(gameObject, damageTextField.color.a, 0.0f, 0.3f)
					.setOnUpdate((float a) => {
						Color c = damageTextField.color;
						c.a = a;
						damageTextField.color = c;
					})
					.setOnComplete(() => {
						Destroy(gameObject);
					});
				});
			});
	}
}
