using UnityEngine;

[System.Serializable]
public class NoteData
{
	private const float batterMinHitInterval = 0.125f;

	// Common
	public float time = 0f;
	public int lineNum = 0;

	// Long
	public float longEndTime = 0f;

	// Drag
	public int drag = 0;

	// Batter
	public float batterEndTime = 0f;
	public int batterHit = 0;
	
	public int noteType
	{
		get
		{
            if (batterHit > 0)
				return Note.N_BATTER;
			if (drag != 0)
				return Note.N_DRAG;
			if (longEndTime > 0f)
				return Note.N_LONG;
			return Note.N_NORMAL;
		}
	}

	public void Set(NoteData _data)
	{
		this.time = _data.time;
		this.lineNum = _data.lineNum;
		this.longEndTime = _data.longEndTime;
		this.drag = _data.drag;
		this.batterEndTime = _data.batterEndTime;
		this.batterHit = _data.batterHit;
    }

	public string GetNoteDataString()
	{
		switch (noteType)
		{
			case Note.N_NORMAL:
				return "NormalNote:" + lineNum + ":" + Mathf.Floor(time * 1000f);
			case Note.N_LONG:
				return "LongNote:" + lineNum + ":" + Mathf.Floor(time * 1000f) + ":" + Mathf.Floor(longEndTime * 1000f);
			case Note.N_DRAG:
				return "DragNote:" + lineNum + ":" + Mathf.Floor(time * 1000f) + ":" + drag;
			case Note.N_BATTER:
				return "BatterNote:" + batterHit + ":" + Mathf.Floor(time * 1000f) + ":" + Mathf.Floor(batterEndTime * 1000f);
		}
		return "";
	}

	public int GetMaxBatterHit()
	{
		return Mathf.Clamp((int)((batterEndTime - time - BatterNote.appearTime) / batterMinHitInterval), 2, 9999);
    }

	public NoteData GetClone()
	{
		return (NoteData)this.MemberwiseClone();
	}
}