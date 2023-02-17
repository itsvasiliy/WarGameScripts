using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollAndPinch : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform _transform;

    [SerializeField] private bool rotate;

    protected Plane plane;

    private float minZoom = 0f;
    private float maxZoom = 10f;

    private void Update()
    {
        var delta1 = Vector3.zero;
        var delta2 = Vector3.zero;

        if (Input.touchCount >= 1)
        {
            plane.SetNormalAndPosition(_transform.up, _transform.position);

            delta1 = PlanePositionDelta(Input.GetTouch(0));

            if (Input.GetTouch(0).phase == TouchPhase.Moved)
                cameraTransform.Translate(delta1, Space.World);
        }

        //Pinch
        if (Input.touchCount >= 2)
        {
            var pos1 = PlanePosition(Input.GetTouch(0).position);
            var pos2 = PlanePosition(Input.GetTouch(1).position);
            var pos1b = PlanePosition(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
            var pos2b = PlanePosition(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

            //calc zoom
            var zoom = Vector3.Distance(pos1, pos2) /
                       Vector3.Distance(pos1b, pos2b);

            //edge case
            if (zoom == minZoom || zoom > maxZoom)
                return;

            //Move cam amount the mid ray
            cameraTransform.position = Vector3.LerpUnclamped(pos1, cameraTransform.position, 1 / zoom);

            if (rotate && pos2b != pos2)
                cameraTransform.RotateAround(pos1, plane.normal, Vector3.SignedAngle(pos2 - pos1, pos2b - pos1b, plane.normal));
        }
    }

    protected Vector3 PlanePositionDelta(Touch touch)
    {
        if (touch.phase != TouchPhase.Moved)
            return Vector3.zero;

        var rayBefore = mainCamera.ScreenPointToRay(touch.position - touch.deltaPosition);
        var rayNow = mainCamera.ScreenPointToRay(touch.position);

        if (plane.Raycast(rayBefore, out var enterBefore) && plane.Raycast(rayNow, out var enterNow))
            return rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);

        return Vector3.zero;
    }

    protected Vector3 PlanePosition(Vector2 screenPos)
    {
        var rayNow = mainCamera.ScreenPointToRay(screenPos);

        if (plane.Raycast(rayNow, out var enterNow))
            return rayNow.GetPoint(enterNow);

        return Vector3.zero;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(_transform.position, _transform.position + _transform.up);
    //}
}