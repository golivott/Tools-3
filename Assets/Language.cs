using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[Serializable]
public class Language : ScriptableObject
{
    public SerializedDictionary<string,string> dict;
}