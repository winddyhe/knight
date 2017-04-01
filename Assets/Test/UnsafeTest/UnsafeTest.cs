using UnityEngine;
using System.Collections;
using System;
using Core;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Test
{
    public unsafe class UnsafeClass1
    {
        private int[]   mArray1;
        private IntPtr  mArray1Ptr;
        private int*    mArrayPoint;

        public unsafe void Init(int nSize)
        {
            mArray1 = new int[nSize];
            for (int i = 0; i < mArray1.Length; i++)
            {
                this.mArray1[i] = i;
            }
            fixed (int* p = this.mArray1)
            {
                mArray1Ptr = new IntPtr(p);
                mArrayPoint = p;
            }
        }

        public unsafe void Destroy()
        {

        }

        public unsafe IntPtr GetPtr()
        {
            return mArray1Ptr;
        }

        public unsafe void Print(int i)
        {
            Debug.LogError(mArrayPoint[i]);
        }

        public static UInt16 ConciseHtons(UInt16 src)
        {
            UInt16 dest = 0;
            unsafe
            {
                ((byte*)&dest)[0] = ((byte*)&src)[1];
                ((byte*)&dest)[1] = ((byte*)&src)[0];
            }
            return dest;
        }
    }

    public class UnsafeTest : MonoBehaviour
    {
        //UnsafeClass1 var1;
        //IEnumerator Start()
        //{
        //    yield return new WaitForSeconds(2.0f);

        //    UInt16 val = 1;
        //    val = UnsafeClass1.ConciseHtons(val);
        //    Debug.LogError(val);

        //    Debug.LogError("------------------------------------------------------");
        //    var1 = new UnsafeClass1();
        //    var1.Init(10000000);
        //    var1.Print(50000);
        //    var1 = null;

        //    yield return new WaitForSeconds(2.0f);

        //    var array2 = new int[10000000];
        //    array2 = null;
        //}

        private UnsafeVector3Array mVecTestArray;
        private int                mMode = 0;

        private List<Vector3>      mVecTestArray1;
        private Vector3[]          mVecTestArray2;

        public  Button             modeButton;
        public  Text               modeDisplayText;

        public  MeshFilter         mCubeMesh;
        public  List<Vector3>      mVecTestArray3;

        public  Mesh               mDestMesh;

        public UnityEngine.Object  mObject;

        void Start()
        {
            mVecTestArray  = new UnsafeVector3Array(1000);
            mVecTestArray1 = new List<Vector3>(2000);
            mVecTestArray2 = new Vector3[2000];
            mVecTestArray3 = new List<Vector3>(mCubeMesh.mesh.vertices);
            mDestMesh = new Mesh();
        }

        Vector3 temp;
        void Update()
        {
            if (this.mMode == 0)
            {
                TestMode_Unsafe();
            }
            else if (this.mMode == 1)
            {
                TestMode_Managed_List();
            }
            else if (this.mMode == 2)
            {
                TestMode_Managed_Array();
            }

            mDestMesh.SetVertices(mVecTestArray3);
        }

        private void TestMode_Unsafe()
        {
            mVecTestArray.Reset();
            for (int i = 0; i < 1000; i++)
            {
                temp.x = i;
                temp.y = i;
                mVecTestArray.Add(temp);
            }
        }

        private void TestMode_Managed_List()
        {
            mVecTestArray1.Clear();
            int nCount = UnityEngine.Random.Range(1000, 2000);
            for (int i = 0; i < nCount; i++)
            {
                temp.x = i;
                temp.y = i;
                mVecTestArray1.Add(temp);
            }
            //mVecTestArray1.ToArray();
            mDestMesh.SetVertices(mVecTestArray1);
        }

        private void TestMode_Managed_Array()
        {
            Array.Resize<Vector3>(ref mVecTestArray2, UnityEngine.Random.Range(1000, 2000));
            for (int i = 0; i < 1000; i++)
            {
                temp.x = i;
                temp.y = i;
                mVecTestArray2[i] = temp;
            }
        }

        public void On_ModeButton_Clicked()
        {
            this.mMode = (this.mMode + 1) % 3;
            if (this.mMode == 0)
            {
                this.modeDisplayText.text = "Unsafe Mode";
            }
            else if (this.mMode == 1)
            {
                this.modeDisplayText.text = "Managed Mode List";
            }
            else if (this.mMode == 2)
            {
                this.modeDisplayText.text = "Managed Mode Array";
            }
        }
    }
}
