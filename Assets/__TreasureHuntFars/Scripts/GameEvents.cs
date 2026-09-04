using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action StepCompleted;
    public static event Action GotInChest;
    public static event Action ObjectTap;
    public static event Action<int> ValidQRScanned;
    public static event Action<int> ReqiredQRScanned;
    public static event Action<int> InProgress;
    public static event Action MissionComplete;

    public static void RaiseStepCompleted()
    { StepCompleted?.Invoke(); }

    public static void RaiseValidQRScanned(int qrID)
    { ValidQRScanned?.Invoke(qrID); }

    public static void RaiseRequiredQRScanned(int qrID)
    { ReqiredQRScanned?.Invoke(qrID); }

    public static void RaiseInProgress(int qrID)
    { InProgress?.Invoke(qrID); }

    public static void RaiseMissionComplete()
    { MissionComplete?.Invoke(); }

    public static void RaiseGotInChest()
    { GotInChest?.Invoke(); }

    public static void RaiseObjectTap()
    { ObjectTap?.Invoke(); }
}