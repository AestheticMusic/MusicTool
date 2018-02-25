using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public Transform limitLT;
    public Transform limitRB;
    public AnimationCurve[] curves;

    private Camera cam;
    private Vector3 camPos;
    private float camZoom = 3f;
    private Vector3 camRot = Vector3.zero;
    private float screenRatio = 1f;
    public List<NoteCameraData> datas;
    private MakeManager g;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        g = MakeManager.instance;

        cam = Camera.main;
        screenRatio = Screen.width / Screen.height;
    }

    private void Update()
    {
        GetCameraInfo(g.syncedTime, camPos, camZoom, camRot.z);

        float camXSize = camZoom * screenRatio * 0.5f;
        float camYSize = camZoom * 0.5f;
        camPos.x = Mathf.Clamp(camPos.x, limitLT.position.x + camXSize, limitRB.position.x - camXSize);
        camPos.y = Mathf.Clamp(camPos.y, limitRB.position.y + camYSize, limitLT.position.y - camYSize);
        camZoom = Mathf.Clamp(camZoom, 1f, 5.5f);

        cam.transform.position = camPos;
        cam.transform.eulerAngles = camRot;
        cam.orthographicSize = camZoom;
    }

    public void AddNoteCameraData(NoteCameraData _data) {
        datas.Add(_data);
    }

    public void GetCameraInfo(float _time, Vector3 _pos, float _zoom, float _rot)
    {
        List<NoteCameraData> data = GetCameraData(_time);
        
        if (data.Count > 0)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                float t = Mathf.Clamp01((_time - data[i].time) / data[i].length);
                switch (data[i].noteType)
                {
                    case NoteCamera.N_POS:
                        camPos = Vector3.Lerp(data[i].startPos, data[i].endPos, curves[data[i].curvePos].Evaluate(t));
                        break;
                    case NoteCamera.N_ROT:
                        camRot.z = Mathf.Lerp(data[i].startRot, data[i].endRot, curves[data[i].curveRot].Evaluate(t));
                        break;
                    case NoteCamera.N_ZOOM:
                        camZoom = Mathf.Lerp(data[i].startZoom, data[i].endZoom, curves[data[i].curveZoom].Evaluate(t));
                        break;
                }
                _pos.z = -10f;
            }
        }
    }

    private List<NoteCameraData> GetCameraData(float _time)
    {
        List<NoteCameraData> res = new List<NoteCameraData>();
        for (int i = 0; i < datas.Count; ++i)
        {
            NoteCameraData data = datas[i];
            if (data.time <= _time)
            {
                res.Add(data);
            }
        }

        return res;
    }
}
