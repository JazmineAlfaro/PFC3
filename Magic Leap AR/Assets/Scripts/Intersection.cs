using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Variables;


public class Intersection : MonoBehaviour
{
    public Material tex;
    public Material prev;

    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        print(collision.collider.name);
        print(Variables.Flag);
        if (collision.gameObject.tag == "Mesh")
        {
            collision.gameObject.GetComponent<Renderer>().material = tex;
            collision.gameObject.tag = "MeshCol";

        }

        else if(collision.gameObject.tag == "MeshCol")
        {
            collision.gameObject.GetComponent<Renderer>().material = prev;
            collision.gameObject.tag = "Mesh";
        }
        
        
     
        

    }

}
