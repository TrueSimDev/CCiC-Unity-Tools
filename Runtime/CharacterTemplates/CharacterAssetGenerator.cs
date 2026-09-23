using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEditor.PrefabUtility;

namespace TrueSim.Runtime.CCiC.CharacterTemplates
{
    public abstract class CharacterAssetGenerator : SerializedScriptableObject
    {
        public abstract void Apply(EditPrefabContentsScope contentScope, string characterName, string characterAssetPath);
    }
}
