using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

    LineRenderer lineRenderer;

    List<Vector3> points;

    [SerializeField]
    int numLinePoint;

    [SerializeField]
    float freq;

    [SerializeField]
    Vector3 offset;

    [SerializeField]
    [Range(0f,1f)]
    float detail;

    // Use this for initialization
    void Start () {
        lineRenderer = this.GetComponent<LineRenderer>();


    }
	
	// Update is called once per frame
	void Update () {
        DrawLine();

    }

    void DrawLine()
    {
        lineRenderer.numPositions = numLinePoint;
        points = new List<Vector3>();

        float t = Time.time;

        for (int i = 0; i < numLinePoint; i++)
        {
            //var p1 = new Vector3(i * detail, 4 * Mathf.Sin(i * detail + (t * freq)), 0);
            //var p2 = new Vector3(i * detail, 0.5f * Mathf.Sin(i * detail * 2f + (t * freq)), 0);
            //var p3 = new Vector3(i * detail, 2 * Mathf.Sin(i * detail * 3f + (t * freq)), 0);
            //var p4 = new Vector3(i * detail, 1 * Mathf.Sin(i * detail * 4f + (t * freq)), 0);
            //points.Add(p1 + p2 + p3 + p4 + offset);
            points.Add(new Vector3(i * detail, Random(Vector3.one * i), 0));
        }
        lineRenderer.SetPositions(points.ToArray());

        //Debug.Log("Random : " + Random(Vector3.one * t));
    }

    //float noise(Vector2 n)
    //{
    //    var d = new Vector2(0.0f, 1.0f);
    //    var b = Vector2.Floor(n);
    //    var f = Vector2.s(vec2(0.0), vec2(1.0), fract(n));
    //    return mix(mix(rand(b), rand(b + d.yx), f.x), mix(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
    //}

    //float noise(float x, float y)
    //{
    
    //// 注目する点を囲む格子の頂点の値
    //    float v00 = value((int)x, (int)y);
    //    float v10 = value((int)x + 1, (int)y);
    //    float v01 = value((int)x, (int)y + 1);
    //    float v11 = value((int)x + 1, (int)y + 1);

    //    float tx = x - (int)x; // 入力から整数部を引いて少数部を取り出す
    //    float ty = y - (int)y;

    //    tx = Interpolate(tx); // 滑らかになるように曲線に変換
    //    ty = Interpolate(ty);

    //    float v0010 = Mix(v00, v10, tx);
    //    float v0111 = Mix(v01, v11, tx);
    //    return Mix(v0010, v0111, ty);
    //}

    float Random(Vector2 st)
    {
        return Frac(Mathf.Sin(Vector2.Dot(st, new Vector2(12.9898f, 78.233f))) * 43758.5453123f);
    }

    float Interpolate(float t)
    {
        return t * t * t * (10.0f + t * (-15.0f + 6.0f * t));
    }

    float Mix(float a, float b, float t)
    {
        return a * (1.0f - t) + b * t;
    }

    float Frac(float t)
    {
        return t - Mathf.Floor(t);
    }


}
