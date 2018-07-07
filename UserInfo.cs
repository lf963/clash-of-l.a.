using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo{
	
	public int id;
	public int money;
	public float energy_Athlete_MAX;
	public float energy_Tower_MAX; 
	public float health_USC_MAX;
	public float health_UCLA_MAX;
	public bool[] players;	//which players does the user unlock
	//[0]: student
	//[1]: golf
	//[2]: tennis
	//[3]: basketball
	//[4]: volleyball
	//[5]: football

	public bool[] level_Unlock;	//which level you unlock

	public UserInfo(int id, int money, float energy_Athlete_MAX, float energy_Tower_MAX, float health_USC_MAX, float health_UCLA_MAX, bool[] players, bool[] level_Unlock){
		this.id = id;
		this.money = money;
		this.players = players;
		this.energy_Athlete_MAX = energy_Athlete_MAX;
		this.energy_Tower_MAX = energy_Tower_MAX;
		this.health_USC_MAX = health_USC_MAX;
		this.health_UCLA_MAX = health_UCLA_MAX;
		this.level_Unlock = level_Unlock;
	}

}
