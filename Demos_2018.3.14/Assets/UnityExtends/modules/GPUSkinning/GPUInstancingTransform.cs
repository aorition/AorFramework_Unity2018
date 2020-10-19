using UnityEngine;

namespace Framework.GPUSkinning
{
    public class GPUInstancingTransform
    {
        /// <summary>
        /// 用于解决GC的问题，原本写法有装拆箱性能问题，会产生大量GC。PeterFAN
        /// </summary>
        public struct GITValueStr
        {
            private float m_floatValues;
            private Vector4 m_ve4Values;
            private Matrix4x4 m_matValues;

            public float FloatValues
            {
                get {
                    return m_floatValues;
                }

                set {
                    m_floatValues = value;
                }
            }

            public Vector4 Ve4Values
            {
                get {
                    return m_ve4Values;
                }

                set {
                    m_ve4Values = value;
                }
            }

            public Matrix4x4 MatValues
            {
                get {
                    return m_matValues;
                }

                set {
                    m_matValues = value;
                }
            }
        }

        public int id;
        //public ObjectGroup objManager;
        //public ObjectGroup.objStage _stage = ObjectGroup.objStage.show;
        //public ObjectGroup.objStage stage
        //{
        //    get { return _stage; }
        //    set
        //    {
        //        if(_stage == ObjectGroup.objStage.delete)
        //        {
        //            return;
        //        }
        //        _stage = value;
        //        objManager.isObjectChange = true;
        //        ObjectGroup.isCameraChange = true;
        //    }
        //}
        public bool isChange;
        private Bounds _bound;
        public Bounds bound;
        //public Vector3 position = Vector3.zero;
        //    public Quaternion rotation = new Quaternion(0,0,0,1);
        //    public Vector3 scale = Vector3.one;
        public Matrix4x4 localToWorldMatrix;

        public float fXScale = 1;   // 横向缩放
        public float fZScale = 1;   // Z方向缩放

        public void SetBound()
        {
            bound = Transform(_bound, localToWorldMatrix);//TODO 包围盒是椎体裁剪 改为以队伍为单位运算。
        }

        //    public void setMatr4()
        //    {
        //        mx.SetTRS(position, rotation, scale);
        //       
        //    }
        /// <summary>
        /// 获取Bounds
        /// </summary>
        static Bounds Transform(Bounds bounds, Matrix4x4 matrix)
        {
            Vector3 p0 = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z);
            Vector3 p1 = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z);
            Vector3 p2 = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z);
            Vector3 p3 = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z);
            Vector3 p4 = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z);
            Vector3 p5 = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
            Vector3 p6 = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z);
            Vector3 p7 = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);


            Bounds _bounds = new Bounds();
            _bounds.center = matrix.MultiplyPoint(p0);
            _bounds.size = Vector3.zero;
            _bounds.Encapsulate(matrix.MultiplyPoint(p1));
            _bounds.Encapsulate(matrix.MultiplyPoint(p2));
            _bounds.Encapsulate(matrix.MultiplyPoint(p3));
            _bounds.Encapsulate(matrix.MultiplyPoint(p4));
            _bounds.Encapsulate(matrix.MultiplyPoint(p5));
            _bounds.Encapsulate(matrix.MultiplyPoint(p6));
            _bounds.Encapsulate(matrix.MultiplyPoint(p7));

            return _bounds;
        }
        //public void SetActive(bool active)
        //{
        //    if (active)
        //    {
        //        stage = ObjectGroup.objStage.show;
        //    }
        //    else
        //    {
        //        stage = ObjectGroup.objStage.hide;
        //    }
        //}

        //// 移除对象
        //public bool Remove()
        //{
        //    if(stage != ObjectGroup.objStage.delete)
        //    {
        //        stage = ObjectGroup.objStage.delete;
        //        id = -1;
        //        objManager.DeleteObj();
        //        return true;
        //    }
        //    return false;
        //}
        public int[] keyWord;
        public GITValueStr[] keyValue;

        public float m_floatValue;
        public Vector4 m_v4Value;
        public Matrix4x4 m_matValue;

        public KeyType[] keyType;
        public enum KeyType
        {
            Float,
            Matrix,
            Vecter,
        }
        ///// <summary>
        ///// 设置几何实例化需要改变的参数和改变参数的方法（可以不设置）
        ///// </summary>
        ///// <param name="_keyWord">Shader里的变量名</param>
        ///// <param name="_keyType">shader里的变量类型</param>
        ///// <param name="_update">改变变量的方法</param>
        //public void setPropertyBlockValue(string[] _keyWord, KeyType[] _keyType, CallBack _update)
        //{
        //    int i = 0;
        //    keyWord = new int[_keyWord.Length];
        //    int iLenth = _keyWord.Length;
        //    for (i = 0; i < iLenth; i++)
        //    {
        //        keyWord[i] = Shader.PropertyToID(_keyWord[i]);
        //    }
        //    keyValue = new GITValueStr[_keyWord.Length];
        //    keyType = _keyType;
        //    update = _update;
        //}

        //public CallBack update;
        //public void Update()
        //{
        //    if(update != null)
        //    {
        //        update();
        //    }
        //}
        //    /// <summary>
        //    /// 创建一个几何实例化数据对象
        //    /// </summary>
        //    /// <param name="id"></param>
        //    /// <param name="pos"></param>
        //    /// <param name="rotation"></param>
        //    /// <param name="scale"></param>
        //    /// <param name="b"></param>
        //    /// <param name="transformIsChange"></param>
        //    /// <returns></returns>
        //    public static GPUInstancingTransform createTransForm(int id, Matrix4x4 worldMatrix/*, Vector3 pos, Quaternion rotation, Vector3 scale*/, Bounds b,bool transformIsChange, ObjectGroup objManager)
        //    {
        //        GPUInstancingTransform tr = new GPUInstancingTransform();
        //        tr.objManager = objManager;
        //        tr.id = id;
        //        tr._bound = b;
        ////        tr.position = pos;
        ////        tr.scale = scale;
        ////        tr.rotation = rotation;
        //       tr.localToWorldMatrix= worldMatrix;
        //        tr.isChange = transformIsChange;
        ////        tr.setMatr4();
        //        tr.SetBound();
        //        return tr;
        //    }


        private bool m_inCameraView;
        /// <summary>
        /// 是否在摄像机检测范围内
        /// </summary>
        public bool InCameraView
        {
            get {
                return m_inCameraView;
            }

            set {
                m_inCameraView = value;
            }
        }
    }

}

