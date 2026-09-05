using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public bool isStarted = false;
    public readonly HashSet<int> collectedPieces = new();

    [SerializeField]
    private QRCodeScanner qRCodeScanner;

    private void Start()
    {
        if (PlayerPrefs.GetString("phone") != "")
        {
            collectedPieces.UnionWith(StringToHashSet(PlayerPrefs.GetString("progress")));
            isStarted = true;
        }
    }

    private void OnEnable()
    {
        GameEvents.ValidQRScanned += ScanningState;
        GameEvents.GotInChest += StepCompleted;
    }

    private void OnDisable()
    {
        GameEvents.ValidQRScanned -= ScanningState;
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
            string progressstate = HashSetToString(collectedPieces);
            PlayerPrefs.SetString("progress",progressstate);

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

    private void StepCompleted()
    {
        if (collectedPieces.Count >= qRCodeScanner.TotalQRCodeCount)
            {
                Debug.Log("Everything is found and done");
                GameEvents.RaiseMissionComplete();
                return;
            }
    }

    public string HashSetToString(HashSet<int> set)
    {
        return string.Join(",", set);
    }

    public HashSet<int> StringToHashSet(string data)
    {
        if (string.IsNullOrEmpty(data))
            return new HashSet<int>();

        return new HashSet<int>(
            data.Split(',').Select(int.Parse)
        );
    }
}

