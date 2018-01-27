using System.Collections.Generic;
using UnityEngine;

public enum CellStatus
{
    default_,
    available,
    enemy
}

public class Cell : MonoBehaviour
{
    public Character Character;

    public CellStatus Status;

    public GameGrid Grid;
    public int X;
    public int Y;

    public Material Material;

    public Cell Origin;
    public int Value;
    public bool Visited;

    public List<Cell> Temp = new List<Cell>();

    private void Awake()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null) renderer = GetComponentInChildren<Renderer>();

        Material = Material.Instantiate(renderer.material);
        renderer.material = Material;
    }

    public Cell Neighbour(int x, int y)
    {
        x += X;
        y += Y;

        if (x < 0 || x >= Grid.Width ||
            y < 0 || y >= Grid.Height)
            return null;

        return Grid.Cells[x, y];
    }

    private void considerAttackOrigin(Cell cell, int range)
    {
        if (cell == null)
            return;

        if (!Grid.HasPath(cell))
            return;

        if (cell.Value + 1 >= range)
            return;

        Temp.Add(cell);
    }

    public Cell AttackOrigin(int range)
    {
        Temp.Clear();

        considerAttackOrigin(Neighbour(-1, 0), range);
        considerAttackOrigin(Neighbour(1, 0), range);
        considerAttackOrigin(Neighbour(0, -1), range);
        considerAttackOrigin(Neighbour(0, 1), range);

        if (Temp.Count == 0)
            return Origin;

        var mouse = Input.mousePosition;
        var v0 = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 0));
        var v1 = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 1));

        RaycastHit hit;
        if (Physics.Raycast(v0, (v1 - v0).normalized, out hit))
        {
            float minDist = 0;
            Cell closest = null;

            foreach (var cell in Temp)
            {
                var dist = Vector3.Distance(cell.transform.position, hit.point);

                if (dist < minDist || closest == null)
                {
                    closest = cell;
                    minDist = dist;
                }
            }

            return closest;
        }
        else
            return Origin;
    }

    private void Update()
    {
        switch (Status)
        {
            case CellStatus.default_:
                if (Grid.Hover != null && Grid.HoverTeam >= 0)
                {
                    if (Grid.Hover == this && Character != null && Character.Team == Grid.HoverTeam)
                        Material.color = Color.yellow;
                    else if (Grid.Hover.X == X)
                    {
                        var inPlace = false;

                        if (Grid.HoverTeam == 0)
                        {
                            if (Y < Grid.Hover.Y)
                                inPlace = true;
                        }
                        else if (Grid.HoverTeam == 1)
                        {
                            if (Y > Grid.Hover.Y)
                                inPlace = true;
                        }

                        if (inPlace)
                            Material.color = Color.blue;
                        else
                            Material.color = Color.white;
                    }
                    else
                        Material.color = Color.white;
                }
                else
                    Material.color = Color.white;
                break;

            case CellStatus.available:
                {
                    var inPath = false;

                    var node = Grid.Hover;
                    Cell attackOrigin = null;

                    if (Grid.Hover != null && Grid.Hover.Status == CellStatus.enemy)
                    {
                        attackOrigin = Grid.Hover.AttackOrigin(Grid.HoverRange);
                        node = attackOrigin;
                    }

                    while (node != null)
                    {
                        if (node == this)
                        {
                            inPath = true;
                            break;
                        }
                        else
                            node = node.Origin;
                    }

                    if (inPath)
                        Material.color = Color.yellow;
                    else
                        Material.color = Color.green;
                }
                break;

            case CellStatus.enemy:
                if (Grid.Hover == this)
                    Material.color = Color.Lerp(Color.red, Color.white, 0.5f);
                else
                    Material.color = Color.red;
                break;
        }
    }

    private void OnMouseOver()
    {
        Grid.Hover = this;
    }

    private void OnMouseExit()
    {
        if (Grid.Hover == this)
            Grid.Hover = null;
    }
}
