using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject cutDirectionPrefab;

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnMouseDown()
    {
        var cutDirection = Instantiate(cutDirectionPrefab);
        cutDirection.transform.SetParent(gameObject.transform, false);
        cutDirection.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 100);
    }
}
