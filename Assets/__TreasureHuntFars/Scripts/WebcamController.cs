using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class CameraManager : MonoBehaviour
{
    [Header("Camera Display")]
    [SerializeField] private RawImage displayImage;

    [Header("Camera Settings")]
    [SerializeField] private int requestedWidth = 1280;
    [SerializeField] private int requestedHeight = 720;
    [SerializeField] private int requestedFPS = 30;

    private WebCamTexture webcamTexture;
    private bool isInitializing;

    public WebCamTexture WebcamTexture => webcamTexture;
    public bool IsCameraPlaying => webcamTexture != null && webcamTexture.isPlaying;

    private void Start()
    {
        StartCamera();
    }

    public void StartCamera()
    {
        if (isInitializing || IsCameraPlaying)
            return;

        isInitializing = true;

#if UNITY_IOS || UNITY_WEBGL
        StartCoroutine(
            AskForPermissionIfRequired(
                UserAuthorization.WebCam,
                InitializeCamera
            )
        );

#elif UNITY_ANDROID

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            AskCameraPermission();
            return;
        }

        InitializeCamera();

#else
        InitializeCamera();
#endif
    }

    // =========================================================
    // WEBGL / IOS PERMISSION
    // =========================================================

#if UNITY_IOS || UNITY_WEBGL

    private bool CheckPermissionAndRaiseCallbackIfGranted(
        UserAuthorization authenticationType,
        Action authenticationGrantedAction)
    {
        if (Application.HasUserAuthorization(authenticationType))
        {
            authenticationGrantedAction?.Invoke();
            return true;
        }

        return false;
    }

    private IEnumerator AskForPermissionIfRequired(
        UserAuthorization authenticationType,
        Action authenticationGrantedAction)
    {
        if (!CheckPermissionAndRaiseCallbackIfGranted(
                authenticationType,
                authenticationGrantedAction))
        {
            yield return Application.RequestUserAuthorization(authenticationType);

            if (!CheckPermissionAndRaiseCallbackIfGranted(
                    authenticationType,
                    authenticationGrantedAction))
            {
                Debug.LogWarning(
                    $"Permission {authenticationType} Denied"
                );

                isInitializing = false;
            }
        }
    }

#endif

    // =========================================================
    // ANDROID PERMISSION
    // =========================================================

#if UNITY_ANDROID

    private void AskCameraPermission()
    {
        PermissionCallbacks callbacks = new PermissionCallbacks();

        callbacks.PermissionGranted +=
            PermissionCallbacksPermissionGranted;

        callbacks.PermissionDenied +=
            PermissionCallbacksPermissionDenied;

        Permission.RequestUserPermission(
            Permission.Camera,
            callbacks
        );
    }

    private void PermissionCallbacksPermissionGranted(
        string permissionName)
    {
        StartCoroutine(DelayedCameraInitialization());
    }

    private IEnumerator DelayedCameraInitialization()
    {
        yield return null;

        InitializeCamera();
    }

    private void PermissionCallbacksPermissionDenied(
        string permissionName)
    {
        Debug.LogWarning(
            $"Permission {permissionName} Denied"
        );

        isInitializing = false;
    }

#endif

    // =========================================================
    // CAMERA INITIALIZATION
    // =========================================================

    private void InitializeCamera()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            isInitializing = false;
            return;
        }

        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("No webcam found on this device.");

            isInitializing = false;
            return;
        }

        Debug.Log($"Found {devices.Length} camera(s).");

        // Use the default camera
        string cameraName = devices[1].name;

        Debug.Log($"Using camera: {cameraName}");

        webcamTexture = new WebCamTexture(
            cameraName,
            requestedWidth,
            requestedHeight,
            requestedFPS
        );

        // Assign to RawImage
        if (displayImage != null)
        {
            displayImage.texture = webcamTexture;
        }

        webcamTexture.Play();

        isInitializing = false;

        Debug.Log("Camera started.");
    }

    // =========================================================
    // STOP CAMERA
    // =========================================================

    public void StopCamera()
    {
        if (webcamTexture == null)
            return;

        if (webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
            Debug.Log("Camera stopped.");
        }
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        StopCamera();

        if (webcamTexture != null)
        {
            Destroy(webcamTexture);
            webcamTexture = null;
        }
    }
}