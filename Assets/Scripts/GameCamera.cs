using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
    public Manager Manager;

    public float Duration = 0.5f;

    public Transform Team0;
    public Transform Team1;

    public float Transition;
    public Vector3 TransitionOriginPosition;
    public Quaternion TransitionOriginRotation;
    public int Team;

    private void Awake()
    {
        TransitionOriginPosition = transform.position;
        TransitionOriginRotation = transform.rotation;
        Transition = 1;
        Team = 0;
        transform.position = Team0.transform.position;
    }

    private void Update()
    {
        if (Team != Manager.Team)
        {
            Team = Manager.Team;
            Transition = 0;
            TransitionOriginPosition = transform.position;
            TransitionOriginRotation = transform.rotation;
        }

        Transition = Mathf.Clamp01(Transition + Time.deltaTime / Duration);

        if (Team == 0)
        {
            transform.position = Vector3.Lerp(TransitionOriginPosition, Team0.position, Transition);
            transform.rotation = Quaternion.Lerp(TransitionOriginRotation, Team0.rotation, Transition);
        }
        else if (Team == 1)
        {
            transform.position = Vector3.Lerp(TransitionOriginPosition, Team1.position, Transition);
            transform.rotation = Quaternion.Lerp(TransitionOriginRotation, Team1.rotation, Transition);
        }
    }
}
