﻿using System.Collections.Generic;
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

    public float Blink;
    public int BlinkDirection;

    public List<Cell> Temp = new List<Cell>();

    private void Awake()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null) renderer = GetComponentInChildren<Renderer>();

        Material = Material.Instantiate(renderer.material);
        renderer.material = Material;
    }

    public int SegmentLength(Cell[] Path, int index)
    {
        if (index >= Path.Length)
            return 0;

        if (Path[index].X > X) return 1 + Path[index].SegmentLength(Path, index + 1, 1);
        if (Path[index].X < X) return 1 + Path[index].SegmentLength(Path, index + 1, 2);
        if (Path[index].Y > Y) return 1 + Path[index].SegmentLength(Path, index + 1, 3);
        if (Path[index].Y < Y) return 1 + Path[index].SegmentLength(Path, index + 1, 4);

        return 0;
    }

    public int SegmentLength(Cell[] Path, int index, int direction)
    {
        if (index >= Path.Length)
            return 0;

        if (direction == 1 && Path[index].X > X) return 1 + Path[index].SegmentLength(Path, index + 1, 1);
        if (direction == 2 && Path[index].X < X) return 1 + Path[index].SegmentLength(Path, index + 1, 2);
        if (direction == 3 && Path[index].Y > Y) return 1 + Path[index].SegmentLength(Path, index + 1, 3);
        if (direction == 4 && Path[index].Y < Y) return 1 + Path[index].SegmentLength(Path, index + 1, 4);

        return 0;
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

    private void considerAttackOrigin(Cell cell, Character attacker)
    {
        if (cell == null)
            return;

        if (cell.Character != null && cell.Character != attacker)
            return;

        if (!Grid.HasPath(cell) && cell.Character != attacker)
            return;

        if (cell.Value >= attacker.Range)
            return;

        Temp.Add(cell);
    }

    public bool IsInAttackRange(Character attacker)
    {
        int x = 0;
        int y = 0;

        if (attacker.Cell.X < X) x = -1;
        if (attacker.Cell.X > X) x = 1;
        if (attacker.Cell.Y < Y) y = -1;
        if (attacker.Cell.Y > Y) y = 1;

        if (x != 0 && y != 0)
            return false;

        var node = this;
        var value = 0;

        while (node != null)
        {
            if (node == attacker.Cell)
                return true;

            if (value >= attacker.Desc.AttackRange || (node != this && node.Character != null && node.Character != attacker))
                return false;

            node = node.Neighbour(x, y);
            value++;
        }

        return false;
    }

    public Cell AttackOrigin(Character attacker)
    {
        Temp.Clear();

        considerAttackOrigin(Neighbour(-1, 0), attacker);
        considerAttackOrigin(Neighbour(1, 0), attacker);
        considerAttackOrigin(Neighbour(0, -1), attacker);
        considerAttackOrigin(Neighbour(0, 1), attacker);

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
                if (Grid.Attacker != null && Character != null && Grid.Attacker.Team != Character.Team && IsInAttackRange(Grid.Attacker))
                {
                    if (Grid.Hover == this)
                        Material.color = Color.Lerp(Color.red, Color.white, 0.5f);
                    else
                        Material.color = Color.red;
                }
                else if (Grid.Hover != null && Grid.HoverTeam >= 0 && Grid.Hover.Character != null)
                {
                    var last = Grid.Last(X, Grid.HoverTeam);

                    if (Grid.HoverTeam == 0 && last < Y)
                        Material.color = Color.white;
                    else if (Grid.HoverTeam == 1 && last > Y)
                        Material.color = Color.white;
                    else
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
                            {
                                var dir = Grid.HoverTeam == 0 ? 1 : -1;
                                var y = Y + dir;

                                var control = Grid.Hover.Character.Control;

                                while (y >= 0 && y < Grid.Height)
                                {
                                    if (Grid.Cells[X, y].Character != null)
                                    {
                                        control = Grid.Cells[X, y].Character.Control;
                                        break;
                                    }

                                    y += dir;
                                }

                                if (control > 80)
                                    Material.color = Color.green;
                                else if (control > 60)
                                    Material.color = Color.Lerp(Color.green, Color.red, 0.5f);
                                else if (control > 40)
                                    Material.color = Color.red;
                                else if (control > 20)
                                    Material.color = Color.Lerp(Color.red, Color.black, 0.5f);
                                else
                                    Material.color = Color.black;
                            }
                            else
                                Material.color = Color.white;
                        }
                        else
                            Material.color = Color.white;
                    }
                }
                else
                    Material.color = Color.white;
                break;

            case CellStatus.available:
                {
                    if (Grid.Hover != null && Grid.Hover.Status == CellStatus.enemy && Grid.Hover.IsInAttackRange(Grid.Attacker))
                    {
                        Material.color = Color.green;
                    }
                    else
                    {
                        var inPath = false;

                        var node = Grid.Hover;
                        Cell attackOrigin = null;

                        if (Grid.Hover != null && Grid.Hover.Status == CellStatus.enemy)
                        {
                            attackOrigin = Grid.Hover.AttackOrigin(Grid.Attacker);
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
                }
                break;

            case CellStatus.enemy:
                if (Grid.Hover == this)
                    Material.color = Color.Lerp(Color.red, Color.white, 0.5f);
                else
                    Material.color = Color.red;
                break;
        }

        if (BlinkDirection >= 0)
        {
            Blink += Time.deltaTime * 3;

            if (Blink >= 1)
            {
                Blink = 1;
                BlinkDirection = -1;
            }
        }
        else
        {
            Blink -= Time.deltaTime * 3;

            if (Blink <= 0)
            {
                Blink = 0;
                BlinkDirection = 1;
            }
        }

        if (Grid.IsTemp)
            Material.color = Color.Lerp(Material.color, Color.Lerp(Material.color, Color.white, 0.75f), Blink);
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
