using UnityEngine;

[CreateAssetMenu(fileName = "Mineral", menuName = "ScriptableObjects/MineralScriptableObject", order =1)]
public class MineralScriptableObject : ScriptableObject
{
    public string mineralName;
    public Sprite mineralSprite;
    public Color spriteColor = Color.white;
    
    [Header("Attributes")]
    [Tooltip("Based off Moh's Scale")]
    public int hardness;
    
    [Tooltip("1 = Cubic\n" +
             "2 = Tetragonal\n" +
             "3 = Hexagonal\n" +
             "4 = Rhombohedral\n" +
             "5 = Orthorhombic\n" +
             "6 = Monoclinic\n" +
             "7 = Triclinic\n")]
    [Range(1, 7)] public int crystalStructure;
    
    [Tooltip("0 = White\n" +
             "1 = Red\n" +
             "2 = Orange\n" +
             "3 = Yellow\n" +
             "4 = Green\n" +
             "5 = Blue\n" +
             "6 = Indigo\n" +
             "7 = Violet\n")]
    [Range(0, 7)] public int color;
    
    [Header("Animation")]
    public RuntimeAnimatorController animatorController; // each mineral's shine animation
}
