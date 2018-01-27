using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
    public Manager Manager;

    public float SwitchDuration = 0.7f;

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

        Transition = Mathf.Clamp01(Transition + Time.deltaTime / SwitchDuration);

        if (Team == 0)
        {
            transform.position = Fluid.Lerp(TransitionOriginPosition, Team0.position, Transition, AnimationMode.easeInOut);
            transform.rotation = Fluid.Lerp(TransitionOriginRotation, Team0.rotation, Transition, AnimationMode.easeInOut);
        }
        else if (Team == 1)
        {
            transform.position = Fluid.Lerp(TransitionOriginPosition, Team1.position, Transition, AnimationMode.easeInOut);
            transform.rotation = Fluid.Lerp(TransitionOriginRotation, Team1.rotation, Transition, AnimationMode.easeInOut);
        }
    }
}
