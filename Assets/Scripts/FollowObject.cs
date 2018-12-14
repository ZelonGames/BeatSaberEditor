using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    private Transform targetTransform;
    [SerializeField]
    private string targetObjectName;

    private Vector3 distanceVector;

    private void Start()
    {
        targetTransform = GameObject.Find(targetObjectName).transform;
        distanceVector = gameObject.transform.position - targetTransform.position;
    }

    private void FixedUpdate()
    {
        gameObject.transform.position = targetTransform.position + distanceVector;
    }
}
