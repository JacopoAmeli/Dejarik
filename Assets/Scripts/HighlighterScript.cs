using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class HighlighterScript : MonoBehaviour {
    //Radius of circles
    public float Outer_Radius = 4.95f;
    public float Inner_Radius = 3.1f;
    public float Center_Radius = 1.29f;
    // theta scale= 1/Number of points, also used for delta angles
    public float Theta_Scale = 0.01f;        //Set lower to add more points
    int numOfPoints;
    LineRenderer lineRenderer;

    // Use this for initialization
    void Start ()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }
    private void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
        float sizeValue = (1f) / Theta_Scale;
        numOfPoints = (int)sizeValue;
    }
    // Update is called once per frame
    void Update () {
		
	}
    public void Disable()
    {
        lineRenderer.enabled = false;
    }
    public void Enable()
    {
        lineRenderer.enabled = true;
    }
    public bool isEnabled()
    {
        return lineRenderer.enabled;
    }
    public void ChangeColor(Gradient newColor)
    {
        lineRenderer.colorGradient = newColor;
    }
    public void HighlightSection(int section)
    {
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        float startPos;
        if (section == 0)
        {
            lineRenderer.positionCount = numOfPoints + 1;
            DrawCircleXZ(Center_Radius, numOfPoints + 1, Theta_Scale, lineRenderer);
            lineRenderer.enabled = true;
            return;
        }
        if (section % 2 == 1)
        {
            startPos = (float)section - 1;
            Arc(startPos * Mathf.PI / 12, numOfPoints, Theta_Scale, lineRenderer, Center_Radius, Inner_Radius);
        }
        else
        {
            startPos = (float)section - 2;
            Arc(startPos * Mathf.PI / 12, numOfPoints, Theta_Scale, lineRenderer, Inner_Radius, Outer_Radius);
        }
        lineRenderer.enabled = true;
        return;
    }
    //Draws a circle around origin
    void DrawCircleXZ(float Radius, int numOfPoints, float Theta_Scale, LineRenderer lineRenderer)
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
            pos = new Vector3(x, gameObject.transform.position.y + 0.1f, z);
            lineRenderer.SetPosition(i, pos);
        }
    }

    public void DrawCirclePlayer(Vector3 center,float Radius)
    {
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        lineRenderer.positionCount = numOfPoints;
        Vector3 pos;
        float theta = 0f;
        for (int i = 0; i < numOfPoints; i++)
        {
            theta += (2.0f * Mathf.PI * Theta_Scale);
            float x = (Radius * Mathf.Cos(theta) )+ center.x;
            float z = (Radius * Mathf.Sin(theta) )+ center.z;
            pos = new Vector3(x, gameObject.transform.position.y + 0.01f, z);
            lineRenderer.SetPosition(i, pos);
        }
        lineRenderer.enabled = true;
    }
    //will trace a 30° arc from initialTheta on inner circle , then move to the outer circle and draw another 30° arc.
    private void Arc(float initialTheta, int numOfPoints, float Theta_Scale, LineRenderer lineRenderer, float innerRadius, float outerRadius)
    {
        lineRenderer.positionCount = numOfPoints + 1;
        Vector3 pos;
        float theta = initialTheta;
        float x = innerRadius * Mathf.Cos(theta);
        float z = innerRadius * Mathf.Sin(theta);
        //x += gameObject.transform.position.x;
        //z += gameObject.transform.position.z;
        pos = new Vector3(x, gameObject.transform.position.y + 0.1f, z);
        Vector3 initialPos = pos;
        lineRenderer.SetPosition(0, pos);

        //first the inner arc
        for (int i = 1; i < (numOfPoints / 2) + 1; i++)
        {
            theta += (Mathf.PI * Theta_Scale) / 3;
            x = innerRadius * Mathf.Cos(theta);
            z = innerRadius * Mathf.Sin(theta);
            //x += gameObject.transform.position.x;
            //z += gameObject.transform.position.z;
            pos = new Vector3(x, gameObject.transform.position.y + 0.1f, z);
            lineRenderer.SetPosition(i, pos);
        }

        //outer arc
        for (int i = 0; i < numOfPoints / 2; i++)
        {
            theta -= (Mathf.PI * Theta_Scale) / 3;
            x = outerRadius * Mathf.Cos(theta);
            z = outerRadius * Mathf.Sin(theta);
            //x += gameObject.transform.position.x;
            //z += gameObject.transform.position.z;
            pos = new Vector3(x, gameObject.transform.position.y + 0.1f, z);
            lineRenderer.SetPosition(i + 1 + numOfPoints / 2, pos);
        }
        lineRenderer.SetPosition(numOfPoints, initialPos);
    }
}
