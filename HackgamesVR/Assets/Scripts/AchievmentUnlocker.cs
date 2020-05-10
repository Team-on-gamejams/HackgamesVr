using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievmentUnlocker : MonoBehaviour {
	[SerializeField] int id = 0;

	bool isUnlocked = false;
	public void Unlock() {
		if (isUnlocked)
			return;
		isUnlocked = true;
		GameFlow.instance.UnlockAchievment(id);
	}
}
