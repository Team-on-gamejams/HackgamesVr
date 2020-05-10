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
	[SerializeField] TextMeshProUGUI nextWaveText;
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
		DelayedWaveStart(5.0f);
	}

	public void OnWinWave() {
		mainText.text = "Волна пройдена!";

		DelayedWaveStart(20.0f);
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

	void DelayedWaveStart(float time) {
		StartCoroutine(Routine());

		IEnumerator Routine() {
			float currTime = time;

			while(currTime >= 0) {
				currTime -= Time.deltaTime;
				nextWaveText.text = $"Next wave in {currTime.ToString("0")} seconds";
				if(currTime <= time / 4) {
					nextWaveText.color = new Color(1, 0, 0, 1);
				}
				yield return null;
			}

			yield return null;
			spawner.StartWave();

			nextWaveText.text = $"  ";
			nextWaveText.color = new Color(1, 0.5803922f, 0, 1);
		}
	}

	[System.Serializable]
	struct AchievmentText {
		public string textTitle;
		public string name;
		public string description;
	}
}
