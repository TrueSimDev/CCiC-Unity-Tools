using UnityEngine;
using UnityEngine.Animations.Rigging;
#if UNITY_EDITOR
using UnityEditor;
using static UnityEditor.PrefabUtility;
#endif

namespace TrueSim.Runtime.CCiC.CharacterTemplates
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "Twist Bone Template", menuName = "TrueSim/CCiC/Twist Bone Template")]
    public class TwistBoneTemplate : ScriptableObject
    {
        [SerializeField] private GameObject _twistRigPrefab;
        [SerializeField] private TwistBoneReference[] _twistBoneReferences;

        public void Apply(EditPrefabContentsScope contentScope)
        {
                var characterRoot = contentScope.prefabContentsRoot;
                var rigBuilder = characterRoot.AddComponent<RigBuilder>();
                var twistRigInstance = PrefabUtility.InstantiatePrefab(_twistRigPrefab, characterRoot.transform) as GameObject;
                for (int i = 0; i < _twistBoneReferences.Length; i++)
                {
                    var target = FindChildByRecursion(twistRigInstance.transform, _twistBoneReferences[i].TwistBoneName);
                    if (target == null)
                    {
                        continue;
                    }
                    _twistBoneReferences[i].Bind(characterRoot, target.GetComponent<TwistCorrection>());
                }
                rigBuilder.layers.Add(new RigLayer(twistRigInstance.GetComponent<Rig>())); 
        }

        private Transform FindChildByRecursion(Transform aParent, string aName)
        {
            if (aParent == null) return null;
            var result = aParent.Find(aName);
            if (result != null)
                return result;
            foreach (Transform child in aParent)
            {
                result = FindChildByRecursion(child, aName);
                if (result != null)
                    return result;
            }
            return null;
        }
    }

    [System.Serializable]
    public class TwistBoneReference
    {
        public string TwistBoneName => _twistBoneName;

        [SerializeField] private string _twistBoneName;
        [SerializeField] private string[] _twistNodeNames;
        [SerializeField] private string _sourceName;

        public void Bind(GameObject targetObject, TwistCorrection twistCorrection)
        {
            var twistNodes = twistCorrection.data.twistNodes;
            for(int i = 0; i < _twistNodeNames.Length; i++)
            {
                twistNodes.SetTransform(i, FindChildByRecursion(targetObject.transform, _twistNodeNames[i]));
            }
            twistCorrection.data.twistNodes = twistNodes;
            twistCorrection.data.sourceObject = FindChildByRecursion(targetObject.transform, _sourceName);
        }

        private Transform FindChildByRecursion(Transform aParent, string aName)
        {
            if (aParent == null) return null;
            var result = aParent.Find(aName);
            if (result != null)
                return result;
            foreach (Transform child in aParent)
            {
                result = FindChildByRecursion(child, aName);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
#endif
}
