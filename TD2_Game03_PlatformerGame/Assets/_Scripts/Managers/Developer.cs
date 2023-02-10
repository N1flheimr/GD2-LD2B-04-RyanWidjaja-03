using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;

namespace NifuDev
{
    public class Developer
    {
        // [MenuItem("Developer/Clear Saves")]
        public static void ClearSaves()
        {
            PlayerPrefs.DeleteAll();

            Debug.Log("All Save have been deleted");
        }
    }
}

