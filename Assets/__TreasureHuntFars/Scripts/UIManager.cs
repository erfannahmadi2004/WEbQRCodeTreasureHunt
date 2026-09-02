using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Video;

[Serializable]
public class UITextDefinition
{
    public string statusDesired;
    public int stepNumber;
}

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private List<UITextDefinition> UIDefs = new();

    [SerializeField]
    private Button chestButton;
    [SerializeField]
    private TMP_Text statusText;
    [SerializeField]
    private TMP_Text userText;

    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closeSprite;

    private void OnEnable()
    {
        GameEvents.ReqiredQRScanned +=UpdateUItexts;
        GameEvents.MissionComplete += UpdateUiComplete;
    }

    private void OnDisable()
    {
        GameEvents.ReqiredQRScanned -=UpdateUItexts;
        GameEvents.MissionComplete -= UpdateUiComplete;
    }

    private void UpdateUItexts(int qrID)
    {
        foreach (UITextDefinition uiDef in UIDefs )
        {
            if(qrID == 0)
            chestButton.image.sprite = openSprite;
            if(uiDef.stepNumber == qrID)
            {
            statusText.text = uiDef.statusDesired;
            return;
            }
        }
    }

    private void UpdateUiComplete()
    {
        chestButton.image.sprite = closeSprite;
        statusText.text = "ماموریت با موفقیت انجام شد :)";
    }

    public void OnCloseButtonPressed()
    {
        GameEvents.RaiseStepCompleted();
    }

}
