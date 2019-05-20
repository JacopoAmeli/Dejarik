using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceNode : DejTree {

    public float[] chanceValues;
    public ChanceNode(float[] chanceValues)
    {
        this.chanceValues = chanceValues;
        this.childrenNodes = new List<DejTree>();
    }
    public float GetChanceForNode(RealNode node)
    {
        return chanceValues[this.childrenNodes.IndexOf(node)];
    }
}
