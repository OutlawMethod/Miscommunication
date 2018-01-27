using System;
using UnityEngine;

[Serializable]
public struct CharacterDesc
{
    public string Name;
    public int MaxRange;
    public int MaxLives;
    public float ShiftDuration;
    public float AttackDuration;
    public float ReturnDuration;
}
