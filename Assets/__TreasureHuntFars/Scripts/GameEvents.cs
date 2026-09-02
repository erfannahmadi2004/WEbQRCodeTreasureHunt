using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<int> StepCompleted;
    public static event Action<int> ValidQRScanned;
    public static event Action<int> ReqiredQRScanned;
    public static event Action<int> InProgress;

    public static void RaiseStepCompleted(int stepNum)
    { StepCompleted?.Invoke(stepNum); }

    public static void RaiseValidQRScanned(int qrID)
    { ValidQRScanned?.Invoke(qrID); }

    public static void RaiseRequiredQRScanned(int qrID)
    { ReqiredQRScanned?.Invoke(qrID); }

    public static void RaiseInProgress(int qrID)
    { InProgress?.Invoke(qrID); }
}