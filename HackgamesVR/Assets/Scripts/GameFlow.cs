using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameFlow : MonoBehaviour {
	public static GameFlow instance;

	[Header("Lore")] [Space]
	[SerializeField] string[] textsMain;
	[SerializeField] AchievmentText[] textsAchievment;

	[Header("Refs")] [Space]
	[SerializeField] TextMeshProUGUI mainText;
	[SerializeField] TextMeshProUGUI achievmentText;
	[SerializeField] Spawner spawner;

	private void Awake() {
		instance = this;
	}

	private void Start() {
		LeanTween.delayedCall(0.5f, spawner.StartWave);
	}

	public void UnlockAchievment(int id) {
		AchievmentText t = textsAchievment[id];

		achievmentText.alpha = 0.0f;
		LeanTweenEx.ChangeTextAlpha(achievmentText, 1.0f, 0.5f);
		achievmentText.text = $"{t.textTitle}\n{t.name}\n{t.description}";
		LeanTweenEx.ChangeTextAlpha(achievmentText, 0.0f, 0.5f)
			.setDelay(5.0f)
			.setOnComplete(()=> {
				achievmentText.text = "  ";
			});
	}

	[System.Serializable]
	struct AchievmentText {
		public string textTitle;
		public string name;
		public string description;
	}
}
