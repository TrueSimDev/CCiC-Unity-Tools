using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TrueSim.Runtime.CCiC.CharacterTemplates
{
    [CreateAssetMenu(fileName = "Animation Override Controller Template", menuName = "TrueSim/CCiC/Animation Override Controller Template")]
    public class AnimationOverrideControllerTemplate : ScriptableObject
    {
        [SerializeField] private RuntimeAnimatorController _source;
        [SerializeField, FolderPath] private string _destinationPath;

        public void Apply(GameObject targetObject, string controllerSuffix)
        {
            var overrideController = new AnimatorOverrideController(_source);
            var controllerPath = $"{_destinationPath}/{_source.name}_{controllerSuffix}.overrideController";
            var animator = targetObject.GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = overrideController;
            AssetDatabase.CreateAsset(overrideController, controllerPath);
        }
    }
}
