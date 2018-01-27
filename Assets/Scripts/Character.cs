using UnityEngine;

public class Character : MonoBehaviour
{
    public bool IsActing
    {
        get { return IsMoving || IsAttacking || IsReturning || IsDying; }
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

    public Cell[] Path;
    public float Transition;
    public int PathIndex;
    public Vector3 TransitionOrigin;

    public void Die()
    {
        IsDying = true;
        TransitionOrigin = transform.position;
        Manager.Process(this);
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
        Manager.Process(this);
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
                var target = Path[Path.Length - 1].Character;

                if (target != null)
                {
                    target.Lives -= 4;

                    if (target.Lives <= 0)
                        target.Die();
                }

                IsAttacking = false;
                IsReturning = true;
                Transition = 0;
                TransitionOrigin = transform.position;
            }

            transform.position = Vector3.Lerp(TransitionOrigin, Path[Path.Length - 1].transform.position, Transition);
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
