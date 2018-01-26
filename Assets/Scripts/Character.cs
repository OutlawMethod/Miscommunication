using UnityEngine;

public class Character : MonoBehaviour
{
    public bool IsActing
    {
        get { return IsMoving || IsAttacking || IsReturning; }
    }

    public int Range = 5;
    public float ShiftDuration = 0.5f;
    public float AttackDuration = 0.25f;
    public float ReturnDuration = 0.5f;

    public int Team;
    public Cell Cell;

    public bool IsMoving;
    public bool IsAttacking;
    public bool IsReturning;

    public Cell[] Path;
    public float Transition;
    public int PathIndex;
    public Vector3 TransitionOrigin;

    public void Attack(Cell[] path)
    {
        Path = path;
        IsMoving = true;
        IsAttacking = true;
        Transition = 0;
        PathIndex = 0;
        TransitionOrigin = transform.position;
    }

    public void Move(Cell[] path)
    {
        Path = path;
        IsMoving = true;
        IsAttacking = false;
        Transition = 0;
        PathIndex = 0;
        TransitionOrigin = transform.position;
    }

    private void Update()
    {
        if (IsMoving)
        {
            Transition += Time.deltaTime / ShiftDuration;

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
                    Transition %= 1;
                    IsMoving = false;
                }
            }
            else
            {
                if (PathIndex >= Path.Length)
                    IsMoving = false;
            }
        }
        else if (IsAttacking)
        {
            Transition += Time.deltaTime / AttackDuration;

            if (Transition >= 1)
            {
                IsAttacking = false;
                IsReturning = true;
                Transition = 0;
                TransitionOrigin = transform.position;
            }

            transform.position = Vector3.Lerp(TransitionOrigin, Path[Path.Length - 1].transform.position, Transition);
        }
        else if (IsReturning)
        {
            Transition += Time.deltaTime / AttackDuration;

            if (Transition >= 1)
                IsReturning = false;

            transform.position = Vector3.Lerp(TransitionOrigin, Cell.transform.position, Transition);
        }
    }
}
