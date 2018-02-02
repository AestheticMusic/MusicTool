using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MakeManager : MonoBehaviour
{
	public const int makerVersion = 1;

	public static MakeManager instance;

	public const int NMODE_DELETE = -2;
	public const int NMODE_SELECT = -1;
	public const int NMODE_NORMAL = 0;
	public const int NMODE_LONG = 1;
	public const int NMODE_DRAG = 2;
	public const int NMODE_BATTER = 3;

	public const float judgePerfect = 0.216f;
	public const float maxBeatInLine = 4f;
	public const float lineLength = 5.15f;
	public const float oneBeatToLine = lineLength / maxBeatInLine;

	// [HideInInspector]
	public float speed = 1f;
	[HideInInspector]
	public float noteSync = 0f;
	[HideInInspector]
	public float beatRate = 1f / 16f;
	[HideInInspector]
	public int beatRateEnum = 7;
	[HideInInspector]
	public bool playMode = false;
	[HideInInspector]
	public int noteMode = NMODE_SELECT;
	[HideInInspector]
	public float time = 0f;
	[HideInInspector]
	public float bpmRatio = 1f;
	[HideInInspector]
	public float scrollSpeed = 1f / 16f;
	[HideInInspector]
	public bool makerCursorEnabled = true;
	[HideInInspector]
	public float cursorTime = 0f;
	[HideInInspector]
	public NoteData selectedData = null;
	[HideInInspector]
	public NoteData selectedBatter = null;
	[HideInInspector]
	public MakeHistoryStack historyStack;
	[HideInInspector]
	public bool hotkeyEnabled = true;

	public float bpm
	{
		get
		{
			return _bpm;
		}
		set
		{
			_bpm = value;
			bpmRatio = _bpm / 60f;
		}
	}
	public float syncedTime
	{
		get
		{
			return time + noteSync;
		}
	}
	public float syncedScroll
	{
		get
		{
			return TimeToPage(syncedTime);
		}
	}

	public Sprite[] pageSprites;
	public Transform[] lines;
	public MakerLine[] makerLines;
	public MakerCursor makerCursor;

	private MusicManager m;
	private UIManager u;
	private NoteManager n;
	private float maxScroll = 100f;
	private float _bpm = 60.0f;
	private NoteData timeModifyData = null;
	private NoteData timeModifyBefore = null;
	private bool modifyEndTime = false;



	private void Awake()
	{
		instance = this;
    }

	private void Start()
	{
		m = MusicManager.instance;
		u = UIManager.instance;
		n = NoteManager.instance;

		historyStack = new MakeHistoryStack();
		historyStack.Initialize();

		m.LoadMusic(StartManager.instance.music);
	}

	private void Update()
	{
		if (playMode)
			time = m.musicTime;
		else
			InputControl();

		ModifyMode();
		HotKeyUpdate();
		CursorUpdate();
	}

	private void InputControl()
	{
		float wheel = Input.GetAxis("Mouse ScrollWheel");
		time = Mathf.Clamp(time + wheel * 10f / bpmRatio * scrollSpeed * 4f, 0f, m.musicLength);
		if (wheel != 0f)
			u.ChangeMusicBarIgnored(time);

		makerLines[0].MouseOver();
		makerLines[1].MouseOver();

		// 왼쪽 클릭
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
			if (hit.collider != null && hit.collider.CompareTag("MakerNote"))
			{
				MakerNote note = hit.collider.GetComponent<MakerNote>();
				if (note == null)
					note = hit.collider.GetComponentInParent<MakerNote>();
				if (note == null)
					return;

				if (noteMode == NMODE_DELETE && selectedData == note.data)
				{
					n.RemoveNoteData(selectedData);
					historyStack.PushHistory(MakeHistory.A_REMOVENOTE, selectedData);
				}
				else if (noteMode == NMODE_SELECT || noteMode == NMODE_DELETE)
					selectedData = note.data;
				else if (noteMode == NMODE_BATTER && note.data.noteType == Note.N_BATTER
					&& Input.GetKey(KeyCode.LeftShift))
				{
					selectedBatter = note.data;
					u.OpenBatterHitSet();
				}
			}
			else
			{
				makerLines[0].MouseDown();
				makerLines[1].MouseDown();
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			makerLines[0].MouseUp();
			makerLines[1].MouseUp();
		}
		// 오른쪽 클릭	
		if (Input.GetMouseButtonDown(1))
		{
			Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
			if (hit.collider != null && hit.collider.CompareTag("MakerNote"))
			{
				MakerNote note = hit.collider.GetComponent<MakerNote>();
				if (note == null && hit.collider.name.Equals("End"))
				{
					modifyEndTime = true;
					note = hit.collider.GetComponentInParent<MakerNote>();
				}
				if (note == null)
					return;

				timeModifyData = note.data;
				timeModifyBefore = note.data.GetClone();
            }
		}
		else if (Input.GetMouseButtonUp(1) && timeModifyData != null)
		{
			historyStack.PushHistory(MakeHistory.A_MODIFYNOTE, timeModifyData, timeModifyBefore);
			modifyEndTime = false;
			timeModifyData = null;
			timeModifyBefore = null;
		}
	}

	private void ModifyMode()
	{
		if (timeModifyData != null)
			makerLines[timeModifyData.lineNum].TimeModifyMode(timeModifyData, modifyEndTime);
	}

	private void CursorUpdate()
	{
		if (playMode)
			makerCursorEnabled = false;
		else if (timeModifyData != null)
			makerCursorEnabled = false;

		if (makerCursor.gameObject.activeSelf != makerCursorEnabled)
			makerCursor.gameObject.SetActive(makerCursorEnabled);

		makerCursorEnabled = false;
    }

	public float PageToTime(float _page)
	{
		return (_page - 1f) * 4f / bpmRatio ;
	}

	public float TimeToPage(float _time)
	{
		return _time * bpmRatio / 4f + 1f;
	}

	public static float PageToTime(float _page, float _bpmRatio)
	{
		return (_page - 1f) * 4f / _bpmRatio;
	}

	public static float TimeToPage(float _time, float _bpmRatio)
	{
		return _time * _bpmRatio / 4f + 1f;
	}

	public void ChangeGridSprite(int _gridIndex)
	{
		makerLines[0].SetPageSprite(pageSprites[_gridIndex]);
		makerLines[1].SetPageSprite(pageSprites[_gridIndex]);
	}

	public void SetMakerCursorBodyColor(Color32 _color)
	{
		makerCursor.SetBodyColor(_color);
	}

	public void GoStartScene()
	{
		SceneManager.LoadScene("StartScene");
	}

	private void AddTime(float _time)
	{
		u.ChangeMusicBar(Mathf.Clamp(time + _time, 0f, m.musicLength));
	}

	private void ToggleMusicState()
	{
		if (playMode)
			u.OnMusicPause();
		else
			u.OnMusicPlay();
	}

	private void HotKeyUpdate()
	{
		if (!hotkeyEnabled)
			return;

		if (Input.GetKey(KeyCode.LeftControl))
		{
			if (Input.GetKeyDown(KeyCode.S))
				n.SaveNoteData();
			else if (Input.GetKeyDown(KeyCode.Q))
				u.OnNoteModeChanged(NMODE_SELECT);
			else if (Input.GetKeyDown(KeyCode.W))
				u.OnNoteModeChanged(NMODE_DELETE);
			else if (Input.GetKeyDown(KeyCode.E))
				u.OnNoteModeChanged(NMODE_NORMAL);
			else if (Input.GetKeyDown(KeyCode.R))
				u.OnNoteModeChanged(NMODE_LONG);
			else if (Input.GetKeyDown(KeyCode.T))
				u.OnNoteModeChanged(NMODE_DRAG);
			else if (Input.GetKeyDown(KeyCode.Y))
				u.OnNoteModeChanged(NMODE_BATTER);
			else if (Input.GetKeyDown(KeyCode.Z))
				historyStack.UndoHistory();
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
			AddTime(-1f);
		else if (Input.GetKeyDown(KeyCode.RightArrow))
			AddTime(1f);
		else if (Input.GetKeyDown(KeyCode.DownArrow))
			AddTime(-5f);
		else if (Input.GetKeyDown(KeyCode.UpArrow))
			AddTime(5f);
		else if (Input.GetKeyDown(KeyCode.Space))
		{
			if (Input.GetKey(KeyCode.LeftShift))
				u.OnMusicStop();
			else
				ToggleMusicState();
		}
	}
}
