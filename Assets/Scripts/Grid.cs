using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject Prefab;
    public int Width = 10;
    public int Height = 10;
    public float Spacing;

    private List<Cell> _cells = new List<Cell>();

    private void Awake()
    {
        var x0 = -Spacing * Width * 0.5f;
        var y0 = -Spacing * Height * 0.5f;

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var instance = GameObject.Instantiate(Prefab);
            }
    }
}
