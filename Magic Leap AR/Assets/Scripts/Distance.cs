using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Distance : MonoBehaviour
{
    public Vector3 position1, position2;//, camera;
    private LineRenderer lineRend;
    public double distance;
    public double distance1C;
    public double distance2C;
    //public TextMeshProUGUI textMesh, dis1, dis2;
    public Text textMesh;
    private double angleXY, angleXZ, angleYZ;

    // Start is called before the first frame update
    void Start()
    {
        distance = 0;
        distance1C = 0;
        distance2C = 0;
        angleXY = 0;
        angleXZ = 0;
        angleYZ = 0;
        lineRend = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        position1 = GameObject.FindGameObjectWithTag("Marcador1").transform.position;
        position2 = GameObject.FindGameObjectWithTag("Marcador2").transform.position;
        //camera = GameObject.FindGameObjectWithTag("MainCamera").transform.position;

        angleXY = 0;
        angleXZ = 0;
        angleYZ = 0;

        double axisX = position1.x * 100 - position2.x * 100;
        double axisY = position1.y * 100 - position2.y * 100;
        double axisZ = position1.z * 100 - position2.z * 100;
        //print("-------");
        //print(axisX);
        //print(axisY);
        //print("Angulo XY: ");
        //print(Math.Atan(axisX / axisY) * 180 / Math.PI);



        //print(axisZ);




        angleXY = Math.Round(Math.Atan(axisX / axisY) * 180 / Math.PI, 2);
        angleXZ = Math.Round(Math.Atan(axisX / axisZ) * 180 / Math.PI, 2);
        angleYZ = Math.Round(Math.Atan(axisZ / axisY) * 180 / Math.PI, 2);


        if (axisX / axisY < 0)
        {
            angleXY += 180;
        }
        if (axisX / axisZ < 0)
        {
            angleXZ += 180;
        }
        if (axisZ / axisY < 0)
        {
            angleYZ += 180;
        }


        lineRend.SetPosition(0, position1);
        lineRend.SetPosition(1, position2);
        //distance = Math.Round(((position1 - position2).magnitude) / 100, 2);
        distance = Math.Round(((position1 - position2).magnitude * 100), 2);
        textMesh.text = "Distancia: " + distance.ToString() + " cm";//  AXY: " + angleXY.ToString() + "°";


       // distance1C = Math.Round(((position1 - camera).magnitude * 100), 2);
        //dis1.text = "M1-Cam: " + distance1C.ToString() + " cm  AXZ: " + angleXZ.ToString() + "°";

        //distance2C = Math.Round(((camera - position2).magnitude * 100), 2);
        //dis2.text = "M2-Cam: " + distance2C.ToString() + " cm  AYZ: " + angleYZ.ToString() + "°";





    }

}
