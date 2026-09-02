using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<int> StepCompleted;
    public static event Action<int> ValidQRScanned;

    public static void RaiseStepCompleted(int stepNum)
    { StepCompleted?.Invoke(stepNum); }

    public static void RaiseValidQRScanned(int qrID)
    { ValidQRScanned?.Invoke(qrID); }
}