using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMakeManager : MonoBehaviour
{

    public static CameraMakeManager instance;

    //public const int NMODE_DELETE = -2;
    //public const int NMODE_SELECT = -1;
    public const int NMODE_POS = 0;
    public const int NMODE_ROT = 1;
    public const int NMODE_ZOOM = 2;

    [HideInInspector]
    public NoteCameraData selectedData = null;
    [HideInInspector]
    public MakeHistoryStack historyStack;

    private NoteCameraData timeModifyData = null;
    private NoteCameraData timeModifyBefore = null;

    private bool modifyEndTime = false;

    [HideInInspector]
    public bool hotkeyEnabled = true;

    MakeManager makeManager;
    public Transform[] lines;
    public MakerLine[] makerLines;
    public MakerCursor makerCursor;

    public bool isInput = false;
    public bool isRun = false;

    public UICameraMaker ui;

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
        if (!isInput)
            return;

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
                MakerCameraNote note = hit.collider.GetComponent<MakerCameraNote>();
                if (note == null)
                    note = hit.collider.GetComponentInParent<MakerCameraNote>();
                if (note == null)
                    return;

                if (makeManager.noteMode == MakeManager.NMODE_DELETE && selectedData == note.data)
                {
                    makeManager.noteManager.RemoveNoteData(selectedData);
                    historyStack.PushHistory(MakeHistory.A_REMOVENOTE, selectedData);
                }
                else if (makeManager.noteMode == MakeManager.NMODE_SELECT || makeManager.noteMode == MakeManager.NMODE_DELETE)
                    selectedData = note.data;
                else if (makeManager.noteMode == NMODE_POS && note.data.noteType == NoteCamera.N_POS
                    && Input.GetKey(KeyCode.LeftShift))
                {
                    selectedData = note.data;
                    ui.OpenPos();
                }
                else if (makeManager.noteMode == NMODE_ROT && note.data.noteType == NoteCamera.N_ROT
                     && Input.GetKey(KeyCode.LeftShift))
                {
                    selectedData = note.data;
                    ui.OpenRot();
                }
                else if (makeManager.noteMode == NMODE_ZOOM && note.data.noteType == NoteCamera.N_ZOOM
                    && Input.GetKey(KeyCode.LeftShift))
                {
                    selectedData = note.data;
                    ui.OpenZoom();
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
                MakerCameraNote note = hit.collider.GetComponent<MakerCameraNote>();
                if (note == null && hit.collider.name.Equals("End"))
                {
                    modifyEndTime = true;
                    note = hit.collider.GetComponentInParent<MakerCameraNote>();
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
                ui.OnNoteModeChanged(NMODE_POS);
            else if (Input.GetKeyDown(KeyCode.R))
                ui.OnNoteModeChanged(NMODE_ROT);
            else if (Input.GetKeyDown(KeyCode.T))
                ui.OnNoteModeChanged(NMODE_ZOOM);
            else if (Input.GetKeyDown(KeyCode.Z))
                historyStack.UndoHistory();
        }
    }

}
