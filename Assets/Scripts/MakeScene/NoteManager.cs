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

	private float noteMinTime = 0f;
	private float noteMaxTime = 0f;
	public List<NoteData> datas;
	private List<NoteData> targetDatas;
	private List<Note>[] notePools;
	private List<MakerNote> makerNotePool;
	private List<MakerNoteLong> makerNoteLongPool;
	private int[] notePoolHeads;
	private int makerNotePoolHead = 0;
	private int makerNoteLongPoolHead = 0;
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

		noteMinTime = Mathf.Clamp(g.syncedTime, 0f, m.musicLength);
		noteMaxTime = Mathf.Clamp(g.syncedTime + MakeManager.maxBeatInLine / g.bpmRatio * 2f, 0f, m.musicLength);

		foreach (NoteData data in datas)
		{
			if (data.time >= noteMinTime &&
				data.time <= noteMaxTime && data.noteType != Note.N_BATTER)
				targetDatas.Add(data);
			else if (data.noteType == Note.N_LONG && 
				data.longEndTime >= noteMinTime && data.time <= noteMaxTime)
				targetDatas.Add(data);
			else if (data.noteType == Note.N_BATTER && 
				data.batterEndTime >= noteMinTime - BatterNote.disappearTime && data.time <= noteMaxTime)
				targetDatas.Add(data);
		}
		
		Vector3 notePos = new Vector3();
		foreach (NoteData data in targetDatas)
		{
			float timeDiff = data.time - g.syncedTime;
			float length = TimeToLineLength(timeDiff);

			Note note = PopNote(data.noteType);
            notePos.x = length;
			notePos.z = data.time * 0.1f;
			if (data.noteType == Note.N_BATTER)
			{
				note.notePos = Vector3.zero;
				note.transform.parent = g.lines[0].parent;
			}
			else
			{
				note.notePos = notePos;
				note.transform.parent = g.lines[data.lineNum];
			}
			note.data = data;
			note.Start();

			MakerNote mn = null;
			if (data.noteType == Note.N_LONG)
			{
				mn = PopMakerNoteLong();
				((MakerNoteLong)mn).SetLength(data.longEndTime - data.time);
			}
			else if (data.noteType == Note.N_BATTER)
			{
				mn = PopMakerNoteLong();
				((MakerNoteLong)mn).SetLength(data.batterEndTime - data.time);

				MakerNoteLong mn2 = PopMakerNoteLong();
				mn2.SetLength(data.batterEndTime - data.time);
				mn2.SetColor(Note.N_BATTER);
				mn2.notePos.y = TimeToMakerLineLength(timeDiff);
				mn2.transform.parent = g.makerLines[1 - data.lineNum].transform;
				mn2.data = data;
                mn2.Start();
			}
			else
				mn = PopMakerNote();
			mn.notePos.y = TimeToMakerLineLength(timeDiff);
			mn.transform.parent = g.makerLines[data.lineNum].transform;
			mn.data = data;
			mn.SetColor(data.noteType);
			mn.Start();
		}
		DisableRestNotes();
    }

	private void InitializePool()
	{
		notePools = new List<Note>[maxType];
		notePoolHeads = new int[maxType];
		for (int i = 0; i < maxType; ++i)
		{
			notePoolHeads[i] = 0;
			notePools[i] = new List<Note>();
			for (int j = 0; j < 5; ++j)
			{
				Note note = Instantiate<GameObject>(notePrafabs[i]).GetComponent<Note>();
				note.gameObject.SetActive(false);
				notePools[i].Add(note);
			}
		}

		makerNotePool = new List<MakerNote>();
		for (int i = 0; i < 5; ++i)
		{
			MakerNote note = Instantiate<GameObject>(makerNotePrafab).GetComponent<MakerNote>();
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
	}

	private Note PopNote(int _type)
	{
		Note note = null;
		if (notePools[_type].Count == notePoolHeads[_type])
		{
			note = Instantiate<GameObject>(notePrafabs[_type]).GetComponent<Note>();
			notePools[_type].Add(note);
		}
		else
			note = notePools[_type][notePoolHeads[_type]];

		++notePoolHeads[_type];
		note.gameObject.SetActive(true);
		return note;
	}

	private MakerNote PopMakerNote()
	{
		MakerNote note = null;
		if (makerNotePool.Count == makerNotePoolHead)
		{
			note = Instantiate<GameObject>(makerNotePrafab).GetComponent<MakerNote>();
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
