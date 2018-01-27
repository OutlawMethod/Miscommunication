using UnityEngine;
using System.Collections.Generic;

public class HeartPanel : MonoBehaviour
{
    public GameObject Prefab;
    public float Spacing = 48;

    public Character Character;
    public Manager Manager;

    public List<Heart> Hearts = new List<Heart>();
    public bool IsTargeted;

    public void Setup()
    {
        var line0 = Character.Lives / 2 + 1;
        var line1 = Character.Lives - line0;

        for (int i = 0; i < line0; i++)
            add(i - (float)line0 / 2, 0);

        for (int i = 0; i < line1; i++)
            add(i - (float)line1 / 2, 1);
    }

    public void Target(int damage)
    {
        for (int i = 0; i < Character.Lives; i++)
            Hearts[i].Target = i >= Hearts.Count - damage;

        IsTargeted = true;
    }

    private void add(float x, float y)
    {
        var instance = GameObject.Instantiate(Prefab);
        instance.transform.parent = transform;
        instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * Spacing, y * Spacing);
        instance.SetActive(true);

        Hearts.Add(instance.GetComponent<Heart>());
    }

    private void Update()
    {
        if (IsTargeted)
            IsTargeted = false;
        else for (int i = 0; i < Hearts.Count; i++)
            Hearts[i].Target = false;

        for (int i = 0; i < Hearts.Count; i++)
            Hearts[i].Removed = Character.Lives <= i;

        var rect = GetComponent<RectTransform>();
        var screen = Camera.main.WorldToScreenPoint(Character.transform.position);
        screen.y += 50;

        rect.position = screen;
    }
}
