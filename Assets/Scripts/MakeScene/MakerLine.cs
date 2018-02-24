using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakerLine : MonoBehaviour
{
    public const float pageHeight = 5.119f;
    private const float noteHeight = 0.15f;

    public int lineNumber = 0;

    public Page page0, page1, page2;
    public List<NoteMusic> loadedNotes;

    private MakeManager g;
    private NoteManager n;
    private UIManager u;

    private NoteMusicData longMusicData = null;
    private NoteCameraData longCameraData = null;

    private bool mouseOver = false;

    private void Start()
    {
        g = MakeManager.instance;
        n = NoteManager.instance;
        u = UIManager.instance;
        loadedNotes = new List<NoteMusic>();
    }

    private void Update()
    {
        float syncedScroll = MakeManager.instance.syncedScroll;
        float pageScroll = syncedScroll % 3f - 2f;
        Vector3 pos = new Vector3(0f, pageScroll * -pageHeight, 0f);
        page0.transform.localPosition = pos;
        page0.SetPageNumber((int)(syncedScroll - pageScroll));

        pageScroll = (syncedScroll + 1f) % 3f - 2f;
        pos = new Vector3(0f, pageScroll * -pageHeight, 0f);
        page1.transform.localPosition = pos;
        page1.SetPageNumber((int)(syncedScroll - pageScroll));

        pageScroll = (syncedScroll + 2f) % 3f - 2f;
        pos = new Vector3(0f, pageScroll * -pageHeight, 0f);
        page2.transform.localPosition = pos;
        page2.SetPageNumber((int)(syncedScroll - pageScroll));
    }

    private void LateUpdate()
    {
        mouseOver = false;
    }

    public void SetPageSprite(Sprite _spr)
    {
        page0.spriteRenderer.sprite = _spr;
        page1.spriteRenderer.sprite = _spr;
        page2.spriteRenderer.sprite = _spr;
    }

    public void MouseOver()
    {
        if (g.playMode)
            return;

        if (g.noteMode == MakeManager.NMODE_DELETE || g.noteMode == MakeManager.NMODE_SELECT)
            return;
        
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (worldPos.y > -1f || (lineNumber == 0 && worldPos.x > 0) || (lineNumber == 1 && worldPos.x < 0))
            return;

        Vector3 localPos = this.transform.InverseTransformPoint(worldPos);
        localPos.x = 1.02f;
        localPos.z = 0f;

        float cursorScroll = g.syncedScroll + localPos.y / pageHeight;
        float beatDifference = cursorScroll % g.beatRate;
        localPos.y = (cursorScroll - g.syncedScroll - beatDifference) * pageHeight + 0.02f;
        if (lineNumber == 1)
            localPos.y += noteHeight;
        g.cursorTime = g.PageToTime(cursorScroll - beatDifference);
        worldPos = this.transform.TransformPoint(localPos);

        g.makerCursor.SetPosition(worldPos);
        g.makerCursorEnabled = true;
        mouseOver = true;

        if (longMusicData != null)
        {
            /*
			localPos.y = (g.TimeToPage(longData.time) - g.syncedScroll) * pageHeight + 0.02f;
			if (lineNumber == 1)
				localPos.y += noteHeight;
			worldPos = this.transform.TransformPoint(localPos);
			g.makerCursor.SetLongHead(worldPos);
			*/
            g.makerCursor.SetLongCursor(lineNumber);
            g.makerCursor.UpdateLong(g.cursorTime - longMusicData.time, lineNumber);
        }
        else
            g.makerCursor.SetNormalCursor();
    }

    public void MouseDown()
    {
        if (!mouseOver)
            return;
        if (g.noteMode == MakeManager.NMODE_DELETE || g.noteMode == MakeManager.NMODE_SELECT)
            return;

        if (g.iscameraNoteMode)
        {
            NoteCameraData data = new NoteCameraData();
            data.time = g.cursorTime;
            data.lineNum = lineNumber;
            switch (g.noteMode)
            {
                case CameraMakeManager.NMODE_POS:
                case CameraMakeManager.NMODE_ROT:
                case CameraMakeManager.NMODE_ZOOM:
                    longCameraData = data;
                    break;
            }
            g.camMake.historyStack.PushHistory(MakeHistory.A_ADDNOTE, data);
        }
        else {
            NoteMusicData data = new NoteMusicData();
            data.time = g.cursorTime;
            data.lineNum = lineNumber;
            switch (g.noteMode)
            {
                case MusicMakeManager.NMODE_NORMAL:
                    n.AddNoteData(data);
                    break;
                case MusicMakeManager.NMODE_DRAG:
                    data.drag = (Input.GetKey(KeyCode.LeftShift)) ? -1 : 1;
                    n.AddNoteData(data);
                    break;
                case MusicMakeManager.NMODE_LONG:
                case MusicMakeManager.NMODE_BATTER:
                    longMusicData = data;
                    break;
            }
            g.musicMake.historyStack.PushHistory(MakeHistory.A_ADDNOTE, data);
        }

    }

    public void MouseUp()
    {
        if (!(g.noteMode == MusicMakeManager.NMODE_LONG || g.noteMode == MusicMakeManager.NMODE_BATTER))
            return;

        if (g.iscameraNoteMode)
        {
            if (longCameraData == null)
                return;
            switch (g.noteMode)
            {
                case CameraMakeManager.NMODE_POS:
                case CameraMakeManager.NMODE_ROT:
                case CameraMakeManager.NMODE_ZOOM:
                    longMusicData.longEndTime = Mathf.Clamp(g.cursorTime, longMusicData.time + (0.25f / g.bpmRatio), 999f);
                    break;
            }

            n.AddNoteData(longCameraData);
            g.camMake.historyStack.PushHistory(MakeHistory.A_ADDNOTE, longMusicData);

            longCameraData = null;
        }
        else {
            if (longMusicData == null)
                return;
            switch (g.noteMode)
            {
                case MusicMakeManager.NMODE_LONG:
                    longMusicData.longEndTime = Mathf.Clamp(g.cursorTime, longMusicData.time + (0.25f / g.bpmRatio), 999f);
                    break;
                case MusicMakeManager.NMODE_BATTER:
                    longMusicData.batterEndTime = Mathf.Clamp(g.cursorTime, longMusicData.time + (0.5f / g.bpmRatio) + BatterNote.appearTime, 999f);
                    longMusicData.batterHit = 2;
                    g.musicMake.selectedBatter = longMusicData;
                    u.musicMaker.OpenBatterHitSet();
                    break;
            }

            n.AddNoteData(longMusicData);
            g.musicMake.historyStack.PushHistory(MakeHistory.A_ADDNOTE, longMusicData);

            longMusicData = null;
        }

    }

    public void TimeModifyMode(NoteMusicData _data, bool _isEndTime)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 localPos = this.transform.InverseTransformPoint(worldPos);
        localPos.x = 1.02f;
        localPos.z = 0f;

        float cursorScroll = g.syncedScroll + localPos.y / pageHeight;
        float beatDifference = cursorScroll % g.beatRate;
        localPos.y = (cursorScroll - g.syncedScroll - beatDifference) * pageHeight + 0.02f;
        if (lineNumber == 1)
            localPos.y += noteHeight;
        float cursorTime = g.PageToTime(cursorScroll - beatDifference);
        if (_isEndTime)
        {
            if (_data.noteType == NoteMusic.N_LONG)
                _data.longEndTime = Mathf.Clamp(cursorTime, _data.time + (0.25f / g.bpmRatio), 999f);
            else if (_data.noteType == NoteMusic.N_BATTER)
            {
                _data.batterEndTime = Mathf.Clamp(cursorTime, _data.time + (0.5f / g.bpmRatio) + BatterNote.appearTime, 999f);
                if (_data.batterHit > _data.GetMaxBatterHit())
                    _data.batterHit = _data.GetMaxBatterHit();
            }
        }
        else
        {
            if (_data.noteType == NoteMusic.N_LONG)
                _data.time = Mathf.Clamp(cursorTime, -999f, _data.longEndTime - (0.25f / g.bpmRatio));
            else if (_data.noteType == NoteMusic.N_BATTER)
            {
                _data.time = Mathf.Clamp(cursorTime, -999f, _data.batterEndTime - (0.5f / g.bpmRatio) - BatterNote.appearTime);
                if (_data.batterHit > _data.GetMaxBatterHit())
                    _data.batterHit = _data.GetMaxBatterHit();
            }
            else
                _data.time = cursorTime;
        }
    }

    public void TimeModifyMode(NoteCameraData _data, bool _isEndTime)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 localPos = this.transform.InverseTransformPoint(worldPos);
        localPos.x = 1.02f;
        localPos.z = 0f;

        float cursorScroll = g.syncedScroll + localPos.y / pageHeight;
        float beatDifference = cursorScroll % g.beatRate;
        localPos.y = (cursorScroll - g.syncedScroll - beatDifference) * pageHeight + 0.02f;
        if (lineNumber == 1)
            localPos.y += noteHeight;
        float cursorTime = g.PageToTime(cursorScroll - beatDifference);
        if (_isEndTime)
        {
            _data.endTime = Mathf.Clamp(cursorTime, _data.time + (0.25f / g.bpmRatio), 999f);

        }
        else
        {
            _data.time = Mathf.Clamp(cursorTime, -999f, _data.endTime - (0.25f / g.bpmRatio));
        }
    }

}