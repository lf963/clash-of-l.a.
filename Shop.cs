using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shop : MonoBehaviour {
	//Manager for background music and attack sounds
	private MusicManager musicManager;

	//the amount of money shown in shop
	[SerializeField]
	private Text moneyText;

	//show athlete's price in shop
	[SerializeField]
	private Text[] athletePriceInShop;



	//show the price of upgrading athlete energy bar in shop
	[SerializeField]
	private Text athleteEnergyPriceInShop;


	//show the price of upgrading tower energy bar in shop
	[SerializeField]
	private Text towerEnergyPriceInShop;



	//show the current value of athlete energy bar in shop
	[SerializeField]
	private Text athleteCurrentValInShop;

	//show the next value of athlete energy bar in shop
	[SerializeField]
	private Text athleteNextValInShop;

	//show the current value of tower energy bar in shop
	[SerializeField]
	private Text towerCurrentValInShop;

	//show the next value of tower energy bar in shop
	[SerializeField]
	private Text towerNextValInShop;






	//show the price of upgrading nexus in shop
	[SerializeField]
	private Text nexusUpgradePriceInShop;

	//show the current level of nexus in shop
	[SerializeField]
	private Text nexusCurrentLevelInShop;

	//show the next level of nexus in shop
	[SerializeField]
	private Text nexusNextLevelInShop;

	//show the current HP of nexus in shop
	[SerializeField]
	private Text nexusCurrentHPInShop;

	//show the next HP of nexus if upgrading in shop
	[SerializeField]
	private Text nexusNextHPInShop;

	//decide whether "Buy Athlete" button show or not
	[SerializeField]
	private GameObject[] athleteBuyBtn;

	//decide whether "Athlete Description" text show or not
	[SerializeField]
	private Text[] athleteDescription;

	//decide whether "Upgrade Nexus" button show or not
	[SerializeField]
	private GameObject nexusUpgradeBtn;

	//decide whether "Upgrade Athlete Energy" button show or not
	[SerializeField]
	private GameObject athleteUpgradeBtn;

	//decide whether "Upgrade Tower Energy" button show or not
	[SerializeField]
	private GameObject towerUpgradeBtn;

	//our custom athlete price
	//[0]: student
	//[1]: golf
	//[2]: tennis
	//[3]: basketball
	//[4]: volleyball
	//[5]: football
	private int[] athletePrices = {10,20,30,40,50,60};

	//value of enegry bar
	private int athleteCurrentVal;
	private int towerCurrentVal;
	private int athleteEnergyUpgradePrice;
	private int towerEnergyUpgradePrice;

	//Level, HP and price of upgrade of our nexus
	private int nexusCurrentLevel;
	private int nexusCurrentHP;
	private int nexusUpgradePrice;

	//increase this amount of HP when upgrading nexus
	private int nexusHpIncrement = 10;

	//increase this amount of price when upgrading nexus
	private int nexusPriceIncrement = 100;

	//increase this amount of energy when upgrading athlete energy
	private int athleteEnergyIncrement = 10;

	//increase this amount of price when upgrading athlete
	private int athletePriceIncrement = 100;

	//increase this amount of energy when upgrading tower energy
	private int towerEnergyIncrement = 10;

	//increase this amount of price when upgrading tower
	private int towerPriceIncrement = 100;

	private int currency;
	public int Currency {
		get { return currency; }
		set { 
			this.currency = value;
			this.moneyText.text = value.ToString();
		}
	}

	// Use this for initialization
	//If it's the first time to play this game, default value of nexus HP, athlete energy and tower energy are all set to 100
	void Start () {
		musicManager = FindObjectOfType<MusicManager> ();
		musicManager.bgMusic.Play ();

		Currency = LevelManager.myPlayer.money;

		//HP=100: Lv1, HP=110: Lv2, HP=150: Lv6
		nexusCurrentLevel = (int)((LevelManager.myPlayer.health_USC_MAX - 100) / nexusHpIncrement + 1);
		nexusCurrentHP = (int)LevelManager.myPlayer.health_USC_MAX;
		nexusUpgradePrice = nexusCurrentLevel * nexusPriceIncrement;

		athleteCurrentVal = (int)(LevelManager.myPlayer.energy_Athlete_MAX);
		towerCurrentVal = (int)(LevelManager.myPlayer.energy_Tower_MAX);


		//energy amount = 100, need 100$ to upgrade
		//energy amount = 110, need 200$ to upgrade
		//energy amount = 120, need 300$ to upgrade
		athleteEnergyUpgradePrice = (int)((athleteCurrentVal - 100) / athleteEnergyIncrement * athletePriceIncrement + 100);
		towerEnergyUpgradePrice = (int)((towerCurrentVal - 100) / towerEnergyIncrement * towerPriceIncrement + 100);

		setAthletePriceAndButton ();
		setNexusPriceAndButton ();
		setEnergyPriceAndButton ();

	}


	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.KeypadPlus)) {
			Currency += 1;
			LevelManager.myPlayer.money += 1;
			setAthletePriceAndButton ();
		}
		if (Input.GetKeyDown (KeyCode.KeypadMinus)) {
			LevelManager.myPlayer.money -= 1;
			Currency -= 1;
			setAthletePriceAndButton();
		}
	
	}

	//set Energy price
	void setEnergyPriceAndButton(){
		if (Currency >= athleteEnergyUpgradePrice) 
			athleteUpgradeBtn.gameObject.GetComponent<Button> ().interactable = true;
		else
			athleteUpgradeBtn.gameObject.GetComponent<Button> ().interactable = false;

		if (Currency >= towerEnergyUpgradePrice) 
			towerUpgradeBtn.gameObject.GetComponent<Button> ().interactable = true;
		else
			towerUpgradeBtn.gameObject.GetComponent<Button> ().interactable = false;

		athleteCurrentValInShop.text = athleteCurrentVal.ToString ();
		athleteNextValInShop.text = (athleteEnergyIncrement + athleteCurrentVal).ToString ();

		towerCurrentValInShop.text = towerCurrentVal.ToString ();
		towerNextValInShop.text = (towerEnergyIncrement + towerCurrentVal).ToString ();

		athleteEnergyPriceInShop.text = athleteEnergyUpgradePrice.ToString();
		towerEnergyPriceInShop.text = towerEnergyUpgradePrice.ToString();

	}

	//set Nexus price
	void setNexusPriceAndButton(){
		if (Currency >= nexusUpgradePrice) 
			nexusUpgradeBtn.gameObject.GetComponent<Button> ().interactable = true;
		else
			nexusUpgradeBtn.gameObject.GetComponent<Button> ().interactable = false;
		nexusUpgradePriceInShop.text = nexusUpgradePrice.ToString();
		nexusCurrentLevelInShop.text = nexusCurrentLevel.ToString();
		nexusNextLevelInShop.text = (1 + nexusCurrentLevel).ToString();
		nexusCurrentHPInShop.text = nexusCurrentHP.ToString();
		nexusNextHPInShop.text = (nexusHpIncrement + nexusCurrentHP).ToString();
	}

	//set athlete price
	void setAthletePriceAndButton(){
		//if LevelManager.myPlayer.players [i] == true, that means we already have that athlete
		//so we don't show "Buy Athlete" button and let his price be "Purchased"
		for (int i = 0; i < LevelManager.myPlayer.players.Length; i++) {
			if (LevelManager.myPlayer.players [i]){	//already have athlete
				athleteBuyBtn [i].gameObject.SetActive(false);
				athleteDescription [i].gameObject.SetActive (true);
				athletePriceInShop [i].text = "Purchased";
			}
			else{	//don't have this athlete
				athleteBuyBtn [i].gameObject.SetActive(true);
				athleteDescription [i].gameObject.SetActive (false);
				athletePriceInShop [i].text = athletePrices[i].ToString();
				if (Currency >= athletePrices [i]) {	//if we have enough money
					athleteBuyBtn [i].gameObject.GetComponent<Button> ().interactable = true;
				}
				else {	//if we don't have enough money
					athleteBuyBtn [i].gameObject.GetComponent<Button> ().interactable = false;
				}
			}
		}
	}

	//energyIndex = 0: upgrade athlete energy
	//energyIndex = 1: upgrade tower energy
	public void upgradeEnergy(int energyIndex){
		if (energyIndex == 0) {
			Currency -= athleteEnergyUpgradePrice;
			LevelManager.myPlayer.money -= athleteEnergyUpgradePrice;
			athleteCurrentVal += athleteEnergyIncrement;
			LevelManager.myPlayer.energy_Athlete_MAX += athleteEnergyIncrement;
			athleteEnergyUpgradePrice += athletePriceIncrement;
		}
		else {
			Currency -= towerEnergyUpgradePrice;
			LevelManager.myPlayer.money -= towerEnergyUpgradePrice;
			towerCurrentVal += towerEnergyIncrement;
			LevelManager.myPlayer.energy_Tower_MAX += towerEnergyIncrement;
			towerEnergyUpgradePrice += towerPriceIncrement;
		}
		setEnergyPriceAndButton ();
		setAthletePriceAndButton ();
		setNexusPriceAndButton ();
	}

	public void upgradeNexus(){
		Currency -= nexusUpgradePrice;
		LevelManager.myPlayer.money -= nexusUpgradePrice;

		nexusCurrentHP += nexusHpIncrement;
		LevelManager.myPlayer.health_USC_MAX += nexusHpIncrement;

		nexusCurrentLevel += 1;

		nexusUpgradePrice += nexusPriceIncrement;

		setEnergyPriceAndButton ();
		setAthletePriceAndButton ();
		setNexusPriceAndButton ();
	}

	//Once clicking "Buy Athlete" button, run this function
	public void buyAthlete(int athleteIndex){
		//LevelManager.myPlayer.money -= athletePrices [athleteIndex];
		Currency -= athletePrices [athleteIndex];
		LevelManager.myPlayer.money -= athletePrices [athleteIndex];
		LevelManager.myPlayer.players [athleteIndex] = true;

		setEnergyPriceAndButton ();
		setAthletePriceAndButton ();
		setNexusPriceAndButton ();
	}

	void OnApplicationQuit()
	{
		Debug.Log("Application ending after " + Time.time + " seconds");
		SaveLoad.Save();
	}

}
