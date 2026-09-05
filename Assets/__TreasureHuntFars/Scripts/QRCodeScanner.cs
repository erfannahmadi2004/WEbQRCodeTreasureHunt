using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

[Serializable]
public class QRCodeDefinition
{
    public string url;
    public string displayName;
    public int scenarioNum;
}

public class QRCodeScanner : MonoBehaviour
{
    [SerializeField]
    private float scanInterval = 1f;

    [SerializeField]
    private List<QRCodeDefinition> qrCodes = new();

    public int TotalQRCodeCount => qrCodes.Count;

    [SerializeField]
    private RawImage cameraRawImage;

    private BarcodeReader barcodeReader;

    private float scanTimer;

    [SerializeField]
    private bool isScanning = true;

    public bool IsScanning => isScanning;

    private void OnEnable()
    {
        GameEvents.ReqiredQRScanned += StopnumScanning;
        GameEvents.GotInChest += StartScanning;
        GameEvents.MissionComplete += StopScanning;      
    }

    private void OnDisable()
    {
        GameEvents.ReqiredQRScanned -= StopnumScanning;
        GameEvents.GotInChest -= StartScanning;
        GameEvents.MissionComplete -= StopScanning; 
    }

    public void StartScanning()
    {
        isScanning = true;
    }

    public void StopScanning()
    {
        isScanning = false;
    }

    public void StopnumScanning(int num)
    {
        isScanning = false;
    }

    private void Awake()
    {
        barcodeReader = new BarcodeReader
        {
            AutoRotate = false
        };
    }

     private void Update()
        {
            if (!isScanning)
                return;

            scanTimer -= Time.deltaTime;

            if (scanTimer > 0f)
                return;

            scanTimer = scanInterval;
            
            ProcessImage();
        }

    public void ProcessImage()
{
    Debug.Log("[QR] scanning ...");

    if (cameraRawImage == null)
    {
        Debug.LogError("[QR] RawImage is not assigned!");
        return;
    }

    WebCamTexture webcamTexture =
        cameraRawImage.texture as WebCamTexture;

    if (webcamTexture == null)
    {
        Debug.LogError("[QR] RawImage texture is not a WebCamTexture!");
        return;
    }

    if (!webcamTexture.isPlaying)
    {
        Debug.LogWarning("[QR] WebCamTexture is not playing!");
        return;
    }

    int width = webcamTexture.width;
    int height = webcamTexture.height;

    if (width <= 16 || height <= 16)
    {
        Debug.LogWarning(
            $"[QR] Invalid camera resolution: {width}x{height}"
        );
        return;
    }

    Color32[] pixels = webcamTexture.GetPixels32();

    var result = barcodeReader.Decode(
        pixels,
        width,
        height
    );

    if (result != null)
    {
        Debug.Log($"[QR] Detected: {result.Text}");

        QRCodeDefinition qrCode = FindQRCode(result.Text);

        if (qrCode != null)
        {
            Debug.Log(
                $"[QR] Detected scenario: {qrCode.scenarioNum}"
            );

            GameEvents.RaiseValidQRScanned(qrCode.scenarioNum);
        }
    }

    }

    private QRCodeDefinition FindQRCode(string rawValue)
    {
        foreach (QRCodeDefinition qrCode in qrCodes)
        {
            if (qrCode == null)
                continue;

            if (string.Equals(
                qrCode.url,
                rawValue,
                StringComparison.OrdinalIgnoreCase))
            {
                return qrCode;
            }
        }

        return null;
    }
}
