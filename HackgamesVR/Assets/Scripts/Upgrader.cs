using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrader : MonoBehaviour {
	public enum UpgradeType : byte { None, Engine, WeaponDmg, WeaponSpeed, WeaponReload, PlayerHp, MothershipHp, SecretOpener, LastUp}

	[Header("Data")] [Space]
	public UpgradeData[] data;

	[Header("refs")] [Space]
	[SerializeField] PlayerShip playerShip;
	[SerializeField] Weapon playerWeapon;
	[SerializeField] Health playerHp;
	[Space]
	[SerializeField] Health motherShipHp;

	public void Shuffle() {
		data.Shuffle();
	}

	public void ApplyUpgrade(UpgradeType type) {
		switch (type) {
			case UpgradeType.Engine:
				playerShip.ApplyEngineUpgrade(20.0f);
				break;
			case UpgradeType.WeaponDmg:
				playerWeapon.AppleDamageUpgrade(5);
				break;
			case UpgradeType.WeaponSpeed:
				playerWeapon.AppleSpeedUpgrade(20);
				break;
			case UpgradeType.WeaponReload:
				playerWeapon.AppleReloadUpgrade(10, 1.0f);
				break;
			case UpgradeType.PlayerHp:
				playerHp.ApplyHpUpgrade(100.0f);
				break;
			case UpgradeType.MothershipHp:
				motherShipHp.ApplyHpUpgrade(1000.0f);
				break;
			case UpgradeType.SecretOpener:
				GameFlow.instance.UnlockAchievment(2);
				break;
		}
	}
}

[System.Serializable]
public struct UpgradeData {
	public Upgrader.UpgradeType type;
	public string name;
	public string description;
}
