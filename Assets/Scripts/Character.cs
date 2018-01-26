using UnityEngine;

public class Character : MonoBehaviour
{
    public int Range = 5;
    public float ShiftDuration = 0.5f;

    public int Team;
    public Cell Cell;

    public bool IsMoving;
    public Cell[] Path;
    public float Transition;
    public int PathIndex;
    public Vector3 TransitionOrigin;

    public void Move(Cell[] path)
    {
        Path = path;
        IsMoving = true;
        Transition = 0;
        PathIndex = 0;
        TransitionOrigin = transform.position;
    }

    private void Update()
    {
        if (!IsMoving)
            return;

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

        if (PathIndex >= Path.Length)
        {
            IsMoving = false;
            return;
        }

        transform.position = Vector3.Lerp(TransitionOrigin, Path[PathIndex].transform.position, Transition);
    }
}
