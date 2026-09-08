using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Text;
using System.IO;



#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TrueSim.Runtime.CCiC.CharacterTemplates
{
    [CreateAssetMenu(fileName = "Material Conversion Table", menuName = "TrueSim/CCiC/Material Conversion Table")]
    public class MaterialConversionTable : ScriptableObject
    {
        [SerializeField] private MaterialReference[] _targets;

        public void Convert(Material[] materials, string destination, string materialSuffix, ref Dictionary<string, Material> convertedMaterials)
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                _targets[i].MigrateToVariant(materials, destination, materialSuffix, ref convertedMaterials);
            }
            AssetDatabase.SaveAssets();
        }

        public void Apply(GameObject targetObject, ref Dictionary<string, Material> convertedMaterials)
        {
            var stringBuilder = new StringBuilder();
            foreach (var renderer in targetObject.GetComponentsInChildren<Renderer>())
            {
                var sharedMaterials = renderer.sharedMaterials;
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    if (convertedMaterials.TryGetValue(sharedMaterials[i].name, out var material))
                    {
                        stringBuilder.AppendLine($"{targetObject.name}/{renderer.gameObject.name} material {i} set to {material.name}");
                        sharedMaterials[i] = material;
                    }
                }
                renderer.sharedMaterials = sharedMaterials;
            }
            Debug.Log(stringBuilder.ToString(), targetObject);
        }
    }

    [System.Serializable]
    public class MaterialReference
    {
        [SerializeField] private string _defaultMaterialName;
        [SerializeField] private string _customMaterialName;

        [SerializeField] private ParameterCopyPair[] _copyPairs;

        public void MigrateToVariant(Material[] materials, string destination, string materialSuffix, ref Dictionary<string, Material> convertedMaterials)
        {
            Material defaultMaterial = null;
            for (int i = 0; i < materials.Length; i++)
            {
                if (!materials[i].name.Equals(_defaultMaterialName))
                {
                    continue;
                }
                defaultMaterial = materials[i];
                break;
            }

            if (defaultMaterial == null)
            {
                return;
            }

            var parentMaterial = GetByName(_customMaterialName);
            if(parentMaterial == null)
            {
                Debug.LogWarning("Material could not be found");
                return;
            }

            var materialPath = $"{destination}/{_customMaterialName}_{materialSuffix}.mat";
            var assetExists = AssetDatabase.AssetPathExists(materialPath);
            var childMaterial = assetExists ?
                AssetDatabase.LoadAssetAtPath<Material>(materialPath) :
                new Material(parentMaterial)
                {
                    parent = parentMaterial
                };

            for(int i = 0; i < _copyPairs.Length; i++)
            {
                _copyPairs[i].CopyParameter(defaultMaterial, childMaterial);
            }
            if(!convertedMaterials.TryAdd(defaultMaterial.name, childMaterial))
            {
                Debug.LogWarning($"{defaultMaterial.name} could not be added to dictionary");
            }
            if (!assetExists)
            {
                AssetDatabase.CreateAsset(childMaterial, materialPath);
            }
        }

        private Material GetByName(string name, string[] folders = null)
        {
            if (folders == null) folders = new string[] { "Assets", "Packages" };

            string[] guids = AssetDatabase.FindAssets(name + " t:material", folders);

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat)
                {
                    if (mat.name.Equals(name))
                    {
                        return mat;
                    }
                }
            }

            return null;
        }
    }

    [System.Serializable]
    public class ParameterCopyPair
    {
        [SerializeField] private string _defaultParameterName;
        [SerializeField] private string _customParameterName;

        public void CopyParameter(Material defaultMaterialInstance, Material customMaterialInstance)
        {
            if (!defaultMaterialInstance.HasProperty(_defaultParameterName))
            {
                Debug.LogError($"{_defaultParameterName} could not be found on material : {defaultMaterialInstance.name}");
                return;
            }

            if (!customMaterialInstance.HasProperty(_customParameterName))
            {
                Debug.LogError($"{_customParameterName} could not be found on material : {customMaterialInstance.name}");
                return;
            }

            var defaultMaterialProperty = MaterialEditor.GetMaterialProperty(new Object[1] { defaultMaterialInstance }, _defaultParameterName);
            var customMaterialProperty = MaterialEditor.GetMaterialProperty(new Object[1] { customMaterialInstance }, _customParameterName);

            if(defaultMaterialProperty.propertyType != customMaterialProperty.propertyType)
            {
                Debug.LogError($"Material properties {_defaultParameterName} and {_customParameterName} dont match types ({defaultMaterialProperty.propertyType} | {customMaterialProperty.propertyType})");
                return;
            }

            switch (defaultMaterialProperty.propertyType)
            {
                case UnityEngine.Rendering.ShaderPropertyType.Color:
                    customMaterialInstance.SetColor(_customParameterName, defaultMaterialInstance.GetColor(_defaultParameterName));
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Vector:
                    customMaterialInstance.SetVector(_customParameterName, defaultMaterialInstance.GetVector(_defaultParameterName));
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Float:
                case UnityEngine.Rendering.ShaderPropertyType.Range:
                    customMaterialInstance.SetFloat(_customParameterName, defaultMaterialInstance.GetFloat(_defaultParameterName));
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Texture:
                    customMaterialInstance.SetTexture(_customParameterName, defaultMaterialInstance.GetTexture(_defaultParameterName));
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Int:
                    customMaterialInstance.SetInteger(_customParameterName, defaultMaterialInstance.GetInteger(_defaultParameterName));
                    break;
            }
        }
    }
}
