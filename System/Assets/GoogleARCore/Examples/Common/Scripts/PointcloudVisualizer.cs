namespace GoogleARCore.Examples.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class PointcloudVisualizer : MonoBehaviour
    {
        [Tooltip("The color of the feature points.")]
        public Color PointColor;

        [Tooltip("Whether to enable the pop animation for the feature points.")]
        public bool EnablePopAnimation = true;

        [Tooltip("The maximum number of points to add per frame.")]
        public int MaxPointsToAddPerFrame = 1;

        [Tooltip("The time interval that the animation lasts in seconds.")]
        public float AnimationDuration = 0.3f;

        [Tooltip("The maximum number of points to show on the screen.")]
        [SerializeField] private int m_MaxPointCount = 1000;

        [Tooltip("The default size of the points.")]
        [SerializeField] private int m_DefaultSize = 10;

        [Tooltip("The maximum size that the points will have when they pop.")]
        [SerializeField] private int m_PopSize = 50;

        private Mesh m_Mesh;
        private MeshRenderer m_MeshRenderer;
        private int m_ScreenWidthId;
        private int m_ScreenHeightId;
        private int m_ColorId;
        private MaterialPropertyBlock m_PropertyBlock;
        private Resolution m_CachedResolution;
        private Color m_CachedColor;
        private LinkedList<PointInfo> m_CachedPoints;

        public void Start()
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_Mesh = GetComponent<MeshFilter>().mesh;
            if (m_Mesh == null)
            {
                m_Mesh = new Mesh();
            }

            m_Mesh.Clear();

            m_CachedColor = PointColor;

            m_ScreenWidthId = Shader.PropertyToID("_ScreenWidth");
            m_ScreenHeightId = Shader.PropertyToID("_ScreenHeight");
            m_ColorId = Shader.PropertyToID("_Color");

            m_PropertyBlock = new MaterialPropertyBlock();
            m_MeshRenderer.GetPropertyBlock(m_PropertyBlock);
            m_PropertyBlock.SetColor(m_ColorId, m_CachedColor);
            m_MeshRenderer.SetPropertyBlock(m_PropertyBlock);

            m_CachedPoints = new LinkedList<PointInfo>();
        }

        public void OnDisable()
        {
            _ClearCachedPoints();
        }

        public void Update()
        {
            if (Session.Status != SessionStatus.Tracking)
            {
                _ClearCachedPoints();
                return;
            }

            if (Screen.currentResolution.height != m_CachedResolution.height || Screen.currentResolution.width != m_CachedResolution.width)
            {
                _UpdateResolution();
            }

            if (m_CachedColor != PointColor)
            {
                _UpdateColor();
            }

            if (EnablePopAnimation)
            {
                _AddPointsIncrementallyToCache();
                _UpdatePointSize();
            }
            else
            {
                _AddAllPointsToCache();
            }

            _UpdateMesh();
        }

        private void _ClearCachedPoints()
        {
            m_CachedPoints.Clear();
            m_Mesh.Clear();
        }

        private void _UpdateResolution()
        {
            m_CachedResolution = Screen.currentResolution;
            if (m_MeshRenderer != null)
            {
                m_MeshRenderer.GetPropertyBlock(m_PropertyBlock);
                m_PropertyBlock.SetFloat(m_ScreenWidthId, m_CachedResolution.width);
                m_PropertyBlock.SetFloat(m_ScreenHeightId, m_CachedResolution.height);
                m_MeshRenderer.SetPropertyBlock(m_PropertyBlock);
            }
        }

        private void _UpdateColor()
        {
            m_CachedColor = PointColor;
            m_MeshRenderer.GetPropertyBlock(m_PropertyBlock);
            m_PropertyBlock.SetColor("_Color", m_CachedColor);
            m_MeshRenderer.SetPropertyBlock(m_PropertyBlock);
        }

        private void _AddPointsIncrementallyToCache()
        {
            if (Frame.PointCloud.PointCount > 0 && Frame.PointCloud.IsUpdatedThisFrame)
            {
                int iterations = Mathf.Min(MaxPointsToAddPerFrame, Frame.PointCloud.PointCount);
                for (int i = 0; i < iterations; i++)
                {
                    Vector3 point = Frame.PointCloud.GetPointAsStruct(
                        Random.Range(0, Frame.PointCloud.PointCount - 1));

                    _AddPointToCache(point);
                }
            }
        }

        private void _AddAllPointsToCache()
        {
            if (Frame.PointCloud.IsUpdatedThisFrame)
            {
                for (int i = 0; i < Frame.PointCloud.PointCount; i++)
                {
                    _AddPointToCache(Frame.PointCloud.GetPointAsStruct(i));
                }
            }
        }

        private void _AddPointToCache(Vector3 point)
        {
            if (m_CachedPoints.Count >= m_MaxPointCount)
            {
                m_CachedPoints.RemoveFirst();
            }

            m_CachedPoints.AddLast(new PointInfo(point, new Vector2(m_DefaultSize, m_DefaultSize), Time.time));
        }

        private void _UpdatePointSize()
        {
            if (m_CachedPoints.Count <= 0 || !EnablePopAnimation)
            {
                return;
            }

            LinkedListNode<PointInfo> pointNode;

            for (pointNode = m_CachedPoints.First; pointNode != null; pointNode = pointNode.Next)
            {
                float timeSinceAdded = Time.time - pointNode.Value.CreationTime;
                if (timeSinceAdded >= AnimationDuration)
                {
                    continue;
                }

                float value = timeSinceAdded / AnimationDuration;
                float size = 0f;

                if (value < 0.5f)
                {
                    size = Mathf.Lerp(m_DefaultSize, m_PopSize, value * 2f);
                }
                else
                {
                    size = Mathf.Lerp(m_PopSize, m_DefaultSize, (value - 0.5f) * 2f);
                }

                pointNode.Value = new PointInfo(pointNode.Value.Position, new Vector2(size, size),
                                                pointNode.Value.CreationTime);
            }
        }

        private void _UpdateMesh()
        {
            m_Mesh.Clear();
            m_Mesh.vertices = m_CachedPoints.Select(p => p.Position).ToArray();
            m_Mesh.uv = m_CachedPoints.Select(p => p.Size).ToArray();
            m_Mesh.SetIndices(Enumerable.Range(0, m_CachedPoints.Count).ToArray(), MeshTopology.Points, 0);
        }

        private struct PointInfo
        {
            public Vector3 Position;
            public Vector2 Size;
            public float CreationTime;

            public PointInfo(Vector3 position, Vector2 size, float creationTime)
            {
                Position = position;
                Size = size;
                CreationTime = creationTime;
            }
        }
    }
}
