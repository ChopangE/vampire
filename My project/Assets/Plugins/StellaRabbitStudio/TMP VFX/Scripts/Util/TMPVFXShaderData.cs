using UnityEngine;
using System.Linq;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
#endif

namespace StellaRabbitStudio
{
    public class TMPVFXShaderData
    {
        public const string versionString = "v1.2";
        private static string basePath = "Assets/Plugins/StellaRabbitStudio/TMP VFX";
        private static readonly string fontsSavesRelativePath = "/Fonts/Generated";
#if UNITY_EDITOR
        public static void GetBasePath()
        {
            string[] guids = AssetDatabase.FindAssets("t:folder TMP VFX");
            if (guids.Length > 0)
            {
                basePath = AssetDatabase.GUIDToAssetPath(guids[0]);
            }
            else
            {
                Debug.LogError("VFX folder is not found in the project's Plugins folder. Changing path to Assets default folder.");
                basePath = "Assets/StellaRabbitStudio/TMP VFX";
            }
        }

        public static string GetFontSavePath()
        {
            if (!PlayerPrefs.HasKey("TMPVFXFonts"))
            {
                GetBasePath();
                return basePath + fontsSavesRelativePath;
            }
            return PlayerPrefs.GetString("TMPVFXFonts");
        }

        public static Shader FindShader(string shaderName)
        {
            string[] guids = AssetDatabase.FindAssets($"{shaderName} t:shader");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
                if (shader != null)
                {
                    string fullShaderName = shader.name;
                    string actualShaderName = fullShaderName.Substring(fullShaderName.LastIndexOf('/') + 1);
                    if (actualShaderName.Equals(shaderName)) return shader;
                }
            }
            return null;
        }

        public static string CheckFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();

            char[] additionalInvalidChars = new char[]
            {
                '<', '>', '|', '\"', '*', ':', '\\', '/'
            };

            string sanitized = fileName;

            foreach (char invalidChar in invalidChars.Concat(additionalInvalidChars))
            {
                sanitized = sanitized.Replace(invalidChar, '_');
            }

            while (sanitized.Contains("__"))
            {
                sanitized = sanitized.Replace("__", "_");
            }

            sanitized = sanitized.Trim().Trim('.', '_');

            if (string.IsNullOrWhiteSpace(sanitized))
            {
                sanitized = "Font_Asset";
            }

            return sanitized;
        }

        [MenuItem("Tools/TMP VFX/Fix Font Atlas And Materials")]
        public static void FixFontAtlasAndMaterials()
        {
            bool hasChanges = false;
            var fontAssets = AssetDatabase.FindAssets("t:TMP_FontAsset")
                .Select(guid => AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(asset => asset != null);

            foreach (var fontAsset in fontAssets)
            {
                if (fontAsset.atlasTexture != null && fontAsset.atlas == null)
                {
                    fontAsset.atlas = fontAsset.atlasTexture;
                    hasChanges = true;
                }
                if (fontAsset.atlasTexture == null && fontAsset.atlas != null)
                {
                    fontAsset.atlasTextures[0] = fontAsset.atlas;
                    hasChanges = true;
                }

                if (fontAsset.material == null && fontAsset.atlas != null)
                {
                    string atlasPath = AssetDatabase.GetAssetPath(fontAsset.atlas);
                    Material[] materials = AssetDatabase.LoadAllAssetsAtPath(atlasPath)
                        .OfType<Material>()
                        .ToArray();

                    if (materials.Length > 0)
                    {
                        fontAsset.material = materials[0];
                        hasChanges = true;
                    }
                }

                var serializedObject = new SerializedObject(fontAsset);
                var sourceFontFileProperty = serializedObject.FindProperty("m_SourceFontFile_EditorRef");
                var sourceFontFileGUIDProperty = serializedObject.FindProperty("m_SourceFontFileGUID");

                if (sourceFontFileProperty.objectReferenceValue == null && 
                    !string.IsNullOrEmpty(sourceFontFileGUIDProperty.stringValue))
                {
                    string fontPath = AssetDatabase.GUIDToAssetPath(sourceFontFileGUIDProperty.stringValue);
                    if (!string.IsNullOrEmpty(fontPath))
                    {
                        Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
                        if (sourceFont != null)
                        {
                            sourceFontFileProperty.objectReferenceValue = sourceFont;
                            serializedObject.ApplyModifiedProperties();
                            
                            hasChanges = true;
                            Debug.Log($"Restored editor source font for {fontAsset.name} from GUID {sourceFontFileGUIDProperty.stringValue}");
                        }
                    }
                }
            }

            if (hasChanges)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                string currentScenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
                if (!string.IsNullOrEmpty(currentScenePath))
                {
                    EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
                }
                
                Debug.Log("Font assets have been fixed and scene has been reloaded.");
            }
        }
    #endif
    }
}
