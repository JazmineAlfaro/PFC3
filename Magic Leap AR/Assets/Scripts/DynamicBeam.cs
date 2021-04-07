using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class DynamicBeam : MonoBehaviour
{
    public GameObject controller;
    private LineRenderer beamLine;

    // Start is called before the first frame update 0 references 

    void Start() 
    {
        beamLine = GetComponent<LineRenderer>();
    }

    // Update is called once per frame 0 references 
    void Update()
    {
        transform.position = controller.transform.position;
        //print(transform.position);
        transform.rotation = controller.transform.rotation;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            beamLine.useWorldSpace = true;
            beamLine.SetPosition(0, transform.position);
            beamLine.SetPosition(1, hit.point);
            
        }
        else
        {
            beamLine.useWorldSpace = false;
            beamLine.SetPosition(0, transform.position);
            beamLine.SetPosition(1, Vector3.forward );
           
        }
    }

}
