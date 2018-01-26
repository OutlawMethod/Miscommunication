using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour
{
    public Grid Grid;
    public Character[] Prefabs;

    public List<Character> Characters = new List<Character>();
    public List<Character> Order = new List<Character>();

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
    }

    private void Update()
    {
        updateInput();   
    }

    private void updateInput()
    {
        var character = Order[0];

        if (Grid.Hover != null && Grid.Hover.Character == null)
        {
            if (Input.GetMouseButtonDown(0))
                giveCommand(character, Grid.Hover);
        }
    }

    private void giveCommand(Character character, Cell target)
    {
        character.Move(target);

        Order.Remove(character);
        Order.Add(character);
    }

    private void placeCharacter(Character prefab, int x, int y, int team)
    {
        var instance = GameObject.Instantiate(prefab.gameObject);
        instance.transform.parent = transform;
        instance.transform.position = Grid.Cells[x, y].transform.position;
        instance.SetActive(true);

        Characters.Add(instance.GetComponent<Character>());
    }
}
