﻿using UnityEngine;

public class Character : Actor
{
    public override bool IsActing
    {
        get { return IsMoving || IsAttacking || IsReturning || IsDying || IsTurning || IsStaying; }
    }

    public CharacterDesc Desc;
    public Transform Origin;

    public int Range;
    public int Lives;

    public int Team;
    public Manager Manager;
    public Cell Cell;
    public HeartPanel Hearts;
    public HeartCount HeartCount;

    public bool IsMoving;
    public bool IsAttacking;
    public bool IsReturning;
    public bool IsDying;
    public bool IsTurning;
    public bool IsStaying;

    public Cell[] Path;
    public float Transition;
    public int PathIndex;
    public Vector3 TransitionOrigin;
    public float TurnOrigin;
    public float TurnTarget;
    public float TurnTransition;

    public bool HasFired;

    public AudioSource Audio;
    public AudioClip AttackSound;
    public AudioClip HitSound;

    private void Awake()
    {
        Audio = GetComponent<AudioSource>();
        Audio.volume = 0;
    }

    public int Control
    {
        get
        {
            var y = Cell.Y;
            var dir = Team == 0 ? -1 : 1;
            var value = 100;
            y += dir;
            var current = this;

            while (y >= 0 && y < Manager.Grid.Height)
            {
                var cell = Manager.Grid.Cells[Cell.X, y];

                if (cell.Character != null)
                {
                    if (cell.Character.Team != Team)
                        return 0;
                    else
                    {
                        value -= current.IsRivalsWith(cell.Character) ? 50 : 30;
                        current = cell.Character;
                    }
                }

                y += dir;
            }

            if (value < 0)
                return 0;

            return value;
        }
    }

    public bool IsAvailable
    {
        get
        {
            var y = Cell.Y;

            while (y >= 0 && y < Manager.Grid.Height)
            {
                var cell = Manager.Grid.Cells[Cell.X, y];

                if (cell.Character != null && cell.Character.Team != Team)
                    return false;

                if (Team == 0)
                    y--;
                else
                    y++;
            }

            return true;
        }
    }

    public bool IsRivalsWith(Character other)
    {
        foreach (var name in Desc.Rivals)
            if (name == other.Desc.Name)
                return true;

        return false;
    }

    public void Die()
    {
        IsDying = true;
        TransitionOrigin = transform.position;
        Manager.Process(this);
    }

    public void Turn(float target)
    {
        var current = transform.eulerAngles.y;

        if (Mathf.Abs(Mathf.DeltaAngle(current, target)) < 1)
            return;

        IsTurning = true;
        TurnOrigin = current;
        TurnTarget = target;
        TurnTransition = 0;
    }

    public void Stay()
    {
        IsStaying = true;
        Transition = 0;
        TransitionOrigin = transform.position;
        Range = 0;
        Manager.Process(this);
    }

    public void Attack(Cell[] path)
    {
        HasFired = false;
        Range = 0;
        Path = path;
        IsMoving = path.Length > 1;
        IsAttacking = true;
        Transition = 0;
        PathIndex = 0;
        TransitionOrigin = transform.position;
        prepareToMoveTo(path[0]);
        Manager.Process(this);

        if (Path.Length > 1)
        {
            var clip = Audio.clip;
            Destroy(Audio);
            Audio = gameObject.AddComponent<AudioSource>();
            Audio.clip = clip;
            Audio.Play();
        }
    }

    public void Move(Cell[] path)
    {
        Range -= path.Length;
        Path = path;
        IsMoving = true;
        IsAttacking = false;
        Transition = 0;
        PathIndex = 0;
        TransitionOrigin = transform.position;
        prepareToMoveTo(path[0]);
        Manager.Process(this);
        var clip = Audio.clip;
        Destroy(Audio);
        Audio = gameObject.AddComponent<AudioSource>();
        Audio.clip = clip;
        Audio.Play();
    }

    private void prepareToMoveTo(Cell next)
    {
        if (next.X > Cell.X)
            Turn(90);
        else if (next.X < Cell.X)
            Turn(-90);
        else if (next.Y > Cell.Y)
            Turn(0);
        else if (next.Y < Cell.Y)
            Turn(180);
    }

