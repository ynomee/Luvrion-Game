using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

[System.Serializable]
public struct SaveData
{
    public static SaveData Instance;
    public HashSet<string> sceneNames;

    public void Initialize()
    {
        if (sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }
}
