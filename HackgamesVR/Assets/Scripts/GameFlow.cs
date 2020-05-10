using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour {
	public static GameFlow instance;

	[Header("Lore")] [Space]
	[SerializeField] string[] textsMain;
	[SerializeField] AchievmentText[] textsAchievment;

	[Header("Refs")] [Space]
	[SerializeField] TextMeshProUGUI mainText;
	[SerializeField] TextMeshProUGUI achievmentText;
	[SerializeField] PlayerShip player;
	[SerializeField] Weapon playerWeapon;
	[SerializeField] Spawner spawner;
	[SerializeField] Upgrader upgrader;
	bool isWaitDeadButton = false;

	byte openedAchievments = 0;

	private void Awake() {
		instance = this;
	}

	private void Start() {
		LeanTween.delayedCall(0.5f, spawner.StartWave);
	}

	public void OnWinWave() {
		mainText.text = "Волна пройдена!";

		spawner.StartWave();
	}

	public void OnLoseGame(bool isMothershipDestroyed) {
		mainText.text = "You lose!\n\n";
		if (isMothershipDestroyed) {
			mainText.text += "Транспортник знищено, ви наступний";
		}
		else {
			mainText.text += "Ваш корабель знищений і все що залишилося це незкінченно переживати момент своєї смерті";
		}
		mainText.text += "\n\nНатистінь 'A' щоб почати гру заново(персональні апгрейди зберігаються)";

		isWaitDeadButton = true;
		player.isProcessInput = false;
	}

	public void OnWinGame() {
		mainText.text = "You Win!\n\n";
		mainText.text += $"Знайдено пасхальних яєць: {openedAchievments}/{textsAchievment.Length + 2}\n";
		mainText.text += $"Вистреляно лазерів: {playerWeapon.shootedProjectiles}\n";
		mainText.text += $"Пройдено відстані: {player.movedDist}\n";
		mainText.text += $"Вбито ворогів: {spawner.killedEnemies}\n";
		mainText.text += "\n\nНатистінь 'A' щоб почати гру заново(персональні апгрейди зберігаються)";

		isWaitDeadButton = true;
	}

	public void OnPlayerPressButton(int id) {
		if(id == 0 && isWaitDeadButton) {
			Destroy(player.gameObject);
			SceneManager.LoadScene(0);
			return;
		}
	}

	public void UnlockAchievment(int id) {
		++openedAchievments;
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
