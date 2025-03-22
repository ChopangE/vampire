using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
#endif

namespace StellaRabbitStudio
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("TMP VFX/TMP VFX")]
    public class TMPVFX : MonoBehaviour
    {
        public enum ShaderTypes
        {
            Default = 0,
            Invalid = 1
        }
        public ShaderTypes currentShaderType = ShaderTypes.Invalid;

        private Material currMaterial, prevMaterial;
        private bool matAssigned = false, destroyed = false;

#if UNITY_EDITOR
        private static float timeLastReload = -1f;
        private void Start()
        {
            if (timeLastReload < 0) timeLastReload = Time.time;
        }

        private void Update()
        {
            if (matAssigned || Application.isPlaying || !gameObject.activeSelf) return;
            TextMeshProUGUI tmpro = GetComponent<TextMeshProUGUI>();
            if (tmpro != null)
            {
                if (tmpro.material == null)
                {
                    CleanFont();
                }
                else matAssigned = true;
            }
            else
            {
                Graphic img = GetComponent<Graphic>();
                if (img != null)
                {
                    matAssigned = true;
                }
            }
        }
#endif

        private string GetStringFromShaderType()
        {
            currentShaderType = ShaderTypes.Default;
            return "TMPVFX";
        }

        public void CleanFont()
        {
            TMP_Text tmpro = GetComponent<TMP_Text>();
            if (tmpro != null)
            {
                tmpro.material = new Material(Shader.Find("TextMeshPro/Mobile/Distance Field"));
                matAssigned = false;
            }
            else
            {
                Graphic img = GetComponent<Graphic>();
                if (img != null)
                {
                    img.material = new Material(Shader.Find("UI/Default"));
                    matAssigned = false;
                }
            }
            SetSceneDirty();
        }

        public bool SaveFont(bool clear = false)
        {
#if UNITY_EDITOR
            string gameObjectName = TMPVFXShaderData.CheckFileName(gameObject.name);
            string fontPath = Path.Combine(TMPVFXShaderData.GetFontSavePath(), gameObjectName);
            
            if (!ValidateSavePath(fontPath)) return false;
            
            string fullPath = GetUniqueFilePath(fontPath);
            if (string.IsNullOrEmpty(fullPath)) return false;
            
            DoSaving(fullPath, clear);
            SetSceneDirty();
            return true;
#else
            return false;
#endif
        }

        private bool ValidateSavePath(string path)
        {
#if UNITY_EDITOR
            string directory = Path.GetDirectoryName(path);
            if (Directory.Exists(directory)) return true;
            
            EditorUtility.DisplayDialog("Invalid Save Path",
                "You need to set a valid folder. Please go support discord for more information.", "Ok");
            return false;
#else
            return false;
#endif
        }

        private string GetUniqueFilePath(string basePath)
        {
#if UNITY_EDITOR
            string path = basePath + ".asset";
            int counter = 1;
            
            while (File.Exists(path))
            {
                path = $"{basePath}_{counter}.asset";
                counter++;
                
                if (counter > 999) // Safety measure
                {
                    Debug.LogError("Failed to generate unique file name");
                    return null;
                }
            }
            return path;
#else
            return "";       
#endif
        }

        private void DoSaving(string fileName, bool clear = false)
        {
#if UNITY_EDITOR
            var (success, font) = GetCurrentFont();
            if (!success) return;
            
            try
            {
                // Clone font asset
                TMP_FontAsset createdFont = Instantiate(font);
                createdFont.name = font.name + "_Custom";
                
                // Clone and setup Material first to avoid modifying the original
                Material newMaterial = new Material(font.material);
                newMaterial.name = font.material.name + "_Copy";
                
                // If clear is true, remove all effects from the cloned material only
                if (clear)
                {
                    ClearAllKeywords(newMaterial);
                }
                
                // Assign the new material to the cloned font
                createdFont.material = newMaterial;
                
                // Save font asset first
                AssetDatabase.CreateAsset(createdFont, fileName);
                
                // Clone Atlas texture
                if (font.atlasTexture != null)
                {
                    // Copy texture data directly
                    RenderTexture renderTex = RenderTexture.GetTemporary(
                        font.atlasTexture.width,
                        font.atlasTexture.height,
                        0,
                        RenderTextureFormat.ARGB32,
                        RenderTextureReadWrite.Linear);
                    
                    Graphics.Blit(font.atlasTexture, renderTex);
                    
                    RenderTexture previous = RenderTexture.active;
                    RenderTexture.active = renderTex;
                    
                    Texture2D newAtlas = new Texture2D(font.atlasTexture.width, font.atlasTexture.height, TextureFormat.Alpha8, false);
                    newAtlas.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
                    newAtlas.Apply();
                    
                    RenderTexture.active = previous;
                    RenderTexture.ReleaseTemporary(renderTex);
                    
                    // Copy texture settings
                    newAtlas.name = font.atlasTexture.name + "_Copy";
                    newAtlas.filterMode = font.atlasTexture.filterMode;
                    newAtlas.wrapMode = font.atlasTexture.wrapMode;
                    
                    // Save Atlas texture as a sub-asset of the font asset
                    AssetDatabase.AddObjectToAsset(newAtlas, fileName);
                    
                    // Set Atlas texture to the cloned material
                    newMaterial.SetTexture("_MainTex", newAtlas);
                    createdFont.atlasTextures = new Texture2D[] { newAtlas };
                    
                    // Save Material as a sub-asset of the font asset
                    AssetDatabase.AddObjectToAsset(newMaterial, fileName);
                    
                    // Setup Atlas texture
                    SerializedObject serializedFont = new SerializedObject(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fileName));
                    SerializedProperty atlasTextureProp = serializedFont.FindProperty("m_AtlasTexture");
                    if (atlasTextureProp != null)
                    {
                        atlasTextureProp.objectReferenceValue = newAtlas;
                        serializedFont.ApplyModifiedProperties();
                    }
                }
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                Debug.Log($"{fileName} has been saved with custom Atlas and Material!");
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(fileName, typeof(TMP_FontAsset)));
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save font: {e.Message}");
                Debug.LogException(e);  // Print detailed error information
            }
