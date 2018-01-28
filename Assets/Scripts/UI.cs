using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour
{
    public Manager Manager;
    public Director Director;

    public Image Skip;
    public Sprite Red;
    public Sprite Blue;

    private void Awake()
    {
        Skip.GetComponent<Button>().onClick.AddListener(Manager.Skip);
    }

    private void Update()
    {
        var canSkip = Manager.CanSkip && Manager.isActiveAndEnabled && Director.State == DirectorState.game;

        if (Skip.gameObject.activeSelf != canSkip) Skip.gameObject.SetActive(canSkip);
        if (Manager.Team == 0) Skip.sprite = Red;
        if (Manager.Team == 1) Skip.sprite = Blue;
    }
}
