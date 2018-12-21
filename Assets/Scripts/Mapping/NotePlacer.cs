using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NotePlacer : MonoBehaviour
{
    #region Fields

    private List<CutDirectionButton> cutDirections = new List<CutDirectionButton>();
    private GameObject circle = null;

    private RectTransform tileRect;
    private Rect cutDirectionRect;
    [SerializeField]
    private CutDirectionButton cutDirectionPrefab;
    [SerializeField]
    private GameObject circlePrefab;

    private GameObject _2DGrid;

    private float circleRadius;

    #endregion

    public void Start()
    {
        _2DGrid = GameObject.FindGameObjectWithTag("2DGrid");

        gameObject.transform.SetParent(_2DGrid.transform, false);
    }

    #region Methods

    public void Activate(Tile tile)
    {
        cutDirectionRect = cutDirectionPrefab.GetComponent<RectTransform>().rect;
        circleRadius = GridGenerator.Instance.GetMaxRadius(cutDirectionRect.width * 0.5f);
        Move(tile);
        ActivateCircle(tile);
        float angle = 0;
        for (int i = 0; i < 8; i++)
        {
            ActivateCutDirection(tile, angle, i);
            angle += 45;
        }
    }

    public void Move(Tile tile)
    {
        tileRect = tile.GetComponent<RectTransform>();
        gameObject.transform.localPosition = new Vector2(tileRect.localPosition.x, tileRect.localPosition.y);

        foreach (var cutDirection in cutDirections)
            UpdateCutDirectionValues(tile, cutDirection);
    }

    #region Activation

    private void ActivateCircle(Tile tile)
    {
        circle = Instantiate(circlePrefab.gameObject);
        circle.transform.SetParent(gameObject.transform, false);
        circle.transform.position = gameObject.transform.position;
        circle.GetComponent<RectTransform>().sizeDelta = new Vector2(circleRadius * 2 + cutDirectionRect.width, circleRadius * 2 + cutDirectionRect.width);
        circle.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
    }

    private void ActivateCutDirection(Tile tile, float angle, int index)
    {
        CutDirectionButton newCutDirection = Instantiate(cutDirectionPrefab);
        newCutDirection.transform.SetParent(gameObject.transform, false);
        newCutDirection.transform.Rotate(new Vector3(0, 0, 1), angle);
        newCutDirection.SetCutDirection(CutDirectionButton.GetCutDirection(angle));

        newCutDirection.gameObject.transform.position = PointOnCircle(circleRadius, angle, gameObject.transform.position);
        UpdateCutDirectionValues(tile, newCutDirection);

        cutDirections.Add(newCutDirection);
    }

    #endregion

    private void UpdateCutDirectionValues(Tile tile, CutDirectionButton cutDirection)
    {
        cutDirection.tileParent = tile;
        cutDirection.GetComponent<Image>().color = MapEditorManager.Instance.ItemType == Note.ItemType.Red ? Color.red : Color.blue;
    }

    public void MoveDownGameObjectIndex(GameObject gameObject)
    {
        gameObject.transform.SetSiblingIndex(gameObject.transform.parent.childCount - 1);
    }

    private Vector2 PointOnCircle(float radius, float angleInDegrees, Vector2 origin)
    {
        float x = (radius * Mathf.Cos(angleInDegrees * Mathf.PI / 180f)) + origin.x;
        float y = (radius * Mathf.Sin(angleInDegrees * Mathf.PI / 180f)) + origin.y;

        return new Vector2(x, y);
    }

    #endregion
}
