using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour {
	public static GameFlow instance;

	[Header("Lore")] [Space]
	[SerializeField] TutorialText[] tutorials;
	[SerializeField] AchievmentText[] textsAchievment;

	[Header("Audio")] [Space]
	[SerializeField] AudioClip buttonClickSound;
	[SerializeField] AudioClip onUnlockAchievment;

	[Header("Refs")] [Space]
	[SerializeField] TextMeshProUGUI mainText;
	[SerializeField] TextMeshProUGUI achievmentText;
	[SerializeField] TextMeshProUGUI objectiveText;
	[SerializeField] TextMeshProUGUI nextWaveText;
	[SerializeField] PlayerShip player;
	[SerializeField] Weapon playerWeapon;
	[SerializeField] Spawner spawner;
	[SerializeField] Upgrader upgrader;
	bool isWaitDeadButton = false;
	bool isWaitForUpgrade = false;

	byte currTutorial = 0;
	byte openedAchievments = 0;

	byte killedAsteroids = 0;
	byte neededAsteroids = 3;

	private void Awake() {
		instance = this;
	}

	private void Start() {
		nextWaveText.text = "  ";
		ProcessTutorial();
	}

	public void OnWinWave() {
		if (isWaitDeadButton)
			return;
		upgrader.Shuffle();

		mainText.text = "Волна пройдена!\n\n";
		mainText.text += "Виберіть апгрейд:\n\n";
		mainText.text += $"(A){upgrader.data[0].name} - {upgrader.data[0].description}\n\n";
		mainText.text += $"(B){upgrader.data[1].name} - {upgrader.data[1].description}\n\n";
		mainText.text += $"(C){upgrader.data[2].name} - {upgrader.data[2].description}\n\n";
		isWaitForUpgrade = true;

		TutorialText t = tutorials[currTutorial];
		objectiveText.text = string.IsNullOrEmpty(t.textObjective) ? "  " : t.textObjective.Replace("\\n", "\n") + spawner.GetProgressStr();

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
		if (isWaitDeadButton)
			return;
		mainText.text = "You Win!\n\n";
		mainText.text += $"Знайдено пасхальних яєць: {openedAchievments}/{textsAchievment.Length + 2}\n";
		mainText.text += $"Вистреляно лазерів: {playerWeapon.shootedProjectiles}\n";
		mainText.text += $"Пройдено відстані: {player.movedDist}\n";
		mainText.text += $"Вбито ворогів: {spawner.killedEnemies}\n";
		mainText.text += "\n\nНатистінь 'A' щоб почати гру заново(персональні апгрейди зберігаються)";

		TutorialText t = tutorials[currTutorial];
		objectiveText.text = string.IsNullOrEmpty(t.textObjective) ? "  " : t.textObjective.Replace("\\n", "\n") + spawner.GetProgressStr();

		isWaitDeadButton = true;
	}

	public void OnPlayerPressButton(int id) {
		if(id == 0 && isWaitDeadButton) {
			Destroy(player.gameObject);
			SceneManager.LoadScene(0);
			return;
		}

		if(id == 0 && currTutorial <= 7) {
			++currTutorial;
			ProcessTutorial();
			AudioManager.Instance.Play(buttonClickSound, player.transform, channel: AudioManager.AudioChannel.Sound);
			return;
		}

		if (isWaitForUpgrade) {
			upgrader.ApplyUpgrade(upgrader.data[id].type);
			mainText.text = "  ";
			isWaitForUpgrade = false;
			AudioManager.Instance.Play(buttonClickSound, player.transform, channel: AudioManager.AudioChannel.Sound);
			return;
		}
	}

	public void UnlockAchievment(int id) {
		AudioManager.Instance.Play(onUnlockAchievment, player.transform, channel: AudioManager.AudioChannel.Sound);
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

	public void OnAsteroidDie() {
		if(currTutorial != 8) {
			if (tutorials[currTutorial].arrowObj != null && tutorials[currTutorial].arrowObj.Length != 0) {
				foreach (var arrow in tutorials[currTutorial].arrowObj)
					arrow.gameObject.SetActive(false);
			}

			for (; currTutorial < 8; ++currTutorial)
				if(tutorials[currTutorial].toEnable)
					tutorials[currTutorial].toEnable.SetActive(true);

			currTutorial = 8;
			ProcessTutorial();
		}

		++killedAsteroids;

		TutorialText t = tutorials[currTutorial];
		objectiveText.text = string.IsNullOrEmpty(t.textObjective) ? "  " : t.textObjective.Replace("\\n", "\n") + $"({killedAsteroids}/{neededAsteroids})";

		if(neededAsteroids == killedAsteroids) {
			LeanTween.delayedCall(1.0f, () => {
				++currTutorial;
				ProcessTutorial();
				DelayedWaveStart(5.0f);
			});
		}
	}

	void ProcessTutorial() {
		if(currTutorial != 0 && tutorials[currTutorial - 1].arrowObj != null && tutorials[currTutorial - 1].arrowObj.Length != 0) {
			foreach (var arrow in tutorials[currTutorial - 1].arrowObj)
				arrow.gameObject.SetActive(false);
		}

		TutorialText t = tutorials[currTutorial];

		mainText.text = string.IsNullOrEmpty(t.textMain) ? "  " : t.textMain.Replace("\\n", "\n");
		objectiveText.text = string.IsNullOrEmpty(t.textObjective) ? "  " : t.textObjective.Replace("\\n", "\n");

		if(currTutorial == 8)
			objectiveText.text += $"({killedAsteroids} / {neededAsteroids})";
		if (currTutorial == 9)
			objectiveText.text += spawner.GetProgressStr();

		if (t.arrowObj != null && t.arrowObj.Length != 0)
			foreach (var arrow in t.arrowObj)
				arrow.gameObject.SetActive(true);
		if(t.toEnable != null)
			t.toEnable.SetActive(true);
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

	[System.Serializable]
	struct TutorialText {
		public string textMain;
		public string textObjective;
		public GameObject[] arrowObj;
		public GameObject toEnable;
	}
}
