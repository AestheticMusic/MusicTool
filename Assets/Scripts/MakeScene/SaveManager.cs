using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Windows.Forms;

public class SaveManager : MonoBehaviour
{
	public static SaveManager instance;

	private MakeManager g;


	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		g = MakeManager.instance;
	}

	public void SaveNoteData(List<NoteData> _datas)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Title = "Save NoteData";
		saveFileDialog.Filter = "Note Datas (*.ntd)|*.ntd|All files (*.*)|*.*";
		saveFileDialog.DefaultExt = "ntd";
		saveFileDialog.AddExtension = true;

		if (saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
			StreamWriter streamWriter = new StreamWriter(fileStream);

			streamWriter.WriteLine("NoteDataVersion:" + MakeManager.makerVersion);
			streamWriter.WriteLine("StartBPM:" + g.bpm);
			streamWriter.WriteLine("NoteSync:" + Mathf.Floor(g.noteSync * 1000f));

			foreach (NoteData data in _datas)
			{
				streamWriter.WriteLine(data.GetNoteDataString());
			}
			
			streamWriter.Close();
		}
	}
}
