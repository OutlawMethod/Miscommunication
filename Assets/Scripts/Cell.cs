using UnityEngine;

public enum CellStatus
{
    default_,
    available,
}

public class Cell : MonoBehaviour
{
    public Character Character;

    public CellStatus Status;

    public Grid Grid;
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
            case CellStatus.default_: Material.color = Color.white; break;
            case CellStatus.available: Material.color = Color.green; break;
        }
    }

    private void OnMouseOver()
    {
        Grid.Hover = this;
    }
}
