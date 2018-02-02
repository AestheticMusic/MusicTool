using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakerNote : MonoBehaviour
{
	public static readonly Color32 C_NORMAL = new Color32(58, 206, 227, 255);
	public static readonly Color32 C_LONG = new Color32(238, 115, 60, 255);
	public static readonly Color32 C_DRAG = new Color32(214, 104, 188, 255);
	public static readonly Color32 C_BATTER = new Color32(249, 143, 143, 255);

	protected static readonly Vector3 noteScale = new Vector3(0.15f, 0.8f, 1f);

	private static readonly Vector3 noteRotation = new Vector3(0f, 0f, -90f);

	public Vector3 notePos;
	public NoteData data = null;

	protected MakeManager g;

	private SpriteRenderer noteRenderer;
	private Color noteColor;



	protected virtual void Awake()
	{
		noteRenderer = this.GetComponent<SpriteRenderer>();
		g = MakeManager.instance;
	}

	public virtual void Start()
	{
		notePos.x = 1.02f;
		notePos.z = -2f;
		this.transform.localScale = noteScale;
        this.transform.localEulerAngles = noteRotation;
        this.transform.localPosition = notePos;
	}
	
	public virtual void SetColor(int _noteType)
	{
		switch (_noteType)
		{
			case Note.N_NORMAL:
				noteRenderer.color = C_NORMAL;
				break;
			case Note.N_LONG:
				noteRenderer.color = C_LONG;
				break;
			case Note.N_DRAG:
				noteRenderer.color = C_DRAG;
				break;
			case Note.N_BATTER:
				noteRenderer.color = C_BATTER;
				break;
		}
		if (g.selectedData == data)
		{
			Color c = noteRenderer.color;
			c.a = 0.5f;
			noteRenderer.color = c;
        }
	}
}
