using System.Collections.Generic;
using Gameplay.MetroDisplay.Model;
using Gameplay.Statistics;
using Newtonsoft.Json.Utilities;
using ScriptableObjects;
using UnityEngine;

namespace Util
{
    public class AotTypeEnforcer : MonoBehaviour
    {
        public void Awake()
        {
            AotHelper.EnsureList<int>();
            AotHelper.EnsureList<string>();
            AotHelper.EnsureDictionary<string, bool>();
            AotHelper.EnsureType<BoolRecord<MetroStation, int>>();
            AotHelper.EnsureType<BoolRecord<MetroLine, int>>();
            AotHelper.EnsureType<BoolRecord<Achievement, string>>();
        }
    }
}