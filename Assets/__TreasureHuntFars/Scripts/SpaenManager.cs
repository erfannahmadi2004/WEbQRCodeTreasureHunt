using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaenManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> prefabsToSpawn = new List<GameObject>();

    [SerializeField]
    private List<GameObject> particlesToSpawn = new List<GameObject>();

    [SerializeField]
    private QRCodeScanner qRCodeScanner;

    [SerializeField]
    private UIManager uIManager;

    private GameObject spawnedObject;

    private GameObject spawnedParticle;

    private float appearDuration = 0.5f;

    private int spawnedID;

    public bool isInProgress = false;

    private Vector3 spawnpose = new Vector3(0, 0, 2.5f);
    private Vector3 pSpawnpose = new Vector3(0, 0, 2f);

    public int trackableID;

    [Header("Animation")]
    [SerializeField]
    private float rotationSpeed = 60f;


    private void OnEnable()
    {
        GameEvents.ReqiredQRScanned += SpawnPrefab;
        GameEvents.GotInChest += ScenarioDone;
        GameEvents.ObjectTap += ObjectTapped;
    }

    private void OnDisable()
    {
        GameEvents.ReqiredQRScanned -= SpawnPrefab;
        GameEvents.GotInChest -= ScenarioDone;
        GameEvents.ObjectTap -= ObjectTapped;
    }

    private void SpawnPrefab(int qrID)
    {
        spawnedID = qrID;

        GameObject particlePrefab = particlesToSpawn[qrID];
        spawnedParticle = Instantiate(
            particlePrefab,
            pSpawnpose,
            Quaternion.identity
        );

        GameObject prefab = prefabsToSpawn[qrID];
        spawnedObject = Instantiate(
            prefab,
            spawnpose,
            Quaternion.Euler(-20, 40, -15)
        );

        StartCoroutine(GrowSpawnedObject());

        GameEvents.RaiseInProgress(qrID);
        isInProgress = true;
    }

    private IEnumerator GrowSpawnedObject()
    {
        Vector3 originalScale = spawnedObject.transform.localScale;

        float elapsed = 0f;

        spawnedObject.transform.localScale = Vector3.zero;

        while (elapsed < appearDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / appearDuration);

            spawnedObject.transform.localScale =
                Vector3.Lerp(Vector3.zero, originalScale, t);

            yield return null;
        }

        spawnedObject.transform.localScale = originalScale;
    }

    private void Update()
    {
        if (spawnedObject == null)
            return;

        spawnedObject.transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.Self);
    }

    private void ScenarioDone()
    {
        if (isInProgress)
            Destroy(spawnedObject);
        if (spawnedParticle != null)
            Destroy(spawnedParticle);
    }

    private void ObjectTapped()
    {
        if (spawnedID == 0)
        uIManager.OnCloseButtonPressed();
        else
        {
            GameObject particlePrefab = particlesToSpawn[spawnedID];
            spawnedParticle = Instantiate(
            particlePrefab,
            pSpawnpose,
            Quaternion.identity);
        }
    }
}
