using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleDrawingScript : MonoBehaviour {

    public float Theta_Scale;        //Set lower to add more points
    int numOfPoints;               //Total number of points in circle
    public float Radius;
    LineRenderer lineRenderer;

    void Start()
    {
        float sizeValue = (1f) / Theta_Scale;
        numOfPoints = (int)sizeValue;
        numOfPoints++;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount= numOfPoints;
    }

    void Update()
    {
        DrawCircleXZ(Radius, numOfPoints, Theta_Scale, lineRenderer);
       
    }
    
    void DrawCircleXZ(float Radius, int numOfPoints,float Theta_Scale,LineRenderer lineRenderer)
    {
        Vector3 pos;
        float theta = 0f;
        for (int i = 0; i < numOfPoints; i++)
        {
            theta += (2.0f * Mathf.PI * Theta_Scale);
            float x = Radius * Mathf.Cos(theta);
            float z = Radius * Mathf.Sin(theta);
            //x += gameObject.transform.position.x;
            //z += gameObject.transform.position.z;
            pos = new Vector3(x, gameObject.transform.position.y, z);
            lineRenderer.SetPosition(i, pos);
        }
    }
    
}
