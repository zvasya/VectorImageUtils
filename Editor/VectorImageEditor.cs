using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VectorImageUtils.Editor
{
    [CustomEditor(typeof(VectorImage))]
    public class VectorImageEditor :  UnityEditor.Editor
    {
        private readonly Dictionary<Object, VectorImagePreview> _vectorImagePreviews = new();
        private void OnEnable()
        {
            foreach (var obj in targets)
            {
                var meshPreview = new VectorImagePreview(obj as VectorImage);
                _vectorImagePreviews.Add(obj, meshPreview);
            }
        }

        private void OnDisable()
        {
            foreach (var obj in targets)
            {
                var meshPreview = _vectorImagePreviews[obj];
                meshPreview.Dispose();
            }
            _vectorImagePreviews.Clear();
        }

        public override bool HasPreviewGUI() => target != null;

        public override Texture2D RenderStaticPreview(
            string assetPath,
            Object[] subAssets,
            int width,
            int height)
        {
            return _vectorImagePreviews.TryGetValue(target, out var vectorImagePreview)
                ? vectorImagePreview.RenderStaticPreview(width, height)
                : null;
        }
        
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (!_vectorImagePreviews.TryGetValue(target, out var vectorImagePreview))
                return;
            vectorImagePreview.OnPreviewGUI(r, background);
        }
    }
}