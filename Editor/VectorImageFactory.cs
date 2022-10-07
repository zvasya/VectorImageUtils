using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VectorImageUtils.Editor
{
    public static class VectorImageFactory
    {
        [MenuItem("Assets/Create/VectorImage/Empty")]
        public static void CreateVectorImage()
        {
            string path = null;
            if (Selection.activeObject != null)
            {
                path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
                if (!string.IsNullOrEmpty(path))
                {
                    path = Path.GetDirectoryName(path);
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            
            var image = ScriptableObject.CreateInstance<VectorImage>();
            AssetDatabase.CreateAsset(image, Path.Combine(path, "vector_image.asset"));
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = image;
        }

        [MenuItem("Assets/Create/VectorImage/Extract", true)]
        public static bool ExtractVectorImageValidate()
        {
            return Selection.activeObject is VectorImage;
        }

        [MenuItem("Assets/Create/VectorImage/Extract")]
        public static void ExtractVectorImage()
        {
            string path = null;
            if (Selection.activeObject != null && Selection.activeObject is VectorImage vi)
            {
                path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
                if (!string.IsNullOrEmpty(path))
                {
                    var name = Path.GetFileNameWithoutExtension(path);
                    path = Path.GetDirectoryName(path) ?? "Assets";

                    VectorImage image = ScriptableObject.CreateInstance<VectorImage>();
                    image.SetVertices(vi.GetVertices());
                    image.SetIndexes(vi.GetIndexes());
                    image.SetAtlas(vi.GetAtlas());
                    image.SetSize(vi.GetSize());
                    AssetDatabase.CreateAsset(image, Path.Combine(path, $"{name}_VectorImage.asset"));
                    AssetDatabase.SaveAssets();
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = image;
                }
            }
        }
        
        [MenuItem("Assets/Create/VectorImage/Create Mesh", true)]
        public static bool MeshFromVectorImageValidate()
        {
            return Selection.activeObject is VectorImage;
        }
        
        [MenuItem("Assets/Create/VectorImage/Create Mesh")]
        public static void MeshFromVectorImageVectorImage()
        {
            string path = null;
            if (Selection.activeObject != null && Selection.activeObject is VectorImage vi)
            {
                path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
                if (!string.IsNullOrEmpty(path))
                {
                    var name = Path.GetFileNameWithoutExtension(path);
                    path = Path.GetDirectoryName(path) ?? "Assets";

                    var mesh = vi.CreateMesh();
                    AssetDatabase.CreateAsset(mesh, Path.Combine(path, $"{name}_mesh.asset"));
                    AssetDatabase.SaveAssets();
                    EditorUtility.FocusProjectWindow();
                }
            }
        }
    }
}