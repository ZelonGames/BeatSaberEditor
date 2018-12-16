using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public CutDirection cutDirectionPrefab;

    private bool hasSetCoordinate = false;

    public Vector2Int Coordinate { get; private set; }

    private void Start()
    {

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
        DestroyAllCutDirections();

        int angle = 0;
        for (int i = 0; i < 8; i++)
        {
            InstantiateCutDirection(PointOnCircle(120, angle, gameObject.transform.position), angle);
            angle += 45;
        }
    }

    private void DestroyAllCutDirections()
    {
        GameObject[] cutDirections = GameObject.FindGameObjectsWithTag("CutDirection");
        foreach (var cutDirection in cutDirections)
        {
            Destroy(cutDirection);
        }
    }

    private void InstantiateCutDirection(Vector2 position, float angle)
    {
        var cutDirection = Instantiate(cutDirectionPrefab);
        cutDirection.tileParent = this;
        cutDirection.transform.SetParent(GameObject.FindGameObjectWithTag("2DGrid").transform, false);
        cutDirection.transform.position = position;
        cutDirection.transform.Rotate(new Vector3(0, 0, 1), angle);
        cutDirection.SetCutDirection(CutDirection.GetCutDirection(angle));
    }

    private Vector2 PointOnCircle(float radius, float angleInDegrees, Vector2 origin)
    {
        float x = (radius * Mathf.Cos(angleInDegrees * Mathf.PI / 180f)) + origin.x;
        float y = (radius * Mathf.Sin(angleInDegrees * Mathf.PI / 180f)) + origin.y;

        return new Vector2(x, y);
    }
}
