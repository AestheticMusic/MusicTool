using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakerCameraNote : MakerNote
{
    public static readonly Color32 C_LONG = new Color32(249, 143, 143, 255);
    public static readonly Color32 C_LONG_BODY = new Color32(184, 78, 54, 255);

    protected static readonly Vector3 noteScale = new Vector3(0.15f, 0.8f, 1f);

    private static readonly Vector3 noteRotation = new Vector3(0f, 0f, -90f);

    public Transform body;
    public Transform end;

    private SpriteRenderer bodyRenderer;
    private SpriteRenderer endRenderer;

    private Color bodyColor;

    protected MakeManager makeManager;
    protected NoteManager noteManager;
    protected CameraMakeManager camMake;

    private Vector3 bodyScale = Vector3.one;
    private Vector3 endPos = Vector3.zero;

    public Vector3 notePos;
    public NoteCameraData data = null;

    private SpriteRenderer noteRenderer;
    private Color noteColor;

    protected void Awake()
    {
        noteRenderer = this.GetComponent<SpriteRenderer>();
        makeManager = MakeManager.instance;
        camMake = CameraMakeManager.instance;
        bodyRenderer = body.GetComponent<SpriteRenderer>();
        endRenderer = end.GetComponent<SpriteRenderer>();
        noteManager = NoteManager.instance;
        bodyScale = body.localScale;
    }

    public void Start()
    {
        notePos.x = 1.02f;
        notePos.z = -2f;
        this.transform.localScale = noteScale;
        this.transform.localEulerAngles = noteRotation;
        this.transform.localPosition = notePos;

        notePos.z = -1f;
        this.transform.localPosition = notePos;
    }

    public void SetColor()
    {
        noteRenderer.color = C_LONG;
        bodyRenderer.color = C_LONG_BODY;
        endRenderer.color = C_LONG;

        if (camMake.selectedData == data)
        {
            Color c = bodyRenderer.color;
            c.a = 0.5f;
            bodyRenderer.color = c;
            c = endRenderer.color;
            c.a = 0.5f;
            endRenderer.color = c;
        }
    }

}
