using UnityEngine;
using static UnityEditor.PrefabUtility;

namespace TrueSim.Runtime.CCiC.CharacterTemplates
{
    public abstract class CharacterAssetGenerator : ScriptableObject
    {
        public abstract void Apply(EditPrefabContentsScope contentScope, string characterName, string characterAssetPath);
    }
}
