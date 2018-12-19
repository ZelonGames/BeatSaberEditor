using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    #region Fields

    private static List<CutDirection> cutDirections = new List<CutDirection>();
    private static GameObject circle = null;

    [SerializeField]
    private Rect cutDirectionRect;

    private GameObject _2DGrid;
    [SerializeField]
    private CutDirection cutDirectionPrefab;
    [SerializeField]
    private GameObject circlePrefab;

    private float circleRadius;

    private bool hasSetCoordinate = false;

    #endregion

    #region Properties

    public Vector2Int Coordinate { get; private set; }

    #endregion


    private void Start()
    {
        _2DGrid = GameObject.FindGameObjectWithTag("2DGrid");
        cutDirectionRect = cutDirectionPrefab.GetComponent<RectTransform>().rect;
        circleRadius = GridGenerator.Instance.GetMaxRadius(cutDirectionRect.width * 0.5f);
    }

    public void SetCoordinate(Vector2Int coordinate)
    {
        if (hasSetCoordinate)
            return;

        Coordinate = coordinate;

        hasSetCoordinate = true;
    }

    public void TouchDown()
    {
        DeActivateCircleObject();

        ActivateCircle();

        int angle = 0;
        for (int i = 0; i < 8; i++)
        {
            ActivateCutDirection(angle, i);
            angle += 45;
        }
    }

    public static void DeActivateCutDirectionTool()
    {
        DeActivateCircleObject();
        DeActivateAllCutDirections();
    }

    private static void DeActivateAllCutDirections()
    {
        foreach (var cutDirection in cutDirections)
            cutDirection.gameObject.SetActive(false);
    }
    private static void DeActivateCircleObject()
    {
        if (circle != null)
            circle.SetActive(false);
    }

    private void ActivateCircle()
    {
        // Activate if already created
        if (circle != null)
        {
            circle.SetActive(true);
            circle.transform.position = gameObject.transform.position;
            MoveDownGameObjectIndex(circle);

            return;
        }

        // Create new ones for the first time
        circle = Instantiate(circlePrefab.gameObject);
        circle.transform.SetParent(_2DGrid.transform, false);
        circle.transform.position = gameObject.transform.position;
        circle.GetComponent<RectTransform>().sizeDelta = new Vector2(circleRadius * 2 + cutDirectionRect.width, circleRadius * 2 + cutDirectionRect.width);
        circle.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
    }

    private void ActivateCutDirection(float angle, int index)
    {
        // Activate if already created
        if (cutDirections.Count == 8)
        {
            cutDirections[index].gameObject.SetActive(true);
            UpdateCutDirectionValues(cutDirections[index], angle);
            MoveDownGameObjectIndex(cutDirections[index].gameObject);

            return;
        }

        // Create new ones for the first time
        CutDirection newCutDirection = Instantiate(cutDirectionPrefab);
        newCutDirection.transform.SetParent(_2DGrid.transform, false);
        newCutDirection.transform.Rotate(new Vector3(0, 0, 1), angle);
        newCutDirection.SetCutDirection(CutDirection.GetCutDirection(angle));
        UpdateCutDirectionValues(newCutDirection, angle);

        cutDirections.Add(newCutDirection);
    }

    private void UpdateCutDirectionValues(CutDirection cutDirection, float angle)
    {
        cutDirection.tileParent = this;
        cutDirection.gameObject.transform.position = PointOnCircle(circleRadius, angle, gameObject.transform.position);
        cutDirection.GetComponent<Image>().color = MapEditorManager.Instance.CurrentColor == Note.ColorType.Red ? Color.red : Color.blue;
    }

    private Vector2 PointOnCircle(float radius, float angleInDegrees, Vector2 origin)
    {
        float x = (radius * Mathf.Cos(angleInDegrees * Mathf.PI / 180f)) + origin.x;
        float y = (radius * Mathf.Sin(angleInDegrees * Mathf.PI / 180f)) + origin.y;

        return new Vector2(x, y);
    }

    private void MoveDownGameObjectIndex(GameObject gameObject)
    {
        gameObject.transform.SetSiblingIndex(_2DGrid.transform.childCount);
    }
}
