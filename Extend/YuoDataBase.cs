using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YuoValue
{
    public float Basis;
    public float BasisPercentage;
    public float Additional;
    public float AdditionalPercentage;
    public float FinalPercentage;

    public List<YuoValue> RelationValues = new List<YuoValue>();

    void UpdateValue()
    {
        
        foreach (var value in RelationValues)
        {
            value.OnUpdateValue();
        }
    }

    void OnUpdateValue()
    {
    }
}