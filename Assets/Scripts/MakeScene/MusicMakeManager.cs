using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicMakeManager : MonoBehaviour
{

    public static MusicMakeManager instance;

    //public const int NMODE_DELETE = -2;
    //public const int NMODE_SELECT = -1;
    public const int NMODE_NORMAL = 0;
    public const int NMODE_LONG = 1;
    public const int NMODE_DRAG = 2;
    public const int NMODE_BATTER = 3;


    //[HideInInspector]
    public NoteMusicData selectedData = null;
    //[HideInInspector]
    public NoteMusicData selectedBatter = null;
    [HideInInspector]
    public MakeHistoryStack historyStack;

    private NoteMusicData timeModifyData = null;
    private NoteMusicData timeModifyBefore = null;

    private bool modifyEndTime = false;

    [HideInInspector]
    public bool hotkeyEnabled = true;

    MakeManager makeManager;
    public Transform[] lines;
    public MakerLine[] makerLines;
    public MakerCursor makerCursor;

    public bool isInput = false;
    public bool isRun = false;

    public UIMusicMaker ui;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        historyStack = new MakeHistoryStack();
        historyStack.Initialize();

        makeManager = MakeManager.instance;

        lines = makeManager.lines;
        makerLines = makeManager.makerLines;
        makerCursor = makeManager.makerCursor;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                makerLines[0].keyboardMake = new Vector2(-0.9f, -1.1f);
                makerLines[0].MouseOver();
                makerLines[0].MouseDown();
                return;
            }
            else if (Input.GetKeyUp(KeyCode.Z))
            {
                makerLines[0].MouseUp();
                makerLines[0].keyboardMake = Vector2.zero;
                return;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                makerLines[1].keyboardMake = new Vector2(0.8f, -1.2f);
                makerLines[1].MouseOver();
                makerLines[1].MouseDown();
                return;
            }
            else if (Input.GetKeyUp(KeyCode.C))
            {
                makerLines[1].MouseUp();
                makerLines[1].keyboardMake = Vector2.zero;
                return;
            }
        }
        else {
            makerLines[0].keyboardMake = Vector2.zero;
            makerLines[1].keyboardMake = Vector2.zero;
        }
       
        ModifyMode();
        HotKeyUpdate();
        CursorUpdate();
        InputControl();
    }

    private void InputControl()
    {

        makerLines[0].MouseOver();
        makerLines[1].MouseOver();

        // 왼쪽 클릭
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("MakerNote"))
            {
                MakerMusicNote note = hit.collider.GetComponent<MakerMusicNote>();
                if (note == null)
                    note = hit.collider.GetComponentInParent<MakerMusicNote>();
                if (note == null)
                    return;

                if (makeManager.noteMode == MakeManager.NMODE_DELETE && selectedData == note.data)
                {
                    makeManager.noteManager.RemoveNoteData(selectedData);
                    historyStack.PushHistory(MakeHistory.A_REMOVENOTE, selectedData);
                }
                else if (makeManager.noteMode == MakeManager.NMODE_SELECT || makeManager.noteMode == MakeManager.NMODE_DELETE)
                    selectedData = note.data;
                else if (makeManager.noteMode == NMODE_BATTER && note.data.noteType == NoteMusic.N_BATTER
                    && Input.GetKey(KeyCode.LeftShift))
                {
                    selectedBatter = note.data;
                    ui.OpenBatterHitSet();
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
                MakerMusicNote note = hit.collider.GetComponent<MakerMusicNote>();
                if (note == null && hit.collider.name.Equals("End"))
                {
                    modifyEndTime = true;
                    note = hit.collider.GetComponentInParent<MakerMusicNote>();
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
        if (makeManager.playMode)
            makeManager.makerCursorEnabled = false;
        else if (timeModifyData != null)
            makeManager.makerCursorEnabled = false;

        if (makerCursor.gameObject.activeSelf != makeManager.makerCursorEnabled)
            makerCursor.gameObject.SetActive(makeManager.makerCursorEnabled);

        makeManager.makerCursorEnabled = false;
    }

    public void SetMakerCursorBodyColor(Color32 _color)
    {
        makerCursor.SetBodyColor(_color);
    }

    private void HotKeyUpdate()
    {
        if (!hotkeyEnabled)
            return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Q))
                ui.OnNoteModeChanged(MakeManager.NMODE_SELECT);
            else if (Input.GetKeyDown(KeyCode.W))
                ui.OnNoteModeChanged(MakeManager.NMODE_DELETE);
            else if (Input.GetKeyDown(KeyCode.E))
                ui.OnNoteModeChanged(NMODE_NORMAL);
            else if (Input.GetKeyDown(KeyCode.R))
                ui.OnNoteModeChanged(NMODE_LONG);
            else if (Input.GetKeyDown(KeyCode.T))
                ui.OnNoteModeChanged(NMODE_DRAG);
            else if (Input.GetKeyDown(KeyCode.Y))
                ui.OnNoteModeChanged(NMODE_BATTER);
            else if (Input.GetKeyDown(KeyCode.Z))
                historyStack.UndoHistory();
        }
    }

}