#endif
        }

        private (bool success, TMP_FontAsset font) GetCurrentFont()
        {
#if UNITY_EDITOR
            if (TryGetComponent<TMP_Text>(out var tmpro))
            {
                return (true, tmpro.font);
            }
            
            MissingRenderer();
            return (false, null);
#else
            return (false, null);
#endif
        }


        private void FindCurrMaterial()
        {
            TMP_Text tmpro = GetComponent<TMP_Text>();
            if (tmpro != null && tmpro.font != null)
            {
                currMaterial = tmpro.font.material;
                matAssigned = true;
            }
            else
            {
                Debug.Log("There's no TMP_Text component or font.");
            }
        }

        public void SetSceneDirty()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) EditorSceneManager.MarkAllScenesDirty();

#if UNITY_2021_2_OR_NEWER
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#else
            var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif
            if (prefabStage != null) EditorSceneManager.MarkSceneDirty(prefabStage.scene);
#endif
        }


        private void MissingRenderer()
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Missing Text Component",
                "This GameObject (" + gameObject.name + ") has no TextMeshPro or UI component. This TMP VFX component will be removed.",
                "Ok");
            destroyed = true;
            DestroyImmediate(this);
#endif
        }

        public bool ClearAllKeywords(Material material = null)
        {
#if UNITY_EDITOR
            Material targetMaterial = material ?? currMaterial;
            if (targetMaterial == null && material == null)
            {
                FindCurrMaterial();
                targetMaterial = currMaterial;
                if (targetMaterial == null)
                {
                    MissingRenderer();
                    return false;
                }
            }

            // Basic TMP keywords
            SetKeyword("BEVEL_ON", false, targetMaterial);
            SetKeyword("GLOW_ON", false, targetMaterial);
            SetKeyword("UNDERLAY_ON", false, targetMaterial);
            SetKeyword("UNDERLAY_INNER", false, targetMaterial);
            SetKeyword("OUTLINE_ON", false, targetMaterial);
            
            // VFX keywords and properties
            SetKeyword("GLITCH_ON", false, targetMaterial);
            targetMaterial.SetFloat("_GLITCHEnabled", 0);
            
            SetKeyword("CRT_ON", false, targetMaterial);
            targetMaterial.SetFloat("_CRTEnabled", 0);
            
#endif
            return true;
        }

        private void SetKeyword(string keyword, bool state = false, Material material = null)
        {
            if (destroyed) return;
            if (currMaterial == null)
            {
                FindCurrMaterial();
                if (currMaterial == null)
                {
                    MissingRenderer();
                    return;
                }
            }
            if(material == null) material = currMaterial;

            if (!state) material.DisableKeyword(keyword);
            else material.EnableKeyword(keyword);
        }
    }
}
