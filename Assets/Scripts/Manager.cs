using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct CharacterSet
{
    public Character[] Teams;
}

public class Manager : MonoBehaviour
{
    public Grid Grid;
    public CharacterSet[] Prefabs;

    public List<Character> Characters = new List<Character>();
    public List<Character> Order = new List<Character>();

    public Character OrderTarget;

    private void Awake()
    {
        Grid.Setup();

        for (int i = 0; i < Prefabs.Length; i++)
        {
            placeCharacter(Prefabs[i], i, 0, 0);
            placeCharacter(Prefabs[i], i, Grid.Height - 1, 1);
        }

        foreach (var character in Characters)
            Order.Add(character);

        updateNextOrder();
    }

    private void Update()
    {
        if (OrderTarget != null && OrderTarget.IsMoving)
            return;
        else if (OrderTarget != null)
            updateNextOrder();

        updateInput();   
    }

    private void updateInput()
    {
        var character = Order[0];

        if (Grid.Hover != null && Dijkstra.HasPath(Grid.Hover))
        {
            if (Input.GetMouseButtonDown(0))
                giveCommand(character, Grid.Hover);
        }
    }

    private void giveCommand(Character character, Cell target)
    {
        if (Dijkstra.HasPath(target))
            character.Move(Dijkstra.Path(target));

        OrderTarget = character;
        Order.Remove(character);
        Order.Add(character);
    }

    private void placeCharacter(CharacterSet prefabs, int x, int y, int team)
    {
        var instance = GameObject.Instantiate(prefabs.Teams[team].gameObject);
        instance.transform.parent = transform;
        instance.transform.position = Grid.Cells[x, y].transform.position;
        instance.SetActive(true);

        var character = instance.GetComponent<Character>();
        character.Cell = Grid.Cells[x, y];

        Characters.Add(character);
    }

    private void updateNextOrder()
    {
        Dijkstra.Setup(Grid, Order[0].Cell, Order[0].Range);
        OrderTarget = null;

        Grid.UpdateStatus();
    }
}
