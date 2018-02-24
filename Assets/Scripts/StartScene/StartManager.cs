using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
	public static StartManager instance;

	[HideInInspector]
	public AudioClip music;
	[HideInInspector]
	public string noteData;
	[HideInInspector]
	public bool musicLoaded = false;
	[HideInInspector]
	public bool noteDataLoaded = false;
	[HideInInspector]
	public string musicFilePath = "";
	[HideInInspector]
	public string noteDataFilePath = "";

	public Text musicPathText;
	public Text noteDataPathText;
	public Button goMakeSceneBtn;



	private void Awake()
	{
		if (instance != null)
		{
			this.music = instance.music;
			this.noteData = instance.noteData;
			this.musicLoaded = instance.musicLoaded;
			this.noteDataLoaded = instance.noteDataLoaded;
			this.musicFilePath = instance.musicFilePath;
			this.noteDataFilePath = instance.noteDataFilePath;

			StartCoroutine(LoadMusic(musicFilePath));
			StartCoroutine(LoadNoteData(noteDataFilePath));

			Destroy(instance.gameObject);
            return;
		}

        musicFilePath = PlayerPrefs.GetString("SongPath", "");
        noteDataFilePath = PlayerPrefs.GetString("NotePath", "");

        instance = this;
		DontDestroyOnLoad(this);

        if (musicFilePath != "") {
            StartCoroutine(LoadMusic(musicFilePath));
        }

        if (noteDataFilePath != "")
        {
            StartCoroutine(LoadNoteData(noteDataFilePath));
        }

    }

	private void Start()
	{

	}

	private void Update()
	{
		musicPathText.text = musicFilePath;
		noteDataPathText.text = noteDataFilePath;

		goMakeSceneBtn.interactable = musicLoaded;
	}

	public void OnSongSelect()
	{
        string path = "";
		System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
		fileDialog.Filter = "Ogg Files(*.ogg)|*.ogg|All(*.*)|*.*";
		if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			path = fileDialog.FileName;


        PlayerPrefs.SetString("SongPath", path);
        StartCoroutine(LoadMusic(path));
	}

	public void OnNoteDataSelect()
	{
        string path = "";
        System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
		fileDialog.Filter = "Text Datas (*.txt)|*.txt|Note Datas (*.ntd)|*.ntd|All files (*.*)|*.*";
		if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			path = fileDialog.FileName;
		else
			return;

    
        PlayerPrefs.SetString("NotePath", path);
        StartCoroutine(LoadNoteData(path));
	}

	private IEnumerator LoadMusic(string _path)
	{
		WWW www = new WWW("file://" + _path);
		yield return www;

		AudioClip load = www.GetAudioClip();
		if (load != null)
		{
			music = load;
			musicFilePath = _path;
			musicLoaded = true;
		}
		else
		{
			music = null;
			musicFilePath = "";
			musicLoaded = false;
		}
	}

	private IEnumerator LoadNoteData(string _path)
	{
		WWW www = new WWW("file://" + _path);
		yield return www;

		string load = www.text;
		if (load != null)
		{
			noteData = www.text;
			noteDataFilePath = _path;
			noteDataLoaded = true;
		}
		else
		{
			noteData = null;
			noteDataFilePath = "";
			noteDataLoaded = false;
		}
	}

	public void OnGoMakeScene()
	{
		SceneManager.LoadScene("MakeScene");
	}
}
