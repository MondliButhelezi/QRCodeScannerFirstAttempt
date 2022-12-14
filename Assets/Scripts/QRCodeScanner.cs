using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ZXing;

public class QRCodeScanner : MonoBehaviour
{
    [SerializeField]
    private RawImage _rawImagebackground; // run the what the camera sees on my phone

    [SerializeField]
    private AspectRatioFitter _aspectRatioFitter;

    [SerializeField]
    private TextMeshProUGUI _textOut;

    [SerializeField]
    private RectTransform _scanZone;

    // Check if handle if camera is available
    private bool _isCamAvailable;

    // Allow access to mobile camera
    private WebCamTexture _cameraTexture;

    // Start is called before the first frame update
    void Start()
    {
        SetUpCamera();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraRender();
    }

    // Setting up camera so it displays QR code contents
    private void SetUpCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            _isCamAvailable = false;
            return;
        }
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
            {
                _cameraTexture = new WebCamTexture(devices[i].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
            }
        }

        _cameraTexture.Play();
        _rawImagebackground.texture = _cameraTexture;
        _isCamAvailable = true;
    }

    // Setting up the QR Code scanner using exceptions when scanning passes or fails
    private void Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(_cameraTexture.GetPixels32(), _cameraTexture.width, _cameraTexture.height);

            if (result != null)
            {
                _textOut.text = result.Text;
            }
            else
            {
                _textOut.text = "Failed to read QR Code";
            }
        }
        catch
        {
            _textOut.text = "Failed in try";
        }
    }

    public void OnClickScan()
    {
        Scan();
    }

    private void UpdateCameraRender()
    {
        if (_isCamAvailable == false)
        {
            return;
        }
        float ratio = (float)_cameraTexture.width / (float)_cameraTexture.height;
        _aspectRatioFitter.aspectRatio = ratio;

        int orientation = -_cameraTexture.videoRotationAngle;
        _rawImagebackground.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
    }

    public void OpenURL(string link)
    {
        Application.OpenURL(link);
    }
}