    public void Play(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    private void Update()
    {
        if (IsDying)
        {
            Audio.volume = 0;
            Transition += Time.deltaTime / Desc.DeathDuration;

            if (Transition >= 1)
            {
                Transition = 1;
                IsDying = false;
            }

            transform.position = Fluid.Lerp(TransitionOrigin, TransitionOrigin - Vector3.up * 1.5f, Transition, AnimationMode.easeIn);
        }
        else if (IsStaying)
        {
            Audio.volume = 0;
            Transition += Time.deltaTime * 5;

            if (Transition >= 1)
                IsStaying = false;
        }
        else
        {
            if (IsTurning)
            {
                Audio.volume = Mathf.Clamp01(Audio.volume - Time.deltaTime * 3);

                TurnTransition += Time.deltaTime / Desc.TurnDuration;

                if (TurnTransition >= 1)
                {
                    TurnTransition = 1;
                    IsTurning = false;
                }

                transform.eulerAngles = new Vector3(transform.eulerAngles.x, Fluid.LerpAngle(TurnOrigin, TurnTarget, TurnTransition, AnimationMode.easeInOut), transform.eulerAngles.z);
            }
            else if (IsMoving)
            {
                Audio.volume = 1;
                var segment = Cell.SegmentLength(Path, PathIndex);

                if (IsAttacking && PathIndex + segment >= Path.Length)
                    segment--;

                Transition += Time.deltaTime / (segment * Desc.ShiftDuration);

                while (Transition >= 1 && PathIndex + segment - 1 < Path.Length)
                {
                    if (Cell != null)
                        Cell.Character = null;

                    Cell = Path[PathIndex + segment - 1];
                    Cell.Character = this;

                    Transition -= 1;
                    TransitionOrigin = transform.position;
                    PathIndex += segment;

                    if (PathIndex < Path.Length)
                        prepareToMoveTo(Path[PathIndex]);

                    segment = 1;
                }

                if (PathIndex + segment - 1 < Path.Length)
                    transform.position = Fluid.Lerp(TransitionOrigin, Path[PathIndex + segment - 1].transform.position, Transition, AnimationMode.easeInOut);

                if (IsAttacking)
                {
                    if (PathIndex + 1 >= Path.Length)
                    {
                        TransitionOrigin = transform.position;
                        Transition = 0;
                        IsMoving = false;
                        prepareToMoveTo(Path[Path.Length - 1]);
                    }
                }
                else
                {
                    if (PathIndex >= Path.Length)
                    {
                        Transition = 0;
                        IsMoving = false;
                    }
                }
            }
            else if (IsAttacking)
            {
                Audio.volume = 0;

                if (Desc.Projectile != null && !HasFired)
                {
                    HasFired = true;
                    Play(AttackSound);

                    var instance = GameObject.Instantiate(Desc.Projectile.gameObject);
                    instance.transform.position = Origin.position;
                    instance.SetActive(true);

                    var projectile = instance.GetComponent<Projectile>();
                    projectile.Origin = instance.transform.position;
                    projectile.Enemy = Path[Path.Length - 1].Character;
                    projectile.Target = projectile.Enemy.transform.position;
                    projectile.Damage = IsRivalsWith(projectile.Enemy) ? 4 : 2;

                    Manager.Process(projectile);
                }
                else if (!HasFired)
                {
                    HasFired = true;
                    Play(AttackSound);
                }

                Transition += Time.deltaTime / Desc.AttackDuration;

                if (Transition >= 1)
                {
                    if (Desc.Projectile == null)
                    {
                        var enemy = Path[Path.Length - 1].Character;

                        if (enemy != null)
                        {
                            enemy.Play(enemy.HitSound);
                            enemy.Lives -= IsRivalsWith(enemy) ? 4 : 2;

                            if (enemy.Lives <= 0)
                                enemy.Die();
                        }
                    }

                    IsAttacking = false;
                    IsReturning = true;
                    Transition = 0;
                    TransitionOrigin = transform.position;
                }

                var target = Path[Path.Length - 1];

                if (target.X > Cell.X + 1) target = Manager.Grid.Cells[Cell.X + 1, Cell.Y];
                if (target.X < Cell.X - 1) target = Manager.Grid.Cells[Cell.X - 1, Cell.Y];
                if (target.Y > Cell.Y + 1) target = Manager.Grid.Cells[Cell.X, Cell.Y + 1];
                if (target.Y < Cell.Y - 1) target = Manager.Grid.Cells[Cell.X, Cell.Y - 1];

                var targetPosition = target.transform.position;

                if (Desc.Projectile != null)
                    targetPosition = TransitionOrigin - (target.transform.position - TransitionOrigin);

                transform.position = Fluid.Lerp(TransitionOrigin, targetPosition, Transition, AnimationMode.easeIn);
            }
            else if (IsReturning)
            {
                Audio.volume = 0;
                Transition += Time.deltaTime / Desc.ReturnDuration;

                if (Transition >= 1)
                    IsReturning = false;

                transform.position = Fluid.Lerp(TransitionOrigin, Cell.transform.position, Transition, AnimationMode.easeOut);
            }
            else
                Audio.volume = 0;
        }
    }
}
