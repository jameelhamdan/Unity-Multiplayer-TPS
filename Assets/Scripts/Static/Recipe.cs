using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Recipe", menuName = "ScriptableObjects/New Recipe")]
public class Recipe : ScriptableObject
{
	public int ID;
	public int[] RequiredItemIds;
	public int[] RequiredItemQuantaties;
	public int ResultID;
	public int ResultQuantity;
}
