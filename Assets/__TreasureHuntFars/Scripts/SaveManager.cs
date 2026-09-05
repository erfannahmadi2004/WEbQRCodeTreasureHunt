using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string IS_STARTED_KEY = "IsStarted";
    private const string PHONE_NUMBER_KEY = "PhoneNumber";
    private const string PROGRESS_KEY = "Progress";
    private const string COLLECTED_PIECES_KEY = "CollectedPieces";


    public static SaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // =========================================================
    // SAVE
    // =========================================================

    public void SaveGame(
        bool isStarted,
        string phoneNumber,
        int progress,
        HashSet<int> collectedPieces)
    {
        PlayerPrefs.SetInt(
            IS_STARTED_KEY,
            isStarted ? 1 : 0
        );

        PlayerPrefs.SetString(
            PHONE_NUMBER_KEY,
            phoneNumber ?? ""
        );

        PlayerPrefs.SetInt(
            PROGRESS_KEY,
            progress
        );

        // Convert HashSet<int> to string
        // Example: 0,1,3,5
        string collectedString = string.Join(
            ",",
            collectedPieces
        );

        PlayerPrefs.SetString(
            COLLECTED_PIECES_KEY,
            collectedString
        );

        // IMPORTANT for WebGL
        PlayerPrefs.Save();

        Debug.Log("Game saved!");
    }

    // =========================================================
    // LOAD
    // =========================================================

    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey(IS_STARTED_KEY);
    }

    public bool LoadGame(
        out bool isStarted,
        out string phoneNumber,
        out int progress,
        out HashSet<int> collectedPieces)
    {
        isStarted = false;
        phoneNumber = "";
        progress = 0;
        collectedPieces = new HashSet<int>();

        if (!HasSaveData())
        {
            Debug.Log("No save data found.");
            return false;
        }

        isStarted =
            PlayerPrefs.GetInt(IS_STARTED_KEY, 0) == 1;

        phoneNumber =
            PlayerPrefs.GetString(
                PHONE_NUMBER_KEY,
                ""
            );

        progress =
            PlayerPrefs.GetInt(
                PROGRESS_KEY,
                0
            );

        string collectedString =
            PlayerPrefs.GetString(
                COLLECTED_PIECES_KEY,
                ""
            );

        if (!string.IsNullOrEmpty(collectedString))
        {
            string[] values =
                collectedString.Split(',');

            foreach (string value in values)
            {
                if (int.TryParse(value, out int id))
                {
                    collectedPieces.Add(id);
                }
            }
        }

        Debug.Log(
            $"Game loaded | Started: {isStarted} | " +
            $"Phone: {phoneNumber} | " +
            $"Progress: {progress} | " +
            $"Collected: {collectedPieces.Count}"
        );

        return true;
    }

    // =========================================================
    // DELETE SAVE
    // =========================================================

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(IS_STARTED_KEY);
        PlayerPrefs.DeleteKey(PHONE_NUMBER_KEY);
        PlayerPrefs.DeleteKey(PROGRESS_KEY);
        PlayerPrefs.DeleteKey(COLLECTED_PIECES_KEY);

        PlayerPrefs.Save();

        Debug.Log("Save data deleted.");
    }
}