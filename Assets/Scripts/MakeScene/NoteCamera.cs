using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteCamera : Note
{
    public const int N_NONE = 0;
    public const int N_POS = 1;
    public const int N_ROT = 2;
    public const int N_ZOOM = 3;

    public NoteCameraData data = null;
    public Vector3 notePos = new Vector3();
    public float time = 0f;

    public virtual void Start()
    {
        time = data.time;
        if (this.transform.localScale.x < 0f)
        {
            Vector3 scale = this.transform.localScale;
            scale.x = -scale.x;
            this.transform.localScale = scale;
        }
        this.transform.localPosition = notePos;
    }

    protected virtual void Update()
    {
        this.transform.localPosition = notePos;
    }

}
