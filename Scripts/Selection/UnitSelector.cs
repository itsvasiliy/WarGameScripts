using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private RectTransform selectorImage;

    private Rect selectionRect;

    private Vector2 startPosition;
    private Vector2 endPosition;

    private bool clickStartedOverUI = false;

    private void Start()
    {
        DrawRectangle();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) //Input.GetTouch(0).fingerId))
            {
                clickStartedOverUI = true;
            }
            else
            {

                foreach (Unit unit in SelectionManager.selectedUnitList)
                {
                    if (unit != null && unit.TryGetComponent<IUnit>(out IUnit _unit))
                    {
                        RaycastHit hit;

                        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
                            _unit.SetTarget(hit.point);
                    }
                }

                DeselectAllUnits();

                startPosition = Input.mousePosition;

                selectionRect = new Rect();
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (!clickStartedOverUI)
            {
                endPosition = Input.mousePosition;

                DrawRectangle();

                if (Input.mousePosition.x < startPosition.x)
                {
                    selectionRect.xMin = Input.mousePosition.x;
                    selectionRect.xMax = startPosition.x;
                }
                else
                {
                    selectionRect.xMin = startPosition.x;
                    selectionRect.xMax = Input.mousePosition.x;
                }

                if (Input.mousePosition.y < startPosition.y)
                {
                    selectionRect.yMin = Input.mousePosition.y;
                    selectionRect.yMax = startPosition.y;
                }
                else
                {
                    selectionRect.yMin = startPosition.y;
                    selectionRect.yMax = Input.mousePosition.y;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if(clickStartedOverUI)
            {
                clickStartedOverUI = false;
                return;
            }

            CheckSelectedUnits();

            startPosition = endPosition = Vector2.zero;
            DrawRectangle();
        }
    }

    private void DrawRectangle()
    {
        Vector2 boxStart = startPosition;
        Vector2 center = (boxStart + endPosition) / 2;

        selectorImage.position = center;

        float sizeX = Mathf.Abs(boxStart.x - endPosition.x);
        float sizeY = Mathf.Abs(boxStart.y - endPosition.y);

        selectorImage.sizeDelta = new Vector2(sizeX, sizeY);
    }

    private void CheckSelectedUnits()
    {
        foreach (Unit unit in SelectionManager.unitList)
        {
            if (unit != null && selectionRect.Contains(mainCamera.WorldToScreenPoint(unit.transform.position)))
            {
                unit.SetSelector(true);
                SelectionManager.selectedUnitList.Add(unit);
            }
        }
    }

    private void DeselectAllUnits()
    {
        SelectionManager.selectedUnitList.Clear();

        foreach (Unit unit in SelectionManager.unitList)
        {
            if (unit != null)
                unit.SetSelector(false);
        }
    }
}