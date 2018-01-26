using UnityEngine;

public class Grid : MonoBehaviour
{
    public Cell Prefab;
    public int Width = 10;
    public int Height = 10;
    public float Spacing = 2;

    public Cell[,] Cells;

    public Cell Hover;

    public void Setup()
    {
        var x0 = -Spacing * Width * 0.5f;
        var y0 = -Spacing * Height * 0.5f;

        Cells = new Cell[Width, Height];

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var instance = GameObject.Instantiate(Prefab.gameObject);
                instance.transform.parent = transform;
                instance.transform.position = new Vector3(x0 + x * Spacing, 0, y0 + y * Spacing);
                instance.name = "Cell " + x.ToString() + ":" + y.ToString();
                instance.SetActive(true);

                var cell = instance.GetComponent<Cell>();
                cell.Grid = this;
                cell.X = x;
                cell.Y = y;

                Cells[x, y] = cell;
            }
    }
}
