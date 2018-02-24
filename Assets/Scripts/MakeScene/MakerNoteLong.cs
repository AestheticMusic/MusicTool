using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakerNoteLong : MakerMusicNote
{
	public static readonly Color32 C_LONG_BODY = new Color32(184, 78, 54, 255);
	public static readonly Color32 C_BATTER_BODY = new Color32(208, 132, 132, 255);

	public Transform body;
	public Transform end;
	public Transform batterStart;

	private SpriteRenderer bodyRenderer;
	private SpriteRenderer endRenderer;
	private Color bodyColor;
    protected NoteManager n;
	private Vector3 bodyScale = Vector3.one;
	private Vector3 endPos = Vector3.zero;
	private Vector3 batterStartPos = Vector3.zero;



	protected override void Awake()
	{
		base.Awake();
		bodyRenderer = body.GetComponent<SpriteRenderer>();
		endRenderer = end.GetComponent<SpriteRenderer>();
		n = NoteManager.instance;
		bodyScale = body.localScale;
		batterStartPos = batterStart.localPosition;
	}

	public override void Start()
	{
		base.Start();
		notePos.z = -1f;
		this.transform.localPosition = notePos;
		if (data.noteType == NoteMusic.N_BATTER)
			batterStart.gameObject.SetActive(true);
		else
			batterStart.gameObject.SetActive(false);
	}
	
	public override void SetColor(int _noteType)
	{
		base.SetColor(_noteType);
		switch (_noteType)
		{
			case NoteMusic.N_LONG:
				bodyRenderer.color = C_LONG_BODY;
				endRenderer.color = C_LONG;
				break;
			case NoteMusic.N_BATTER:
				bodyRenderer.color = C_BATTER_BODY;
				endRenderer.color = C_BATTER;
				break;
		}
		if (musicMake.selectedData == data)
		{
			Color c = bodyRenderer.color;
			c.a = 0.5f;
			bodyRenderer.color = c;
			c = endRenderer.color;
			c.a = 0.5f;
			endRenderer.color = c;
		}
	}

	public void SetLength(float _length)
	{
		float bodyLength = n.TimeToMakerLineLength(_length) / noteScale.x;
		endPos.x = -bodyLength;
		end.localPosition = endPos;

		if (data.noteType == NoteMusic.N_BATTER)
		{
			batterStartPos.x = -n.TimeToMakerLineLength(BatterNote.appearTime) / noteScale.x;
            bodyScale.x = n.TimeToMakerLineLength(_length + BatterNote.disappearTime) / noteScale.x;
		}
		else
			bodyScale.x = bodyLength;
		body.localScale = bodyScale;
		batterStart.localPosition = batterStartPos;
	}
}
