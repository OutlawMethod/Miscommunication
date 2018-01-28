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
    public float TurnDuration;
    public string[] Rivals;
    public Projectile Projectile;
    public Sprite Image;
    public string Line1;
    public string Line2;
    public string Line3;
}
