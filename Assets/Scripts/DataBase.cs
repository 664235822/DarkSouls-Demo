using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase
{
    private const string weaponDataBaseFileName = "weaponData";

    public readonly JSONObject weaponJson;

    public DataBase()
    {
        TextAsset weaponText = Resources.Load(weaponDataBaseFileName) as TextAsset;
        weaponJson = new JSONObject(weaponText.text);
    }
}
