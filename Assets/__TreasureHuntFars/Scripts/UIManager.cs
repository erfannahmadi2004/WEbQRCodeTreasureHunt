using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

[Serializable]
public class UITextDefinition
{
    public string statusDesired;
    public string collectedText;
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
    
    private UITextDefinition detectedui;

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
            if(uiDef.stepNumber == qrID)
            {
                detectedui = uiDef;
            StartCoroutine(StatusTExtTansition(uiDef.statusDesired));
            return;
            }
        }
    }

    private IEnumerator StatusTExtTansition (string theText)
    {
        float elapsed = 0f;

        float delay = 0.5f;

        Vector3 currentPos = statusText.transform.position;

        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / delay);

            Vector3 pos = statusText.transform.position;
            pos.x = Mathf.Lerp(currentPos.x, -500f, t);
            statusText.transform.position = pos;

            yield return null;
        }

        statusText.text = theText;

        elapsed = 0f;

        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / delay);

            Vector3 pos = statusText.transform.position;
            pos.x = Mathf.Lerp(-500, currentPos.x, t);
            statusText.transform.position = pos;

            yield return null;
        }
    }

    private void UpdateUiComplete()
    {
        chestButton.image.sprite = closeSprite;
        StartCoroutine(StatusTExtTansition("ماموریت با موفقیت انجام شد :)"));
    }

    public void OnCloseButtonPressed()
    {
        StartCoroutine(StatusTExtTansition(detectedui.collectedText));
        GameEvents.RaiseStepCompleted();
    }

}
