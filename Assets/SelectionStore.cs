using UnityEngine;

public static class SelectionStore
{
    private const string Key = "SelectedCarId";

    public static int SelectedCarId
    {
        get => PlayerPrefs.GetInt(Key, 0);
        set { PlayerPrefs.SetInt(Key, value); PlayerPrefs.Save(); }
    }
}