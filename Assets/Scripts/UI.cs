using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour
{
    public Manager Manager;

    public GameObject Skip;

    private void Awake()
    {
        Skip.GetComponent<Button>().onClick.AddListener(Manager.Skip);
    }

    private void Update()
    {
        if (Skip.activeSelf != Manager.CanSkip) Skip.SetActive(Manager.CanSkip);
    }
}
