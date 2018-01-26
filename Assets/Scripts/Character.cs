using UnityEngine;

public class Character : MonoBehaviour
{
    public int Team;

    public Cell Cell;

    public void Move(Cell destination)
    {
        if (Cell != null)
            Cell.Character = null;

        Cell = destination;
        Cell.Character = this;
        transform.position = Cell.transform.position;
    }
}
