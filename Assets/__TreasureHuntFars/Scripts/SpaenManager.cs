using System.Collections.Generic;
using UnityEngine;

public class SpaenManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> prefabsToSpawn = new List<GameObject>();

    [SerializeField]
    private QRCodeScanner qRCodeScanner;

    private GameObject spawnedObject;

    public bool isInProgress = false;

    private Vector3 spawnpose = new Vector3 (0,0,2.5f);

    public int trackableID;

    [Header("Animation")]
    [SerializeField]
    private float rotationSpeed = 60f;


    private void OnEnable()
    {
        GameEvents.ReqiredQRScanned += SpawnPrefab;
        GameEvents.StepCompleted += ScenarioDone;
    }

    private void OnDisable()
    {
        GameEvents.ReqiredQRScanned -= SpawnPrefab;
        GameEvents.StepCompleted -= ScenarioDone;
    }

    private void SpawnPrefab(int qrID)
    {
        if (true)
        {
            GameObject toSpawnObject = prefabsToSpawn[qrID];
            spawnedObject = Instantiate(toSpawnObject, spawnpose, Quaternion.Euler(-20, 40, -15));
            GameEvents.RaiseInProgress(qrID);
            isInProgress = true;
        }
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
        if(isInProgress)
        Destroy(spawnedObject);
    }
}
