using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public bool isStarted = false;
    private readonly HashSet<int> collectedPieces = new();

    [SerializeField]
    private QRCodeScanner qRCodeScanner;


    private void OnEnable()
    {
        GameEvents.ValidQRScanned += ScanningState;
        GameEvents.InProgress += InProgressState;
        GameEvents.GotInChest += StepCompleted;
    }

    private void OnDisable()
    {
        GameEvents.ValidQRScanned -= ScanningState;
        GameEvents.InProgress -= InProgressState;
        GameEvents.GotInChest += StepCompleted;
    }

    private void ScanningState(int qrID)
    {
        Debug.Log("[qr] it runs");
        if (isStarted)
        {
            bool isNew = collectedPieces.Add(qrID);
            if (!isNew)
            {
                Debug.Log("Duplicated Piece");
                return;
            }
            Debug.Log("Mission Started");
            GameEvents.RaiseRequiredQRScanned(qrID);

        }
        else if (qrID == 0)
        {
            Debug.Log("Started QR Scanned and Mission Started!");
            isStarted = true;
            collectedPieces.Clear();
            collectedPieces.Add(qrID);
            GameEvents.RaiseRequiredQRScanned(qrID);
        }
        else
        {
            Debug.Log("Start QR not scanned yet");
        }
    }

    private void InProgressState(int ID)
    {
        
    }

    private void StepCompleted()
    {
        if (collectedPieces.Count >= qRCodeScanner.TotalQRCodeCount)
            {
                Debug.Log("Everything is found and done");
                GameEvents.RaiseMissionComplete();
                return;
            }
    }
}
