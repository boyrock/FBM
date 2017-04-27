using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

public class Picture : MonoBehaviour
{
    [SerializeField]
    ComputeShader cp_shader;
    [SerializeField]
    int numBufferTouchPoint;

    ComputeBuffer cp_touchDataBuffer;
    Renderer renderer;

    TouchData[] tpDatas;

    int BLOCK_SIZE = 8;

    int i = 0;
    // Use this for initialization
    void Start()
    {
        InitBuffer();
        renderer = this.GetComponent<Renderer>();
    }

    void InitBuffer()
    {
        tpDatas = new TouchData[numBufferTouchPoint];

        for (int i = 0; i < numBufferTouchPoint; i++)
        {
            TouchData touchData = new TouchData();
            touchData.tpPoint = Vector3.zero;
            touchData.lifeTime = 0;
            tpDatas[i] = touchData;
        }

        cp_touchDataBuffer = new ComputeBuffer(tpDatas.Length, Marshal.SizeOf(typeof(TouchData)));
        cp_touchDataBuffer.SetData(tpDatas);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
            DrawLine();

        var num_active_blocks = (cp_touchDataBuffer.count / BLOCK_SIZE) + cp_touchDataBuffer.count % BLOCK_SIZE;

        cp_shader.SetBuffer(0, "touchDataList", cp_touchDataBuffer);
        cp_shader.SetFloat("deltaTime", Time.deltaTime);
        cp_shader.Dispatch(0, num_active_blocks, 1, 1);

        renderer.material.SetBuffer("touchDataList", cp_touchDataBuffer);
        renderer.material.SetInt("touchCount", numBufferTouchPoint);
    }

    void DrawLine()
    {
        cp_shader.SetVector("touchPoint", Input.mousePosition);
        cp_shader.SetInt("i", i);
        cp_shader.SetBuffer(1, "touchDataList", cp_touchDataBuffer);
        cp_shader.Dispatch(1, 1, 1, 1);

        i++;
        if (i == numBufferTouchPoint)
            i = 0;
    }
    void OnDisable()
    {
        ReleaseBuffer();
    }

    private void ReleaseBuffer()
    {
        if (cp_touchDataBuffer != null)
            cp_touchDataBuffer.Release();
    }

    struct TouchData
    {
        public Vector2 tpPoint;
        public float lifeTime;
    }
}
