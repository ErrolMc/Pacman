using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuTools
{
	[MenuItem("Tools/Nuke Player Prefs")]
	static void DeletePlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
		Debug.Log("Nuking prefs");
	}
}
