using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject cutDirectionPrefab;

    private bool isPressed = false;

    private void Start()
    {

    }

    public void TouchDown()
    {
        int angle = 0;
        for (int i = 0; i < 8; i++)
        {
            InstantiateCutDirection(PointOnCircle(120, angle, gameObject.transform.position), angle - 90);
            angle += 45;
        }

        isPressed = true;
    }

    public void TouchRelease()
    {
        if (!isPressed)
            return;
        
        var cutDirections = GameObject.FindGameObjectsWithTag("CutDirection");
        foreach (var cutDirection in cutDirections)
        {
            Destroy(cutDirection);
        }

        isPressed = false;
    }

    private void InstantiateCutDirection(Vector2 position, float rotation)
    {
        var cutDirection = Instantiate(cutDirectionPrefab);
        cutDirection.transform.SetParent(gameObject.transform, false);
        cutDirection.transform.position = position;
        cutDirection.transform.Rotate(new Vector3(0, 0, 1), rotation);
    }

    private Vector2 PointOnCircle(float radius, float angleInDegrees, Vector2 origin)
    {
        float x = (radius * Mathf.Cos(angleInDegrees * Mathf.PI / 180f)) + origin.x;
        float y = (radius * Mathf.Sin(angleInDegrees * Mathf.PI / 180f)) + origin.y;

        return new Vector2(x, y);
    }
}
