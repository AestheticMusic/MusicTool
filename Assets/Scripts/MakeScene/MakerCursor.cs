using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakerCursor : MonoBehaviour
{
	public Transform body;
	public Transform head;

	private SpriteRenderer cursorRenderer;
	private SpriteRenderer headRenderer;
	private SpriteRenderer bodyRenderer;
	private Vector3 noteScale;
	private Vector3 bodyScale;
	private Vector3 headPos;
	private MakeManager g;
	private NoteManager n;
	private bool longCursor = false;
    
	private void Awake()
	{
		cursorRenderer = this.GetComponent<SpriteRenderer>();
		bodyRenderer = body.GetComponent<SpriteRenderer>();
		headRenderer = head.GetComponent<SpriteRenderer>();
		noteScale = this.transform.localScale;
		bodyScale = body.localScale;
		headPos = head.localPosition;
    }

	private void Start()
	{
		g = MakeManager.instance;
		n = NoteManager.instance;
	}

	public void SetColor(Color32 _color)
	{
		Color32 color = _color;
		color.a = 175;
		cursorRenderer.color = color;
		headRenderer.color = color;
	}

	public void SetBodyColor(Color32 _color)
	{
		Color32 color = _color;
		color.a = 175;
		bodyRenderer.color = color;
	}

	public void SetLongCursor(int _lineNum)
	{
		if (longCursor)
			return;

		body.gameObject.SetActive(true);
		head.gameObject.SetActive(true);
		this.transform.parent = g.makerLines[_lineNum].transform;
		longCursor = true;
    }

	public void SetNormalCursor()
	{
		if (!longCursor)
			return;

		body.gameObject.SetActive(false);
		head.gameObject.SetActive(false);
		this.transform.parent = g.makerLines[0].transform.parent;
		longCursor = false;
    }

	public void UpdateLong(float _timeLength, int _lineNum)
	{
		float length = n.TimeToMakerLineLength(_timeLength);
		bodyScale.x = (_lineNum == 0 ? -length : length) / noteScale.x;
		headPos.x = (_lineNum == 0 ? length : -length) / noteScale.x;
		body.localScale = bodyScale;
		head.localPosition = headPos;
	}

	public void SetPosition(Vector3 _worldPos)
	{
		this.transform.position = _worldPos;
	}
}
