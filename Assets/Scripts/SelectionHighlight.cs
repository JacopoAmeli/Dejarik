using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SelectionHighlight : MonoBehaviour {

    public float Outer_Radius = 5f;
    public float Inner_Radius = 3.1f;
    public float Center_Radius = 1.29f;
    public float Theta_Scale;        //Set lower to add more points
    int numOfPoints;
    LineRenderer lineRenderer;
    int Section = -1;

    // Use this for initialization
    void Start () {
        float sizeValue = (1f) / Theta_Scale;
        numOfPoints = (int)sizeValue;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false; 
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateHighlight();
	}
    private void UpdateHighlight()
    {
        if (!Camera.main)
            return;
        RaycastHit hit;
        float SelectionX;
        float SelectionZ;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessBoard")))
        {
            SelectionX = hit.point.x;
            SelectionZ = hit.point.z;
            int newSection;
            newSection = pointToSection(SelectionX, SelectionZ);
            if (Section == newSection)
                return;
            Section = newSection;
            if (Section < 0)
            {
                lineRenderer.enabled = false;
                return;
            }
            HighlightSection(Section);
        }

    }
    private int pointToSection(float x, float z)
    {
        float radius = Mathf.Sqrt( x * x + z * z);
        if (radius> Outer_Radius)
            return -1;
        if (radius < Center_Radius)
            return 0;
        float angle = Mathf.Rad2Deg* Mathf.Atan(z / x) ;
        //Debug.Log(angle);
        // Quadrant 1: angle positive, cos positive.
        // Quadrant 3: angle positive, cos negative.
        // Quadrant 2: angle negative, cos negative.
        // quadrant 4: angle negative, cos positive. 
        if (radius < Inner_Radius)
        { //INNER CIRCLE
            if(angle>0)
            {
                if(x>0)
                {
                    if (angle < 30)
                        return 1;
                    else if (angle < 60)
                        return 3;
                    else if (angle < 90)
                        return 5; 
                }
                else
                {
                    if (angle < 30)
                        return 13;
                    else if (angle < 60)
                        return 15;
                    else if (angle < 90)
                        return 17;
                }
            }
            else
            {
                if (x < 0)
                {
                    if (angle < -60)
                        return 7;
                    else if (angle < -30)
                        return 9;
                    else
                        return 11;
                }
                else
                {
                    if (angle < -60)
                        return 19;
                    else if (angle < -30)
                        return 21;
                    else
                        return 23;
                }
            }
        }
        else
        { //OUTER CIRCLE
            if (angle > 0)
            {
                if (x > 0)
                {
                    if (angle < 30)
                        return 2;
                    else if (angle < 60)
                        return 4;
                    else if (angle < 90)
                        return 6;
                }
                else
                {
                    if (angle < 30)
                        return 14;
                    else if (angle < 60)
                        return 16;
                    else if (angle < 90)
                        return 18;
                }
            }
            else
            {
                if (x < 0)
                {
                    if (angle < -60)
                        return 8;
                    else if (angle < -30)
                        return 10;
                    else
                        return 12;
                }
                else
                {
                    if (angle < -60)
                        return 20;
                    else if (angle < -30)
                        return 22;
                    else
                        return 24;
                }
            }
        }
        return -1;
    }
    private void HighlightSection(int Section)
    {
        Debug.Log(Section);
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        switch (Section)
        {
            case (0):
                {                 
                    lineRenderer.positionCount = numOfPoints+1;
                    DrawCircleXZ(Center_Radius, numOfPoints+1, Theta_Scale, lineRenderer);
                    break;
                }
            case (1):
                {
                    Arc(0f, numOfPoints, Theta_Scale, lineRenderer, Center_Radius, Inner_Radius);
                    break;
                }
            case (2):
                {
                    Arc(0f, numOfPoints, Theta_Scale, lineRenderer,Inner_Radius, Outer_Radius);
                    break;
                }
            case (3):
                {
                    Arc(Mathf.PI/6, numOfPoints, Theta_Scale, lineRenderer, Center_Radius, Inner_Radius);
                    break;
                }
            case (4):
                {
                    Arc(Mathf.PI/6, numOfPoints, Theta_Scale, lineRenderer, Inner_Radius, Outer_Radius);
                    break;
                }
            case (5):
                {
                    Arc(Mathf.PI / 3, numOfPoints, Theta_Scale, lineRenderer, Center_Radius, Inner_Radius);               
                    break;
                }
            case (6):
                {
                    Arc(Mathf.PI / 3, numOfPoints, Theta_Scale, lineRenderer, Inner_Radius, Outer_Radius);
                    break;
                }
            case (7):
                {
                    Arc(Mathf.PI /2 , numOfPoints, Theta_Scale, lineRenderer, Center_Radius, Inner_Radius);
                    break;
                }
            case (8):
                {
                    Arc(Mathf.PI / 2, numOfPoints, Theta_Scale, lineRenderer, Inner_Radius, Outer_Radius);
                    break;
                }
            case (9):
                {
                    Arc(Mathf.PI* 2/ 3, numOfPoints, Theta_Scale, lineRenderer, Center_Radius, Inner_Radius);
                    break;
                }
            case (10):
                {
                    Arc(Mathf.PI *2/ 3, numOfPoints, Theta_Scale, lineRenderer, Inner_Radius, Outer_Radius);
                    break;
                }
            case (11):
                {
                    Arc(Mathf.PI*5/6, numOfPoints, Theta_Scale, lineRenderer, Center_Radius, Inner_Radius);
                    break;
                }
            case (12):
                {
                    Arc(Mathf.PI*5/6, numOfPoints, Theta_Scale, lineRenderer, Inner_Radius, Outer_Radius);
                    break;
                }
            case (13):
                {
                    Arc(Mathf.PI, numOfPoints, Theta_Scale, lineRenderer, Center_Radius, Inner_Radius);
                    break;
                }
            case (14):
                {
                    Arc(Mathf.PI, numOfPoints, Theta_Scale, lineRenderer, Inner_Radius, Outer_Radius);
                    break;
                }
            case (15):
                {
                    Arc(Mathf.PI * 7 / 6, numOfPoints, Theta_Scale, lineRenderer, Center_Radius, Inner_Radius);
                    break;
                }
            case (16):
                {
                    Arc(Mathf.PI*7/6, numOfPoints, Theta_Scale, lineRenderer, Inner_Radius, Outer_Radius);
                    break;
                }
            case (17):
                {
                    Arc(Mathf.PI * 4 / 3, numOfPoints, Theta_Scale, lineRenderer, Center_Radius, Inner_Radius);
                    break;
                }
            case (18):
                {
                    Arc(Mathf.PI * 4 / 3, numOfPoints, Theta_Scale, lineRenderer, Inner_Radius, Outer_Radius);
                    break;
                }
            case (19):
                {
                    Arc(Mathf.PI * 3 / 2, numOfPoints, Theta_Scale, lineRenderer, Center_Radius, Inner_Radius);
                    break;
                }
            case (20):
                {
                    Arc(Mathf.PI * 3 / 2, numOfPoints, Theta_Scale, lineRenderer, Inner_Radius, Outer_Radius);
                    break;
                }
            case (21):
                {
                    Arc(Mathf.PI * 5 / 3, numOfPoints, Theta_Scale, lineRenderer, Center_Radius, Inner_Radius);
                    break;
                }
            case (22):
                {
                    Arc(Mathf.PI * 5 / 3, numOfPoints, Theta_Scale, lineRenderer, Inner_Radius, Outer_Radius);
                    break;
                }
            case (23):
                {
                    Arc(Mathf.PI * 11 / 6, numOfPoints, Theta_Scale, lineRenderer, Center_Radius, Inner_Radius);
                    break;
                }
            case (24):
                {
                    Arc(Mathf.PI *11 / 6, numOfPoints, Theta_Scale, lineRenderer, Inner_Radius, Outer_Radius);
                    break;
                } 
        }
        lineRenderer.enabled = true;
        return;
    }
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
            pos = new Vector3(x, gameObject.transform.position.y, z);
            lineRenderer.SetPosition(i, pos);
        }
    }
    
    //will trace a 30° arc from initialTheta on inner circle , then move to the outer circle and draw another 30° arc.
    private void Arc(float initialTheta, int numOfPoints, float Theta_Scale, LineRenderer lineRenderer, float innerRadius, float outerRadius)
    {
        lineRenderer.positionCount = numOfPoints+1;
        Vector3 pos;
        float theta = initialTheta;
        float x = innerRadius * Mathf.Cos(theta);
        float z = innerRadius * Mathf.Sin(theta);
        //x += gameObject.transform.position.x;
        //z += gameObject.transform.position.z;
        pos = new Vector3(x, gameObject.transform.position.y, z);
        Vector3 initialPos = pos;
        lineRenderer.SetPosition(0, pos);

        //first the inner arc
        for (int i = 1; i <( numOfPoints / 2 )+1; i++)
        {
            theta += (Mathf.PI * Theta_Scale) / 3;
            x = innerRadius * Mathf.Cos(theta);
            z = innerRadius * Mathf.Sin(theta);
            //x += gameObject.transform.position.x;
            //z += gameObject.transform.position.z;
            pos = new Vector3(x, gameObject.transform.position.y, z);
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
            pos = new Vector3(x, gameObject.transform.position.y, z);
            lineRenderer.SetPosition(i + 1+ numOfPoints / 2, pos);
        }
        lineRenderer.SetPosition(numOfPoints, initialPos);
    }
}
