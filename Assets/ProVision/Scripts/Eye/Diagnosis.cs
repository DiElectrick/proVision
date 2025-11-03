
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Diseases { 
    Stigmatizm_plus,
    Stigmatizm_minus,
    Capillaries,
    Vitamins,
    Rotation,
    Scale
}

//[CreateAssetMenu(fileName = "Diagnosis", menuName = "Scriptable Objects/Diagnosis")]
[System.Serializable]
public class Diagnosis 
{
    public List<bool> diseases;

    public Diagnosis() {
        diseases = new List<bool>();
        for (int i = 0; i < Enum.GetNames(typeof(Diseases)).Length; i++)
        {
            diseases.Add(false);
        }
    }

    public Diagnosis(List<bool> diseases) {
        this.diseases = diseases;
    }
}
