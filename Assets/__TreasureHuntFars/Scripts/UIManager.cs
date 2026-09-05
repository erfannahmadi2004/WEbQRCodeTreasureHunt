using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using Unity.Android.Gradle.Manifest;
using System.Linq;

[Serializable]
public class UITextDefinition
{
    public string statusDesired;
    public string collectedText;
    public int stepNumber;
    public GameObject stepText;
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

    public int numberofProgress;
    
    private UITextDefinition detectedui;

    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closeSprite;

    private void Start()
    {
        if (PlayerPrefs.GetString("phone") != "")
        {
            chestButton.gameObject.SetActive(true);
            statusText.text = "راز آرامگاه ها";
            string data = PlayerPrefs.GetString("progress");
            numberofProgress = data.Count(char.IsDigit);
            if (numberofProgress == UIDefs.Count)
            GameEvents.RaiseMissionComplete();
            UpdateProgress();
        }
    }
    private void OnEnable()
    {
        GameEvents.ReqiredQRScanned +=UpdateUItexts;
        GameEvents.MissionComplete += UpdateUiComplete;
        GameEvents.StepCompleted += UpdateProgress;
    }

    private void OnDisable()
    {
        GameEvents.ReqiredQRScanned -=UpdateUItexts;
        GameEvents.MissionComplete -= UpdateUiComplete;
        GameEvents.StepCompleted -= UpdateProgress;
    }

    private void UpdateUItexts(int qrID)
    {
        foreach (UITextDefinition uiDef in UIDefs )
        {
            if(uiDef.stepNumber == qrID)
            {
                numberofProgress ++;
                detectedui = uiDef;
                StartCoroutine(StatusTExtTansition(uiDef.statusDesired));
                switch (qrID)
                {
                    case 0 :
                        ScenarioOne();
                        break;
                    default:
                        GameEvents.RaiseTextShowed(detectedui.stepNumber);
                        break;
                }
                return;
            }
        }
    }

    private void UpdateProgress()
    {
        string theText = 
        $"پیشرفت :" +
        $"{numberofProgress-1}/{UIDefs.Count-1}";
        StartCoroutine(userTextTansition(theText));
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
            pos.x = Mathf.Lerp(currentPos.x, -currentPos.x * 5, t);
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
            pos.x = Mathf.Lerp(-currentPos.x * 5, currentPos.x, t);
            statusText.transform.position = pos;

            yield return null;
        }
    }

    private IEnumerator userTextTansition (string theText)
    {
        float elapsed = 0f;

        float delay = 0.5f;

        Vector3 currentPos = userText.transform.position;

        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / delay);

            Vector3 pos = userText.transform.position;
            pos.x = Mathf.Lerp(currentPos.x, -currentPos.x * 5, t);
            userText.transform.position = pos;

            yield return null;
        }

        userText.text = theText;

        elapsed = 0f;

        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / delay);

            Vector3 pos = userText.transform.position;
            pos.x = Mathf.Lerp(-currentPos.x * 5, currentPos.x, t);
            userText.transform.position = pos;

            yield return null;
        }
    }

    private void UpdateUiComplete()
    {
        chestButton.image.sprite = closeSprite;
        StartCoroutine(StatusTExtTansition("ماموریت با موفقیت انجام شد :)"));
        chestButton.interactable = false;
    }

    public void OnCloseButtonPressed()
    {
        StartCoroutine(StatusTExtTansition(detectedui.collectedText));
        GameEvents.RaiseStepCompleted();
    }

    private void ScenarioOne()
    {
        detectedui.stepText.SetActive(true);
        StartCoroutine(ObjectComeIn(detectedui.stepText));
    }

    private IEnumerator ObjectComeIn(GameObject textObject)
    {
        float elapsed=0f;

        float inTime = 0.5f;

        Vector3 initialPose = textObject.transform.position;

        while (elapsed < inTime)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / inTime);

            Vector3 pos = textObject.transform.position;
            pos.x = Mathf.Lerp(initialPose.x * -3, initialPose.x, t);
            textObject.transform.position = pos;

            yield return null;
        }

    }

    private IEnumerator ObjectGetOut(GameObject textObject)
    {
        float elapsed=0f;

        float inTime = 0.5f;

        Vector3 initialPose = textObject.transform.position;

        while (elapsed < inTime)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / inTime);

            Vector3 pos = textObject.transform.position;
            pos.x = Mathf.Lerp(initialPose.x, initialPose.x * -3, t);
            textObject.transform.position = pos;

            yield return null;
        }

        Destroy(textObject);
    }

    public void CheckNumber(TMP_InputField phoneNum)
    {
        string phone = phoneNum.text.Trim();

        if (Regex.IsMatch(phone, @"^9\d{9}$"))
        {
            Debug.Log("Valid phone number");
            GameEvents.RaiseTextShowed(detectedui.stepNumber);
            StartCoroutine(ObjectGetOut(detectedui.stepText));
            StartCoroutine(userTextTansition(
                $"ثبت نام با موفقیت انجام شد\n"+
                $"بعد از به دست آوردن هر دستاورد روی صندوق میراث بزنید"));
            PlayerPrefs.SetString("phone", phone);
        }
        else
        {
            Debug.Log("Please enter a valid phone number.");
            StartCoroutine(userTextTansition("شماره نامعتبر است"));
        }
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
    }

}
