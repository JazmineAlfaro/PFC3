using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Variables;
public class WebCam : MonoBehaviour {

    private WebCamDevice _cam;
    //public WebCamTexture _texture;
    public TextMeshProUGUI t;
    public RawImage Display;

    private int _currentCamIndex = 0;
    

	void Start () {
        Init();
	}

    private void Init() {
        SetCam(0);
    }

    void Update()
    {

        /* 
        t.text = _texture.videoRotationAngle.ToString();
        int orientation = _texture.videoRotationAngle;
        Display.rectTransform.localEulerAngles = new Vector3(.0f, .0f, -orientation);
        */ 

        t.text = Variables.Texture.videoRotationAngle.ToString();
        int orientation = Variables.Texture.videoRotationAngle;
        Display.rectTransform.localEulerAngles = new Vector3(.0f, .0f, -orientation);
    }
    public void StartCam() {
        if (!HasCam()) {
            return;
        }

        Variables.Texture = new WebCamTexture(_cam.name);
        //_texture = new WebCamTexture(_cam.name);
        //Display.texture = _texture;
        Display.texture = Variables.Texture;
        //_texture.Play();
        Variables.Texture.Play();
    }

    public void StopCam() {
        /*
         if (_texture == null) {
             return;
         }

         Display.texture = null;
         _texture.Stop();
         _texture = null;
        */

        

        if (Variables.Texture == null)
        {
            return;
        }

        Display.texture = null;
        Variables.Texture.Stop();
        Variables.Texture = null;
    }

    
    public void PrevCam() {
        if (!HasCam()) {
            return;
        }

        _currentCamIndex--;
        if (_currentCamIndex < 0) {
            _currentCamIndex = 0;
        }
        SetCam(_currentCamIndex);

        StopCam();
        StartCam();
    }

    public void NextCam() {
        if (!HasCam()) {
            return;
        }

        _currentCamIndex++;
        if (_currentCamIndex >= WebCamTexture.devices.Length) {
            _currentCamIndex = WebCamTexture.devices.Length - 1;
        }

        SetCam(_currentCamIndex);

        StopCam();
        StartCam();
    }

    
    private void SetCam(int camIndex) {
        if (!HasCam()) {
            return;
        }

        _cam = WebCamTexture.devices[camIndex];
    }

    private bool HasCam() {
        return WebCamTexture.devices.Length > 0;
    }

}