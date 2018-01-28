using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
    public Manager Manager;

    public float SwitchDuration = 0.7f;

    public float Turn;
    public float Zoom;
    public float CurrentZoom;

    public float ZoomTargetTransition;

    public Transform Team0;
    public Transform Team1;

    public float Transition;
    public Vector3 TransitionOriginPosition;
    public Quaternion TransitionOriginRotation;
    public int Team;
    public int PreviousTeam;

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

        if (Team != PreviousTeam)
        {
            Turn = 0;
            Zoom = 0;
            CurrentZoom = 0;
            PreviousTeam = Team;
            ZoomTargetTransition = 0;
        }

        if (Manager.isActiveAndEnabled && Transition > 0.9f)
        {
            if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
                Turn += Input.GetAxis("Mouse X") * 2;

            Zoom = Mathf.Clamp01(Zoom + Input.GetAxis("Mouse ScrollWheel"));
        }

        CurrentZoom = Mathf.Lerp(CurrentZoom, Zoom, Time.deltaTime * 10);
        
        var rotation = Quaternion.AngleAxis(Turn, Vector3.up);
        var zoom = CurrentZoom * 0.75f;
        var zoomTarget = Vector3.zero;

        if (Manager.Current != null)
        {
            ZoomTargetTransition = Mathf.Clamp01(ZoomTargetTransition + Time.deltaTime * 3);
            var shift = Fluid.Lerp(0, 1, ZoomTargetTransition, AnimationMode.easeInOut);

            if (Manager.Current.IsMoving || !Manager.Current.IsActing)
                zoomTarget = Vector3.Lerp(zoomTarget, Manager.Current.transform.position, shift);
            else
                zoomTarget = Vector3.Lerp(zoomTarget, Manager.Current.Cell.transform.position, shift);
        }

        if (Team == 0)
        {
            var target = Vector3.Lerp(rotation * Team0.position, zoomTarget, zoom);

            transform.position = Fluid.Lerp(TransitionOriginPosition, target, Transition, AnimationMode.easeInOut);
            transform.rotation = Fluid.Lerp(TransitionOriginRotation, rotation * Team0.rotation, Transition, AnimationMode.easeInOut);
        }
        else if (Team == 1)
        {
            var target = Vector3.Lerp(rotation * Team1.position, zoomTarget, zoom);

            transform.position = Fluid.Lerp(TransitionOriginPosition, target, Transition, AnimationMode.easeInOut);
            transform.rotation = Fluid.Lerp(TransitionOriginRotation, rotation * Team1.rotation, Transition, AnimationMode.easeInOut);
        }
    }
}
