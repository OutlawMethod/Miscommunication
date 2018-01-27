using UnityEngine;

public class Character : MonoBehaviour
{
    public bool IsActing
    {
        get { return IsMoving || IsAttacking || IsReturning || IsDying || IsTurning; }
    }

    public CharacterDesc Desc;

    public int Range;
    public int Lives;

    public int Team;
    public Manager Manager;
    public Cell Cell;

    public bool IsMoving;
    public bool IsAttacking;
    public bool IsReturning;
    public bool IsDying;
    public bool IsTurning;

    public Cell[] Path;
    public float Transition;
    public int PathIndex;
    public Vector3 TransitionOrigin;
    public float TurnOrigin;
    public float TurnTarget;
    public float TurnTransition;

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

    public void Attack(Cell[] path)
    {
        Range = 0;
        Path = path;
        IsMoving = true;
        IsAttacking = true;
        Transition = 0;
        PathIndex = 0;
        TransitionOrigin = transform.position;
        prepareToMoveTo(path[0]);
        Manager.Process(this);
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

    private void Update()
    {
        if (IsDying)
        {
            Transition += Time.deltaTime / Desc.DeathDuration;

            if (Transition >= 1)
            {
                Transition = 1;
                IsDying = false;
            }

            transform.position = Vector3.Lerp(TransitionOrigin, TransitionOrigin - Vector3.up * 1.5f, Transition);
        }
        else if (IsTurning)
        {
            TurnTransition += Time.deltaTime / Desc.TurnDuration;

            if (TurnTransition >= 1)
            {
                TurnTransition = 1;
                IsTurning = false;
            }

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.LerpAngle(TurnOrigin, TurnTarget, TurnTransition), transform.eulerAngles.z);
        }
        else if (IsMoving)
        {
            Transition += Time.deltaTime / Desc.ShiftDuration;

            while (Transition >= 1 && PathIndex < Path.Length)
            {
                if (Cell != null)
                    Cell.Character = null;

                Cell = Path[PathIndex];
                Cell.Character = this;

                Transition -= 1;
                TransitionOrigin = transform.position;
                PathIndex++;

                if (PathIndex < Path.Length)
                    prepareToMoveTo(Path[PathIndex]);
            }

            if (PathIndex < Path.Length)
                transform.position = Vector3.Lerp(TransitionOrigin, Path[PathIndex].transform.position, Transition);

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
            Transition += Time.deltaTime / Desc.AttackDuration;

            if (Transition >= 1)
            {
                var enemy = Path[Path.Length - 1].Character;

                if (enemy != null)
                {
                    enemy.Lives -= 4;

                    if (enemy.Lives <= 0)
                        enemy.Die();
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

            transform.position = Vector3.Lerp(TransitionOrigin, target.transform.position, Transition);
        }
        else if (IsReturning)
        {
            Transition += Time.deltaTime / Desc.ReturnDuration;

            if (Transition >= 1)
                IsReturning = false;

            transform.position = Vector3.Lerp(TransitionOrigin, Cell.transform.position, Transition);
        }
    }
}
