using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MakeManager : MonoBehaviour
{
    public const int makerVersion = 1;

    public static MakeManager instance;
    
    public const float judgePerfect = 0.216f;
    public const float maxBeatInLine = 4f;
    public const float lineLength = 5.15f;
    public const float oneBeatToLine = lineLength / maxBeatInLine;

    public const int NMODE_DELETE = -2;
    public const int NMODE_SELECT = -1;

    // [HideInInspector]
    public float speed = 1f;
    [HideInInspector]
    public float noteSync = 0f;
    [HideInInspector]
    public float beatRate = 1f / 16f;
    [HideInInspector]
    public int beatRateEnum = 7;
    //[HideInInspector]
    public int noteMode = NMODE_SELECT;
    [HideInInspector]
    public bool playMode = false;
    [HideInInspector]
    public float time = 0f;
    [HideInInspector]
    public float bpmRatio = 1f;
    [HideInInspector]
    public float scrollSpeed = 1f / 16f;
    //[HideInInspector]
    public bool makerCursorEnabled = true;
    [HideInInspector]
    public float cursorTime = 0f;

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

    [HideInInspector]
    public MusicManager musicManager;
    [HideInInspector]
    public UIManager ui;
    [HideInInspector]
    public NoteManager noteManager;

    [HideInInspector]
    public MusicMakeManager musicMake;
    [HideInInspector]
    public CameraMakeManager camMake;

    public bool isCameraNoteMode = false;

    private float maxScroll = 100f;
    private float _bpm = 60.0f;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        musicManager = MusicManager.instance;
        ui = UIManager.instance;
        noteManager = NoteManager.instance;

        musicMake = MusicMakeManager.instance;
        camMake = CameraMakeManager.instance;

        musicManager.LoadMusic(StartManager.instance.music);
    }

    private void Update()
    {
        if (playMode)
        {
            time = musicManager.musicTime;
            camMake.isInput = musicMake.isInput = false;
        }
        else
        {
            if (isCameraNoteMode)
            {
                camMake.isInput = true;
            }
            else {
                musicMake.isInput = true;
            }

            InputControl();
        }

        HotKeyUpdate();
    }

    private void InputControl()
    {
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        time = Mathf.Clamp(time + wheel * 10f / bpmRatio * scrollSpeed * 4f, 0f, musicManager.musicLength);
        if (wheel != 0f)
            ui.ChangeMusicBarIgnored(time);
    }

    public float PageToTime(float _page)
    {
        return (_page - 1f) * 4f / bpmRatio;
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
        ui.ChangeMusicBar(Mathf.Clamp(time + _time, 0f, musicManager.musicLength));
    }

    private void ToggleMusicState()
    {
        if (playMode)
            ui.OnMusicPause();
        else
            ui.OnMusicPlay();
    }

    private void HotKeyUpdate()
    {
        if (!hotkeyEnabled)
            return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.S))
                noteManager.SaveNoteData();

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
                ui.OnMusicStop();
            else
                ToggleMusicState();
        }
    }

}
