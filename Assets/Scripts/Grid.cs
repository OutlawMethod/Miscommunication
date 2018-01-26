﻿using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Cell Prefab;
    public int Width = 10;
    public int Height = 10;
    public float Spacing = 2;

    public Cell[,] Cells;

    public Cell Hover;

    public List<Cell> Unvisited = new List<Cell>();

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

    const int max = 99999;

    public bool HasPath(Cell target)
    {
        return target.Value < max;
    }

    public Cell[] Path(Cell target)
    {
        if (target.Value >= max) return null;

        var path = new List<Cell>();
        var node = target;

        while (node.Origin != null)
        {
            path.Insert(0, node);
            node = node.Origin;
        }

        return path.ToArray();
    }

    public void Setup(Grid grid, Cell origin, int range)
    {
        for (int x = 0; x < grid.Width; x++)
            for (int y = 0; y < grid.Height; y++)
            {
                var cell = Cells[x, y];

                cell.Value = max;
                cell.Visited = false;
                cell.Origin = null;

                Unvisited.Add(cell);
            }

        var current = origin;
        current.Value = 0;

        while (Unvisited.Count > 0 && current != null)
        {
            if (current.Value < range)
            {
                if (current.X > 0) consider(grid.Cells[current.X - 1, current.Y], current);
                if (current.X < grid.Width - 1) consider(grid.Cells[current.X + 1, current.Y], current);
                if (current.Y > 0) consider(grid.Cells[current.X, current.Y - 1], current);
                if (current.Y < grid.Height - 1) consider(grid.Cells[current.X, current.Y + 1], current);
            }

            current.Visited = true;
            Unvisited.Remove(current);

            current = null;
            int minValue = max;

            foreach (var cell in Unvisited)
                if (cell.Value < minValue)
                {
                    current = cell;
                    minValue = cell.Value;
                }
        }

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (HasPath(Cells[x, y]))
                    Cells[x, y].Status = CellStatus.available;
                else
                    Cells[x, y].Status = CellStatus.default_;
    }

    private void consider(Cell cell, Cell origin)
    {
        if (cell.Character != null)
            return;

        var newValue = origin.Value + 1;

        if (newValue < cell.Value)
        {
            cell.Origin = origin;
            cell.Value = newValue;
        }
    }
}
