using System;
using UnityEngine;

[Serializable]
public struct CharacterDesc
{
    public string Name;
    public int MaxRange;
    public int MaxLives;
    public int AttackRange;
    public float ShiftDuration;
    public float AttackDuration;
    public float ReturnDuration;
    public float DeathDuration;
}
