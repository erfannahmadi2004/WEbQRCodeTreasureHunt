using System.Collections.Generic;
using UnityEngine;

public class SpaenManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> prefabsToSpawn = new List<GameObject>();

    [SerializeField]
    private QRCodeScanner qRCodeScanner;

    private GameObject spawnedObject;

    private Vector3 spawnpose = new Vector3 (0,0,2.5f);

    public int trackableID;

    [Header("Animation")]
    [SerializeField]
    private float rotationSpeed = 60f;


    private void OnEnable()
    {
        GameEvents.ValidQRScanned += SpawnPrefab;
    }

    private void OnDisable()
    {
        GameEvents.ValidQRScanned -= SpawnPrefab;
    }

    private void SpawnPrefab(int qrID)
    {
        if (true)
        {
            GameObject toSpawnObject = prefabsToSpawn[qrID];
            spawnedObject = Instantiate(toSpawnObject, spawnpose, Quaternion.identity);
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
}
