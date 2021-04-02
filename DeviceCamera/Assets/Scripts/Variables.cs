using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variables : MonoBehaviour
{

    private static WebCamTexture _texture = new WebCamTexture();

    public static WebCamTexture Texture
    {
        get
        {
            return _texture;
        }
        set
        {
            _texture = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
