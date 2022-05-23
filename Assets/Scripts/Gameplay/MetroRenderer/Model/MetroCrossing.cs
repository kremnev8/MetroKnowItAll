using System;
using System.Collections.Generic;
using System.Linq;
using Util;

namespace Gameplay.MetroDisplay.Model
{
    /// <summary>
    /// Defines a crossing between lines
    /// </summary>
    [Serializable]
    public class MetroCrossing : INamedArrayElement
    {
        public List<GlobalId> stationsGlobalIds;

        public bool isAbove;

        public int openIn;
        public int closedIn;

        public bool IsOpen(int year)
        {
            return year >= openIn && year < closedIn;
        }

        public string editorName
        {
            get
            {
#if UNITY_EDITOR
                if (MetroRenderer.instance != null)
                {
                    List<string> names = stationsGlobalIds.Select(id => MetroRenderer.instance.metro.GetStation(id).currentName).ToList();
                    return String.Join(", ", names);
                }
#endif

                return "Переход (Ошибка)";
            }
        }

        public string displayName => editorName;
    }
}