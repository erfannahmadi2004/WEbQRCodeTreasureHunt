using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUI : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    private RectTransform rectTransform;
    private RawImage rawImage;
    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Vector3 chestInitialScale;
    private Color initialColor;

    private float scaleMulty = 1.2f;
    private float centerDuration = 1;

    [SerializeField]
    private GameObject theChest;

    // -------------------------
    // CLICK / DRAG DETECTION
    // -------------------------

    private Vector2 pointerDownPosition;
    private bool isDragging;

    // How much the pointer must move before it counts as a drag
    [SerializeField]
    private float dragThreshold = 5f;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rawImage = GetComponent<RawImage>();
    }

    private void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
        chestInitialScale = theChest.transform.localScale;
        initialColor = rawImage.color;
    }

    private void OnEnable()
    {
        GameEvents.InProgress += ResetPosition;
        GameEvents.StepCompleted += GetInChest;
    }

    private void OnDisable()
    {
        GameEvents.InProgress -= ResetPosition;
        GameEvents.StepCompleted -= GetInChest;
    }


    // -------------------------
    // POINTER DOWN
    // -------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownPosition = eventData.position;
        isDragging = false;
    }


    // -------------------------
    // DRAG
    // -------------------------

    public void OnDrag(PointerEventData eventData)
    {
        // Check whether the pointer has moved far enough
        // to actually count as a drag.

        float distance = Vector2.Distance(
            pointerDownPosition,
            eventData.position
        );

        if (distance >= dragThreshold)
        {
            isDragging = true;
        }

        // Don't move the object until it is actually a drag
        if (!isDragging)
            return;

        rectTransform.position = eventData.position;
    }


    // -------------------------
    // POINTER UP
    // -------------------------

    public void OnPointerUp(PointerEventData eventData)
    {
        // If we didn't drag, this is a CLICK.
        if (!isDragging)
        {
            GameEvents.RaiseObjectTap();
        }

        isDragging = false;
    }


    // -------------------------
    // RESET
    // -------------------------

    private void ResetPosition(int num)
    {
        transform.position = initialPosition;
        transform.localScale = initialScale;
        rawImage.color = initialColor;
    }


    // -------------------------
    // GET IN CHEST
    // -------------------------

    private void GetInChest()
    {
        StartCoroutine(GetInCenterChest());
        StartCoroutine(ChestGetsBig());
    }


    private IEnumerator GetInCenterChest()
    {
        float elapsed = 0f;

        Vector3 currentPos = transform.position;

        while (elapsed < centerDuration / 4)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / (centerDuration / 4)
            );

            transform.position = Vector3.Lerp(
                currentPos,
                initialPosition,
                t
            );

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < centerDuration / 2)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / (centerDuration / 2)
            );

            transform.localScale =
                Vector3.Lerp(
                    initialScale,
                    initialScale * scaleMulty,
                    t
                );

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < centerDuration / 2)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / (centerDuration / 2)
            );

            transform.localScale =
                Vector3.Lerp(
                    initialScale * scaleMulty,
                    initialScale,
                    t
                );

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < centerDuration / 4)
        {
            elapsed += Time.deltaTime;

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < centerDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / centerDuration
            );

            transform.position =
                Vector3.Lerp(
                    initialPosition,
                    theChest.transform.position,
                    t
                );

            transform.localScale =
                Vector3.Lerp(
                    initialScale * scaleMulty,
                    Vector3.zero,
                    t
                );
            Color color = initialColor;
            color.a = Mathf.Lerp(initialColor.a, 0f, t);
            rawImage.color = color;
            

            yield return null;
        }
    }


    private IEnumerator ChestGetsBig()
    {
        Vector3 startScale;
        float elapsed = 0f;

        while (elapsed < centerDuration * 2.3)
        {
            elapsed += Time.deltaTime;

            yield return null;
        }

        startScale = chestInitialScale;


        if (theChest.activeSelf == false)
        {
            startScale = Vector3.zero;
            theChest.SetActive(true);
        }

        elapsed = 0f;

        while (elapsed < centerDuration / 2)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / (centerDuration / 2)
            );


            theChest.transform.localScale =
                Vector3.Lerp(
                    startScale,
                    chestInitialScale * scaleMulty,
                    t
                );

            yield return null;
        }

        GameEvents.RaiseGotInChest();

        elapsed = 0f;

        while (elapsed < centerDuration / 2)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / (centerDuration / 2)
            );

            theChest.transform.localScale =
                Vector3.Lerp(
                    chestInitialScale * scaleMulty,
                    chestInitialScale,
                    t
                );

            yield return null;
        }
    }
}