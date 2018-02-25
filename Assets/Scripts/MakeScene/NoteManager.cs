using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public static NoteManager instance;
    private const int maxType = 4;

    public GameObject[] notePrafabs;
    public GameObject makerNotePrafab;
    public GameObject makerNoteLongPrafab;
    public GameObject makerNoteCameraPosPrefab;
    public GameObject makerNoteCameraRotPrefab;
    public GameObject makerNoteCameraZoomPrefab;

    private float noteMinTime = 0f;
    private float noteMaxTime = 0f;
    public List<NoteData> datas;
    private List<NoteData> targetDatas;
    private List<NoteMusic>[] notePools;
    private List<MakerMusicNote> makerNotePool;
    private List<MakerNoteLong> makerNoteLongPool;
    private List<MakerCameraPosNote> makerCameraPosNotePool;
    private List<MakerCameraRotNote> makerCameraRotNotePool;
    private List<MakerCameraZoomNote> makerCameraZoomNotePool;
    private int[] notePoolHeads;
    private int makerNotePoolHead = 0;
    private int makerNoteLongPoolHead = 0;
    private int makerCameraPosNotePoolHead = 0;
    private int makerCameraRotNotePoolHead = 0;
    private int makerCameraZoomNotePoolHead = 0;

    private MakeManager g;
    private MusicManager m;
    private SaveManager s;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        g = MakeManager.instance;
        m = MusicManager.instance;
        s = SaveManager.instance;

        NoteDataReader reader = new NoteDataReader();
        reader.ReadData(StartManager.instance.noteData);

        g.bpm = reader.startBPM;
        g.noteSync = reader.noteSync;
        datas = reader.datas;
        targetDatas = new List<NoteData>();

        InitializePool();
    }

    private void Update()
    {
        targetDatas.Clear();
        CameraManager.instance.datas.Clear();
        UpdateNote();
    }

    void UpdateNote()
    {
        noteMinTime = Mathf.Clamp(g.syncedTime, 0f, m.musicLength);
        noteMaxTime = Mathf.Clamp(g.syncedTime + MakeManager.maxBeatInLine / g.bpmRatio * 2f, 0f, m.musicLength);

        foreach (NoteData data in datas)
        {
            if (data.type == NoteType.Music)
            {
                NoteMusicData musicData = (NoteMusicData)data;
                if (musicData.time >= noteMinTime &&
                    musicData.time <= noteMaxTime && musicData.noteType != NoteMusic.N_BATTER)
                    targetDatas.Add(musicData);
                else if (musicData.noteType == NoteMusic.N_LONG &&
                    musicData.longEndTime >= noteMinTime && musicData.time <= noteMaxTime)
                    targetDatas.Add(musicData);
                else if (musicData.noteType == NoteMusic.N_BATTER &&
                    musicData.batterEndTime >= noteMinTime - BatterNote.disappearTime && musicData.time <= noteMaxTime)
                    targetDatas.Add(musicData);
            }
            else {
                NoteCameraData cameraData = (NoteCameraData)data;
                if (cameraData.endTime >= noteMinTime && cameraData.time <= noteMaxTime)
                {
                    targetDatas.Add(cameraData);
                    CameraManager.instance.AddNoteCameraData(cameraData);
                }
            }
        }

        Vector3 notePos = new Vector3();
        foreach (NoteData data in targetDatas)
        {
            if (data.type == NoteType.Music)
            {
                NoteMusicData musicData = (NoteMusicData)data;
                float timeDiff = musicData.time - g.syncedTime;
                float length = TimeToLineLength(timeDiff);

                NoteMusic note = PopMusicNote(musicData.noteType);
                notePos.x = length;
                notePos.z = musicData.time * 0.1f;
                if (musicData.noteType == NoteMusic.N_BATTER)
                {
                    note.notePos = Vector3.zero;
                    note.transform.parent = g.lines[0].parent;
                }
                else
                {
                    note.notePos = notePos;
                    note.transform.parent = g.lines[data.lineNum];
                }
                note.data = musicData;
                note.Start();

                MakerMusicNote mn = null;
                if (musicData.noteType == NoteMusic.N_LONG)
                {
                    mn = PopMakerNoteLong();
                    ((MakerNoteLong)mn).SetLength(musicData.longEndTime - musicData.time);
                }
                else if (musicData.noteType == NoteMusic.N_BATTER)
                {
                    mn = PopMakerNoteLong();
                    ((MakerNoteLong)mn).SetLength(musicData.batterEndTime - musicData.time);

                    MakerNoteLong mn2 = PopMakerNoteLong();
                    mn2.SetLength(musicData.batterEndTime - musicData.time);
                    mn2.SetColor(NoteMusic.N_BATTER);
                    mn2.notePos.y = TimeToMakerLineLength(timeDiff);
                    mn2.transform.parent = g.makerLines[1 - musicData.lineNum].transform;
                    mn2.data = musicData;
                    mn2.Start();
                }
                else
                    mn = PopMakerNote();

                mn.notePos.y = TimeToMakerLineLength(timeDiff);
                mn.transform.parent = g.makerLines[musicData.lineNum].transform;
                mn.data = musicData;
                mn.SetColor(musicData.noteType);
                mn.Start();
            }
            else {
                NoteCameraData cameraData = (NoteCameraData)data;

                float timeDiff = cameraData.time - g.syncedTime;
                float length = TimeToLineLength(timeDiff);

                MakerCameraNote cn = null;

                if (cameraData.noteType == NoteCamera.N_POS)
                {
                    cn = PopMakerNoteCameraPos();
                    cn.SetLength(cameraData.endTime - cameraData.time);
                }
                else if (cameraData.noteType == NoteCamera.N_ROT)
                {
                    cn = PopMakerNoteCameraRot();
                    cn.SetLength(cameraData.endTime - cameraData.time);
                }
                else if (cameraData.noteType == NoteCamera.N_ZOOM)
                {
                    cn = PopMakerNoteCameraZoom();
                    cn.SetLength(cameraData.endTime - cameraData.time);
                }
                
                cn.notePos.y = TimeToMakerLineLength(timeDiff);
                cn.transform.parent = g.makerLines[cameraData.lineNum].transform;
                cn.data = cameraData;
                cn.SetColor();
                cn.Start();
            }
        }

        DisableRestNotes();
    }

    private void InitializePool()
    {
        notePools = new List<NoteMusic>[maxType];
        notePoolHeads = new int[maxType];
        for (int i = 0; i < maxType; ++i)
        {
            notePoolHeads[i] = 0;
            notePools[i] = new List<NoteMusic>();
            for (int j = 0; j < 5; ++j)
            {
                NoteMusic note = Instantiate<GameObject>(notePrafabs[i]).GetComponent<NoteMusic>();
                note.gameObject.SetActive(false);
                notePools[i].Add(note);
            }
        }

        makerNotePool = new List<MakerMusicNote>();
        for (int i = 0; i < 5; ++i)
        {
            MakerMusicNote note = Instantiate<GameObject>(makerNotePrafab).GetComponent<MakerMusicNote>();
            note.gameObject.SetActive(false);
            makerNotePool.Add(note);
        }

        makerNoteLongPool = new List<MakerNoteLong>();
        for (int i = 0; i < 5; ++i)
        {
            MakerNoteLong note = Instantiate<GameObject>(makerNoteLongPrafab).GetComponent<MakerNoteLong>();
            note.gameObject.SetActive(false);
            makerNoteLongPool.Add(note);
        }

        makerCameraPosNotePool = new List<MakerCameraPosNote>();
        for (int i = 0; i < 5; ++i)
        {
            MakerCameraPosNote note = Instantiate<GameObject>(makerNoteCameraPosPrefab).GetComponent<MakerCameraPosNote>();
            note.gameObject.SetActive(false);
            makerCameraPosNotePool.Add(note);
        }

        makerCameraRotNotePool = new List<MakerCameraRotNote>();
        for (int i = 0; i < 5; ++i)
        {
            MakerCameraRotNote note = Instantiate<GameObject>(makerNoteCameraRotPrefab).GetComponent<MakerCameraRotNote>();
            note.gameObject.SetActive(false);
            makerCameraRotNotePool.Add(note);
        }

        makerCameraZoomNotePool = new List<MakerCameraZoomNote>();
        for (int i = 0; i < 5; ++i)
        {
            MakerCameraZoomNote note = Instantiate<GameObject>(makerNoteCameraZoomPrefab).GetComponent<MakerCameraZoomNote>();
            note.gameObject.SetActive(false);
            makerCameraZoomNotePool.Add(note);
        }
    }

    private NoteMusic PopMusicNote(int _type)
    {
        NoteMusic note = null;
        if (notePools[_type].Count == notePoolHeads[_type])
        {
            note = Instantiate<GameObject>(notePrafabs[_type]).GetComponent<NoteMusic>();
            notePools[_type].Add(note);
        }
        else
            note = notePools[_type][notePoolHeads[_type]];

        ++notePoolHeads[_type];
        note.gameObject.SetActive(true);
        return note;
    }

    private MakerMusicNote PopMakerNote()
    {
        MakerMusicNote note = null;
        if (makerNotePool.Count == makerNotePoolHead)
        {
            note = Instantiate<GameObject>(makerNotePrafab).GetComponent<MakerMusicNote>();
            makerNotePool.Add(note);
        }
        else
            note = makerNotePool[makerNotePoolHead];

        ++makerNotePoolHead;
        note.gameObject.SetActive(true);
        return note;
    }

    private MakerNoteLong PopMakerNoteLong()
    {
        MakerNoteLong note = null;
        if (makerNoteLongPool.Count == makerNoteLongPoolHead)
        {
            note = Instantiate<GameObject>(makerNoteLongPrafab).GetComponent<MakerNoteLong>();
            makerNoteLongPool.Add(note);
        }
        else
            note = makerNoteLongPool[makerNoteLongPoolHead];

        ++makerNoteLongPoolHead;
        note.gameObject.SetActive(true);
        return note;
    }

    private MakerCameraPosNote PopMakerNoteCameraPos()
    {
        MakerCameraPosNote note = null;
        if (makerCameraPosNotePool.Count == makerCameraPosNotePoolHead)
        {
            note = Instantiate<GameObject>(makerNoteCameraPosPrefab).GetComponent<MakerCameraPosNote>();
            makerCameraPosNotePool.Add(note);
        }
        else
            note = makerCameraPosNotePool[makerCameraPosNotePoolHead];

        ++makerCameraPosNotePoolHead;
        note.gameObject.SetActive(true);
        return note;
    }

    private MakerCameraRotNote PopMakerNoteCameraRot()
    {
        MakerCameraRotNote note = null;
        if (makerCameraRotNotePool.Count == makerCameraRotNotePoolHead)
        {
            note = Instantiate<GameObject>(makerNoteCameraRotPrefab).GetComponent<MakerCameraRotNote>();
            makerCameraRotNotePool.Add(note);
        }
        else
            note = makerCameraRotNotePool[makerCameraRotNotePoolHead];

        ++makerCameraRotNotePoolHead;
        note.gameObject.SetActive(true);
        return note;
    }

    private MakerCameraZoomNote PopMakerNoteCameraZoom()
    {
        MakerCameraZoomNote note = null;
        if (makerCameraZoomNotePool.Count == makerCameraZoomNotePoolHead)
        {
            note = Instantiate<GameObject>(makerNoteCameraZoomPrefab).GetComponent<MakerCameraZoomNote>();
            makerCameraZoomNotePool.Add(note);
        }
        else
            note = makerCameraZoomNotePool[makerCameraZoomNotePoolHead];

        ++makerCameraZoomNotePoolHead;
        note.gameObject.SetActive(true);
        return note;
    }

    private void DisableRestNotes()
    {
        for (int t = 0; t < maxType; ++t)
        {
            for (int i = notePoolHeads[t]; i < notePools[t].Count; ++i)
                notePools[t][i].gameObject.SetActive(false);
            notePoolHeads[t] = 0;
        }

        for (int i = makerNotePoolHead; i < makerNotePool.Count; ++i)
            makerNotePool[i].gameObject.SetActive(false);
        makerNotePoolHead = 0;

        for (int i = makerNoteLongPoolHead; i < makerNoteLongPool.Count; ++i)
            makerNoteLongPool[i].gameObject.SetActive(false);
        makerNoteLongPoolHead = 0;

        for (int i = makerCameraPosNotePoolHead; i < makerCameraPosNotePool.Count; ++i)
            makerCameraPosNotePool[i].gameObject.SetActive(false);
        makerCameraPosNotePoolHead = 0;

        for (int i = makerCameraRotNotePoolHead; i < makerCameraRotNotePool.Count; ++i)
            makerCameraRotNotePool[i].gameObject.SetActive(false);
        makerCameraRotNotePoolHead = 0;

        for (int i = makerCameraZoomNotePoolHead; i < makerCameraZoomNotePool.Count; ++i)
            makerCameraZoomNotePool[i].gameObject.SetActive(false);
        makerCameraZoomNotePoolHead = 0;

    }

    public float TimeToLineLength(float _time)
    {
        return _time * g.bpmRatio * g.speed * -MakeManager.oneBeatToLine;
    }

    public float TimeToMakerLineLength(float _time)
    {
        return _time * g.bpmRatio * MakeManager.oneBeatToLine;
    }

    public void AddNoteData(NoteData _data)
    {
        datas.Add(_data);
    }

    public void RemoveNoteData(NoteData _data)
    {
        datas.Remove(_data);
    }

    public void SaveNoteData()
    {
        s.SaveNoteData(datas);
    }
}
