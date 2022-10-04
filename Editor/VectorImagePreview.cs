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
            var mesh = CreateMesh();

            var size = target.GetSize();
            var go = new GameObject();
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshFilter.mesh = mesh;
            meshRenderer.material = new Material(Shader.Find("Unlit/VectorUI"));

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


        private Mesh CreateMesh()
        {
            var m = new Mesh();
            var vertices = _target.GetVertices();
            var verticesLength = vertices.Length;
            var meshVertices = new Vector3[verticesLength];
            var meshUV = new Vector2[verticesLength];
            var meshColors = new Color[verticesLength];

            for (var i = 0; i < verticesLength; i++)
            {
                var vertex = vertices[i];
                meshVertices[i] = vertex.position;
                meshUV[i] = vertex.uv;
                meshColors[i] = vertex.tint;
            }


            var indexes = _target.GetIndexes();
            var indexesLength = indexes.Length;
            var meshTriangles = new int[indexesLength];
            for (var i = 0; i < indexesLength; i++)
            {
                meshTriangles[i] = indexes[i];
            }

            m.vertices = meshVertices;
            m.uv = meshUV;
            m.colors = meshColors;
            m.triangles = meshTriangles;

            m.RecalculateBounds();
            return m;
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