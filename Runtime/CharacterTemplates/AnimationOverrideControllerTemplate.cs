using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using static UnityEditor.PrefabUtility;
#endif


namespace TrueSim.Runtime.CCiC.CharacterTemplates
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "Animation Override Controller Template", menuName = "TrueSim/CCiC/Animation Override Controller Template")]
    public class AnimationOverrideControllerTemplate : ScriptableObject
    {
        [SerializeField] private RuntimeAnimatorController _source;
        [SerializeField, FolderPath] private string _destinationPath;

        public void Apply(EditPrefabContentsScope assetPath, string controllerSuffix)
        {
                var overrideController = new AnimatorOverrideController(_source);
                var controllerPath = $"{_destinationPath}/{_source.name}_{controllerSuffix}.overrideController";
                var animator = assetPath.prefabContentsRoot.GetComponentInChildren<Animator>();
                animator.runtimeAnimatorController = overrideController;
                AssetDatabase.CreateAsset(overrideController, controllerPath);
        }
    }
#endif
}
