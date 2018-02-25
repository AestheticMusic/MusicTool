using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoteCameraData : NoteData
{
    // Pos
    public Vector2 startPos = Vector2.zero;
    public Vector2 endPos = Vector2.zero;
    public int curvePos = 0;

    // Zoom
    public float startZoom = 0f;
    public float endZoom = 0f;
    public int curveZoom = 0;

    // Rot
    public float startRot = 0f;
    public float endRot = 0f;
    public int curveRot = 0;

    public float endTime = 0f;

    public float length
    {
        get
        {
            return endTime - time;
        }
    }

    public int noteType;

    public NoteCameraData()
    {
        type = NoteType.Camera;
    }

    public void Set(NoteCameraData _data)
    {
        this.startPos = _data.startPos;
        this.endPos = _data.endPos;
        this.curvePos = _data.curvePos;

        this.startZoom = _data.startZoom;
        this.endZoom = _data.endZoom;
        this.curveZoom = _data.curveZoom;

        this.startRot = _data.startRot;
        this.endRot = _data.endRot;
        this.curveRot = _data.curveRot;

        this.time = _data.time;
        this.endTime = _data.endTime;
    }

    public override string GetNoteDataString()
    {
        switch (noteType)
        {
            case NoteCamera.N_NONE:
                return "";
            case NoteCamera.N_POS:
                return "POS:" + startPos + ":" + endPos + ":" + curvePos + ":" + Mathf.Floor(time * 1000f) + ":" + Mathf.Floor(endTime * 1000f);
            case NoteCamera.N_ROT:
                return "ROT:" + startRot + ":" + endRot + ":" + curveRot + ":" + Mathf.Floor(time * 1000f) + ":" + Mathf.Floor(endTime * 1000f);
            case NoteCamera.N_ZOOM:
                return "ZOOM:" + startZoom + ":" + endZoom + ":" + curveZoom + ":" + Mathf.Floor(time * 1000f) + ":" + Mathf.Floor(endTime * 1000f);
        }

        return "";
    }

    public NoteCameraData GetClone()
    {
        return (NoteCameraData)this.MemberwiseClone();
    }
}
