using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VectorImageUtils.Editor
{
    public class VectorImagePreview : IDisposable
    {
        private readonly VectorImage _target;
        private readonly PreviewRenderUtility _previewUtility;

        public VectorImagePreview(VectorImage target)
        {
            _target = target;
            _previewUtility = new();
            SetupScene();
            var mesh = _target.CreateMesh();

            var size = target.GetSize();
            var go = new GameObject();
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshFilter.mesh = mesh;
            meshRenderer.material = new Material(Shader.Find("UI/Default"));

            _previewUtility.AddSingleGO(go);
            go.transform.position = new Vector3(-size.x / 2, size.y / 2, 0);
            go.transform.rotation = Quaternion.Euler(180, 0, 0);
        }

        private void SetupScene()
        {
            _previewUtility.camera.orthographic = true;
            _previewUtility.camera.transform.position = new Vector3(0, 0, -10);
            foreach (var light in _previewUtility.lights)
            {
                light.enabled = false;
            }

            _previewUtility.ambientColor = Color.white;
        }

        public void Dispose()
        {
            _previewUtility.Cleanup();
        }

        public Texture2D RenderStaticPreview(int width, int height)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture)
                return null;
            SetupScene();
            _previewUtility.camera.orthographicSize = GetCameraSize(width, height, 0.3f);
            _previewUtility.BeginStaticPreview(new Rect(0.0f, 0.0f, (float) width, (float) height));
            _previewUtility.Render();
            return _previewUtility.EndStaticPreview();
        }

        public void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            SetupScene();
            _previewUtility.camera.orthographicSize = GetCameraSize(rect.width, rect.height, 0.1f);
            _previewUtility.BeginPreview(rect, background);
            _previewUtility.Render();
            _previewUtility.EndAndDrawPreview(rect);
        }

        private float GetCameraSize(float width, float height, float overflow)
        {
            var aspect = height / width;
            var size = _target.GetSize();
            var offset = Mathf.Max(size.x, size.y) * overflow;
            return (aspect < 1 ? size.y / 2 : aspect * size.x / 2) + offset;
        }
    }
}