using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName ="New Item",menuName = "ScriptableObjects/New Item")]
public class Item : ScriptableObject
{
    public long ID;
    public string Name; 
    public float Value;
	public int Quantity;
    public Material Texture;
    public int StackMax;

}
