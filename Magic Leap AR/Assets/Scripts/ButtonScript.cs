using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using static Variables;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public GameObject sphere;
    public GameObject prefab, prefab2;
    public GameObject combinedMeshes;
    private GameObject mapper;
    private GameObject[] respawns;
    private GameObject[] selectedMeshes;
    private MLInput.Controller _controller;
    public Material material;

    void Start()
    {
        MLInput.Start();
        MLInput.OnControllerButtonDown += OnButtonDown;
        MLInput.OnControllerButtonUp += OnButtonUp;
        _controller = MLInput.GetController(MLInput.Hand.Left);
    }

    void OnDestroy()
    {
        MLInput.OnControllerButtonDown -= OnButtonDown;
        MLInput.OnControllerButtonUp -= OnButtonUp;
        MLInput.Stop();
    }

    void Update()
    {

    }
  
    void OnButtonDown(byte controllerId, MLInput.Controller.Button button)
    {

    }

    void OnButtonUp(byte controllerId, MLInput.Controller.Button button)
    {
        if (button == MLInput.Controller.Button.Bumper)
        {
            RaycastHit hit;

            if (Physics.Raycast(_controller.Position, transform.forward, out hit))
            {
                Variables.Flag = false;
                sphere.transform.position = hit.point;
                sphere.GetComponent<Collider>().enabled = true;
             
            }
        }
        if (button == MLInput.Controller.Button.HomeTap)
        {
            respawns = GameObject.FindGameObjectsWithTag("Mesh");
            mapper = GameObject.FindGameObjectWithTag("SMapper");

            print(respawns.Length);

            foreach (GameObject i in respawns) {
                Destroy(i);
            }

            Destroy(mapper);

            selectedMeshes = GameObject.FindGameObjectsWithTag("MeshCol");
            CombineInstance[] meshFilters = new CombineInstance[selectedMeshes.Length];

            int j = 0;
            while (j < selectedMeshes.Length)
            {
                meshFilters[j].mesh = selectedMeshes[j].GetComponent<MeshFilter>().sharedMesh;
                meshFilters[j].transform = selectedMeshes[j].GetComponent<MeshFilter>().transform.localToWorldMatrix;
                j++;
            }

            combinedMeshes.GetComponent<MeshFilter>().mesh = new Mesh();
            combinedMeshes.GetComponent<MeshFilter>().mesh.CombineMeshes(meshFilters);
            
            foreach (GameObject i in selectedMeshes)
            {
                Destroy(i);
            }

            

            Mesh mesh = combinedMeshes.GetComponent<MeshFilter>().mesh;

            Vector3[] vertices = mesh.vertices;
            var triangles = mesh.triangles;

            Vector3 minVector = Vector3.positiveInfinity;
            Vector3 maxVector = Vector3.zero;
            for (int i = 0; i < vertices.Length; i++)
            {
                minVector = (vertices[i].magnitude < minVector.magnitude) ? vertices[i] : minVector;
                maxVector = (vertices[i].magnitude > maxVector.magnitude) ? vertices[i] : maxVector;
            }
            print("------------");
            print(minVector);
            print(maxVector);
            print("------------");

            /*
            Vector2[] uvs = new Vector2[vertices.Length];
            Bounds bounds = mesh.bounds;
            int i = 0;
            while (i < uvs.Length)
            {
                uvs[i] = new Vector2(vertices[i].x / bounds.size.x, vertices[i].z / bounds.size.x);
                i++;
            }
            mesh.uv = uvs;

            */
            /*

           Vector3[] normals = mesh.normals;
           Vector3[] vertices = mesh.vertices;
          
           int tam = triangles.Length;
           int[] duplicates = new int[tam];


           for (int i = 0; i < triangles.Length/3; i++)
           {

               for (int k = 0; k < triangles.Length/3; k++)
               {
                   if(i != k)
                   {
                       if ((triangles[i] == triangles[k]) && (triangles[i+1] == triangles[k+1]) && (triangles[i+2] == triangles[k + 2]))
                       {
                           duplicates[k] = 1;
                           print("Entra");
                       }
                       else
                       {
                           print("GG");
                           print("------------");
                           print(triangles[i]);
                           print(triangles[k]);
                           print(triangles[k + 1]);
                           print(triangles[i + 1]);
                           print(triangles[k + 2]);
                           print(triangles[i + 2]);
                           print("------------");
                       }
                   }




               }

           }

           */


            for (int i = 0; i < vertices.Length; i++)
            {
                Instantiate(prefab, vertices[i], Quaternion.identity);
            }


            /*
                for (int i = 0; i < triangles.Length/3; i++)
            {
               
                Vector3 res = (vertices[triangles[i + 1]]+ vertices[triangles[i + 0]]+ vertices[triangles[i + 2]])/3.0f;

                Instantiate(prefab, vertices[triangles[i ]], Quaternion.identity);
                Instantiate(prefab, vertices[triangles[i + 1]], Quaternion.identity);
                Instantiate(prefab, vertices[triangles[i + 2]], Quaternion.identity);

                //print(res);
                //Instantiate(prefab2, res, Quaternion.identity);


            }

    */



        }
    }
   
}