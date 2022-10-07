using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace VectorImageUtils.Editor
{
    public static class Utils
    {
        [Serializable]
        public struct VectorImageVertexReplacer
        {
            public Vector3 position;
            public Color32 tint;
            public Vector2 uv;
            public uint settingIndex;
        }


        private static Type _vectorImageType;
        private static Type VectorImageType => _vectorImageType ??= typeof(VectorImage);

        private static FieldInfo _verticesFieldInfo;

        private static FieldInfo VerticesFieldInfo => _verticesFieldInfo ??=
            VectorImageType.GetField("vertices", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo _indexesFieldInfo;

        private static FieldInfo IndexesFieldInfo => _indexesFieldInfo ??=
            VectorImageType.GetField("indices", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo _sizeFieldInfo;

        private static FieldInfo SizeFieldInfo => _sizeFieldInfo ??=
            VectorImageType.GetField("size", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo _atlasFieldInfo;

        private static FieldInfo AtlasFieldInfo => _atlasFieldInfo ??=
            VectorImageType.GetField("atlas", BindingFlags.NonPublic | BindingFlags.Instance);

        private static Array GetViVertices(object obj) => VerticesFieldInfo.GetValue(obj) as Array;
        private static void SetViVertices(object obj, Array value) => VerticesFieldInfo.SetValue(obj, value);

        private static ushort[] GetViIndexes(object obj) => IndexesFieldInfo.GetValue(obj) as ushort[];
        private static void SetViIndexes(object obj, ushort[] value) => IndexesFieldInfo.SetValue(obj, value);

        private static Vector2 GetViSize(object obj) => (Vector2) SizeFieldInfo.GetValue(obj);
        private static void SetViSize(object obj, Vector2 value) => SizeFieldInfo.SetValue(obj, value);
        private static Texture2D GetViAtlas(object obj) => AtlasFieldInfo.GetValue(obj) as Texture2D;
        private static void SetViAtlas(object obj, Texture2D value) => AtlasFieldInfo.SetValue(obj, value);


        private static Type _vectorImageVerticesType;

        private static Type VectorImageVerticesType => _vectorImageVerticesType ??=
            VectorImageType.Assembly.GetType("UnityEngine.UIElements.VectorImageVertex");

        private static FieldInfo _positionFieldInfo;

        private static FieldInfo PositionFieldInfo => _positionFieldInfo ??=
            VectorImageVerticesType.GetField("position", BindingFlags.Public | BindingFlags.Instance);

        private static FieldInfo _tintFieldInfo;

        private static FieldInfo TintFieldInfo => _tintFieldInfo ??=
            VectorImageVerticesType.GetField("tint", BindingFlags.Public | BindingFlags.Instance);

        private static FieldInfo _uvFieldInfo;

        private static FieldInfo UVFieldInfo => _uvFieldInfo ??=
            VectorImageVerticesType.GetField("uv", BindingFlags.Public | BindingFlags.Instance);

        private static FieldInfo _settingIndexFieldInfo;

        private static FieldInfo SettingIndexFieldInfo => _settingIndexFieldInfo ??=
            VectorImageVerticesType.GetField("settingIndex", BindingFlags.Public | BindingFlags.Instance);

        private static Array CreateArrayVectorImageVertex(int count) =>
            Array.CreateInstance(VectorImageVerticesType, count);

        private static object CreateVectorImageVertex() => Activator.CreateInstance(VectorImageVerticesType);
        private static Vector3 GetPosition(object obj) => (Vector3) PositionFieldInfo.GetValue(obj);
        private static void SetPosition(object obj, Vector3 value) => PositionFieldInfo.SetValue(obj, value);
        private static Vector2 GetUV(object obj) => (Vector2) UVFieldInfo.GetValue(obj);
        private static void SetUV(object obj, Vector2 value) => UVFieldInfo.SetValue(obj, value);
        private static Color32 GetTint(object obj) => (Color32) TintFieldInfo.GetValue(obj);
        private static void SetTint(object obj, Color32 value) => TintFieldInfo.SetValue(obj, value);
        private static uint GetSettingIndex(object obj) => (uint) SettingIndexFieldInfo.GetValue(obj);
        private static void SetSettingIndex(object obj, uint value) => SettingIndexFieldInfo.SetValue(obj, value);


        public static VectorImageVertexReplacer[] GetVertices(this VectorImage img)
        {
            var vectorImageVertices = GetViVertices(img);

            VectorImageVertexReplacer[] v = new VectorImageVertexReplacer[vectorImageVertices.Length];
            for (int i = 0; i < vectorImageVertices.Length; i++)
            {
                var s = new VectorImageVertexReplacer();
                s.position = GetPosition(vectorImageVertices.GetValue(i));
                s.uv = GetUV(vectorImageVertices.GetValue(i));
                s.tint = GetTint(vectorImageVertices.GetValue(i));
                s.settingIndex = GetSettingIndex(vectorImageVertices.GetValue(i));
                v[i] = s;
            }

            return v;
        }

        public static Vector2[] GetVerticesPositions(this VectorImage img)
        {
            var verticesFieldInfo = VerticesFieldInfo;
            var vectorImageVertices = verticesFieldInfo.GetValue(img) as Array;

            var v = new Vector2[vectorImageVertices.Length];
            for (int i = 0; i < vectorImageVertices.Length; i++)
                v[i] = GetPosition(vectorImageVertices.GetValue(i));

            return v;
        }

        public static void SetVertices(this VectorImage img, VectorImageVertexReplacer[] vertices)
        {
            var v = CreateArrayVectorImageVertex(vertices.Length);
            for (int i = 0; i < vertices.Length; i++)
            {
                var s = CreateVectorImageVertex();
                SetPosition(s, vertices[i].position);
                SetUV(s, vertices[i].uv);
                SetTint(s, vertices[i].tint);
                SetSettingIndex(s, vertices[i].settingIndex);
                v.SetValue(s, i);
            }

            SetViVertices(img, v);
        }

        public static ushort[] GetIndexes(this VectorImage img)
        {
            return GetViIndexes(img).ToArray();
        }

        public static void SetIndexes(this VectorImage img, ushort[] indices)
        {
            SetViIndexes(img, indices.ToArray());
        }

        public static Vector2 GetSize(this VectorImage img)
        {
            return GetViSize(img);
        }

        public static void SetSize(this VectorImage img, Vector2 size)
        {
            SetViSize(img, size);
        }

        public static Texture2D GetAtlas(this VectorImage img)
        {
            return GetViAtlas(img);
        }

        public static void SetAtlas(this VectorImage img, Texture2D atlas)
        {
            SetViAtlas(img, atlas);
        }
        
        public static Mesh CreateMesh(this VectorImage img)
        {
            var m = new Mesh();
            var vertices = img.GetVertices();
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


            var indexes = img.GetIndexes();
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
    }
}