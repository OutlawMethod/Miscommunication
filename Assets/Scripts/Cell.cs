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

    private void Awake()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null) renderer = GetComponentInChildren<Renderer>();

        Material = Material.Instantiate(renderer.material);
        renderer.material = Material;
    }

    private void Update()
    {
        switch (Status)
        {
            case CellStatus.default_:
                if (Grid.Hover == this && Character != null && Character.Team == Grid.HoverTeam)
                    Material.color = Color.yellow;
                else
                    Material.color = Color.white;
                break;

            case CellStatus.available:
                if (Grid.Hover == this)
                    Material.color = Color.Lerp(Color.green, Color.white, 0.5f);
                else if (Grid.Hover != null && Grid.Hover.Origin == this)
                    Material.color = Color.Lerp(Color.green, Color.white, 0.25f);
                else
                    Material.color = Color.green;
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
}
