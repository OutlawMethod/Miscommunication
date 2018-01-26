using UnityEngine;
using System.Collections.Generic;

struct Node
{
    public Cell Cell;
    public Cell Origin;
    public int Value;
    public bool Visited;
}

class Dijkstra
{
    const int max = 99999;

    private static Node[,] _nodes;
    private static List<Cell> _unvisited = new List<Cell>();

    public static bool HasPath(Cell target)
    {
        return _nodes[target.X, target.Y].Value < max;
    }

    public static Cell[] Path(Cell target)
    {
        var node = _nodes[target.X, target.Y];
        if (node.Value >= max) return null;

        var path = new List<Cell>();

        while (node.Origin != null)
        {
            path.Insert(0, node.Cell);
            node = _nodes[node.Origin.X, node.Origin.Y];
        }

        return path.ToArray();
    }

    public static void Setup(Grid grid, Cell origin, int range)
    {
        if (_nodes == null || _nodes.GetLength(0) != grid.Width || _nodes.GetLength(1) != grid.Height)
            _nodes = new Node[grid.Width, grid.Height];

        for (int x = 0; x < grid.Width; x++)
            for (int y = 0; y < grid.Height; y++)
            {
                Node node;

                node.Cell = grid.Cells[x, y];
                node.Value = max;
                node.Visited = false;
                node.Origin = null;

                _nodes[x, y] = node;

                if (node.Cell.Character == null || node.Cell == origin)
                    _unvisited.Add(node.Cell);
            }

        var current = origin;
        _nodes[current.X, current.Y].Value = 0;

        while (_unvisited.Count > 0 && current != null)
        {
            if (_nodes[current.X, current.Y].Value < range)
            {
                if (current.X > 0) consider(grid.Cells[current.X - 1, current.Y], current);
                if (current.X < grid.Width - 1) consider(grid.Cells[current.X + 1, current.Y], current);
                if (current.Y > 0) consider(grid.Cells[current.X, current.Y - 1], current);
                if (current.Y < grid.Height - 1) consider(grid.Cells[current.X, current.Y + 1], current);
            }

            _nodes[current.X, current.Y].Visited = true;
            _unvisited.Remove(current);

            current = null;
            int minValue = max;

            foreach (var cell in _unvisited)
                if (_nodes[cell.X, cell.Y].Value < minValue)
                {
                    current = cell;
                    minValue = _nodes[cell.X, cell.Y].Value;
                }
        }
    }

    private static void consider(Cell cell, Cell origin)
    {
        var newValue = _nodes[origin.X, origin.Y].Value + 1;

        if (newValue < _nodes[cell.X, cell.Y].Value)
        {
            _nodes[cell.X, cell.Y].Origin = origin;
            _nodes[cell.X, cell.Y].Value = newValue;
        }
    }
}
