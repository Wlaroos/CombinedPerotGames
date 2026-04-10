using UnityEngine;
[CreateAssetMenu(fileName = "SortingRule", menuName = "ScriptableObjects/SortingRule", order = 2)]
public class SortingRule : ScriptableObject
{
    public enum AttributeType {Hardness, crystalStructure}
    public AttributeType attribute;
    public bool ascending; // true = low to high, false = high to low
}
