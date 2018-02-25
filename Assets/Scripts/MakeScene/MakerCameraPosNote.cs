using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakerCameraPosNote : MakerCameraNote
{
    private static readonly Color32 C_LONG = new Color32(249, 143, 143, 255);
    private static readonly Color32 C_LONG_BODY = new Color32(184, 78, 54, 255);

    public override void Start()
    {
        base.Start();
        notePos.x = 1.06f; // 2 : 1.06 | 3 : 0.674 | 4 : 0.29

        this.transform.localPosition = notePos;
    }
}
