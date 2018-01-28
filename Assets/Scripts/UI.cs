using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour
{
    public Manager Manager;

    public Image Skip;
    public Sprite Red;
    public Sprite Blue;

    private void Awake()
    {
        Skip.GetComponent<Button>().onClick.AddListener(Manager.Skip);
    }

    private void Update()
    {
        if (Skip.gameObject.activeSelf != Manager.CanSkip) Skip.gameObject.SetActive(Manager.CanSkip);
        if (Manager.Team == 0) Skip.sprite = Red;
        if (Manager.Team == 1) Skip.sprite = Blue;
    }
}
