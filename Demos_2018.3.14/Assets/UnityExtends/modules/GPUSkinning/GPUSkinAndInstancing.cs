using UnityEngine;
using System;

namespace Framework.GPUSkinning
{
    public class GPUSkinAndInstancing :MonoBehaviour
    {
        public class InstanceData
        {
            public float x;
            public float y;
            public float z;
            public float rx;
            public float ry;
            public float rz;
            public float frame;
            public float sc;
        }

        public InstanceData[] list;

        protected int mCount = 0;
        Mesh mesh;
        Material material;
        Transform tra;
        Matrix4x4[] matrices;
        float[] frames;
        MaterialPropertyBlock properties;
        Vector3 position = Vector3.one;
        Quaternion rotation;
        Vector3 scale;
        Vector3 dic = Vector3.one;

        public void Init(int count, Mesh _mesh, Material _material, Transform _tra)
        {
            scale = gameObject.transform.localScale;
            properties = new MaterialPropertyBlock();
            mCount = count;
            list = new InstanceData[count];
            matrices = new Matrix4x4[count];
            frames = new float[count];
            for(int i = 0; i < count; i++)
            {
                list[i] = (new InstanceData());
            }
            mesh = _mesh;
            material = _material;
            tra = _tra;
        }

        public void SetCount(int count)
        {
            mCount = count;
        }

        public void Set(int i, float x, float y, float z, float rx, float ry, float rz, float frame, float sc = 0)
        {
            InstanceData data = list[i];
            data.x = x;
            data.y = y;
            data.z = z;
            data.rx = rx;
            data.ry = ry;
            data.rz = rz;
            data.frame = frame;
            data.sc = sc;
        }

        //public void Unpack(string packData)
        //{
        //    Debug.Log(packData);
        //    return;
        //    int iData = 0;
        //    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(packData);
        //    int len = sizeof(float);
        //    int dataLen = len * 7;
        //    for(int i = 0; i < bytes.Length; i += dataLen)
        //    {
        //        InstanceData data = list[iData];
        //        data.x = BitConverter.ToSingle(bytes, i + 0);
        //        data.y = BitConverter.ToSingle(bytes, i + len * 1);
        //        data.z = BitConverter.ToSingle(bytes, i + len * 2);
        //        data.rx = BitConverter.ToSingle(bytes, i + len * 3);
        //        data.ry = BitConverter.ToSingle(bytes, i + len * 4);
        //        data.rz = BitConverter.ToSingle(bytes, i + len * 5);
        //        data.frame = BitConverter.ToSingle(bytes, i + len * 6);
        //        iData++;
        //    }
        //}

        public void Draw()
        {
            for(int i = 0; i < mCount; i++)
            {
                InstanceData data = list[i];
                dic.x = data.rx;
                dic.y = data.ry;
                dic.z = data.rz;
                if(dic.Equals(Vector3.zero))
                    rotation = tra.rotation;
                else
                    rotation = tra.rotation * Quaternion.LookRotation(dic);

                if(data.sc != 0)
                {
                    scale.x = data.sc;
                    scale.y = data.sc;
                    scale.z = data.sc;
                }
                position.x = data.x;
                position.y = data.y;
                position.z = data.z;
                position = tra.rotation * position;
                position += tra.position;
                matrices[i] = Matrix4x4.TRS(position, rotation, scale);
                frames[i] = data.frame;
            }
            properties.SetFloatArray("_Frame", frames);
            Graphics.DrawMeshInstanced(mesh, 0, material, matrices, mCount, properties);
        }

        public void Update()
        {
            if(mCount != 0)
            {
                Draw();
            }
        }
    }

}

