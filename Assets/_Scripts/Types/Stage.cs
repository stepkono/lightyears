using System;
using JetBrains.Annotations;
using UnityEngine;

namespace _Scripts.Types
{
    public class Stage
    {
        public string Name { get; private set; }
        public float HoursRequired { get; private set; }
        public GameObject PlanetModel { get; private set; }
        public int StageIndex { get; private set; }
        public bool Evo { get; private set; }

        public Stage(string name, float hoursRequired, int stageIndex, bool evo)
        {
            Name = name;
            HoursRequired = hoursRequired;
            StageIndex = stageIndex;
            Evo = evo;
        }
        public Stage(string name, int stageIndex, bool evo)
        {
            Name = name;
            StageIndex = stageIndex;
            Evo = evo;
        }

        public void SetModel(GameObject model)
        {
            PlanetModel = model;
        }
    }
}