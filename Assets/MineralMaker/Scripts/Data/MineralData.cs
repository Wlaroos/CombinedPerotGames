using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Stores info about a mineral (used as a ScriptableObject)
[CreateAssetMenu(menuName = "Chemistry/Mineral Data")]
public class MineralData : ScriptableObject
{
    [Header("Mineral Info")]
    public string mineralName;         // Name of the mineral
    public string description;       // Description of the mineral
    public string funFact;       // Fun fact about the mineral
    [Header("Mineral Appearance")]
    public Sprite mineralSprite;       // Main sprite for the mineral
    public Sprite mineralBigSprite;    // Big version of the mineral sprite
    public Color32 defaultColor = new Color32(255, 255, 255, 255); // Default color
    [Header("Mineral Attributes")]
    [Tooltip("Hardness of the mineral Moh's Scale (1-10)")]
    [Range(1,10)]public int hardness;            // Hardness (1-10)

    [Tooltip("1=Cubic\n" +
             "2=Tetragonal\n" +
             "3=Orthorhombic\n" +
             "4=Hexagonal\n" +
             "5=Trigonal\n" +
             "6=Monoclinic\n" +
             "7=Triclinic")]

    [Range(1,7)]public int crystalStructure;       // Crystal structure type (1-7)
    [Header("Misc")]
    public bool isVariant = false;

    private void OnEnable()
    {
        // Set the mineral name to the asset's name
        mineralName = SOHelpers.StripCommonPrefix(this.name);
    }
}

#if UNITY_EDITOR
public class MineralRecipePostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string path in importedAssets)
        {
            // Try to load a MineralData at the imported path
            var mineral = AssetDatabase.LoadAssetAtPath<MineralData>(path);
            if (mineral == null) continue;

            // Build recipe name: replace "M_" prefix with "R_" when present, otherwise prefix with "R_"
            string mineralName = mineral.name;
            string recipeName = mineralName.StartsWith("M_") ? "R_" + mineralName.Substring(2) : "R_" + mineralName;

            // Target folder inside Resources
            string recipesFolder = "Assets/Resources/SOs/Recipes";

            // Ensure Resources/SOs/Recipes exists
            EnsureFolder("Assets/Resources");
            EnsureFolder("Assets/Resources/SOs");
            EnsureFolder(recipesFolder);

            string desiredPath = recipesFolder + "/" + recipeName + ".asset";

            // Skip if a recipe already exists at that path
            if (AssetDatabase.LoadAssetAtPath<CraftingRecipe>(desiredPath) != null) continue;

            var recipe = ScriptableObject.CreateInstance<CraftingRecipe>();
            recipe.name = recipeName;
            recipe.output = mineral;

            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(desiredPath);
            AssetDatabase.CreateAsset(recipe, uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath)) return;

        string parent = Path.GetDirectoryName(folderPath).Replace("\\", "/");
        string newFolderName = Path.GetFileName(folderPath);
        if (string.IsNullOrEmpty(parent)) parent = "Assets";
        // CreateFolder requires parent path and new folder name
        if (!AssetDatabase.IsValidFolder(parent))
        {
            // Recursively ensure parent exists
            EnsureFolder(parent);
        }
        AssetDatabase.CreateFolder(parent, newFolderName);
    }
}
#endif