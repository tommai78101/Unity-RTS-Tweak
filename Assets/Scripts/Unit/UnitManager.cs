using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour {
	public static readonly int PG_Player = 1;
	public static readonly int PG_Enemy = 2;

	public static List<GameObject> PlayerUnits = new List<GameObject>();
}