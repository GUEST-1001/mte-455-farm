using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveUI : MonoBehaviour
{
    [SerializeField]
    private string saveName;
    public string SaveName
    {
        get { return saveName; }
        set
        {
            saveName =
    value;
        }
    }

    [SerializeField]
    private int currentIndex = 0;

    [SerializeField]
    private Button[] buttons; //map with slot btns

    [SerializeField]
    private TMP_Text[] slotText;

    [SerializeField]
    private TMP_Text reportText;

    [SerializeField]
    private string[] saveFiles; //name of files in save folder

    public void SelectButton(int i) //Slot Button for Load or Save
    {
        currentIndex = i;
        saveName = "savedgame" + i.ToString();
    }

    public void OnSave() //Save Game Button
    {
        SaveManager.instance.Save();
        bool success = SerializationManager.Save(saveName, SaveManager.instance); //Serialize SaveManager into a file

        if (success)
        {
            reportText.text = $"save {saveName} success!";
            slotText[currentIndex].text = saveName;
        }
        else
            reportText.text = $"save {saveName} error!";
    }

    private bool CheckSaveFiles(string saveName)
    {
        for (int i = 0; i < saveFiles.Length; i++)
        {
            string filename = saveFiles[i].Replace(Application.dataPath + "/saves/", "");

            if (filename == saveName + ".sav")
                return true;
        }

        return false;
    }

    private void LoadFile(string saveName)
    {
        SaveManager.instance.ClearData();
        SaveManager.instance =
            (SaveManager)SerializationManager.Load(Application.dataPath + "/saves/" + saveName + ".sav");

        SceneManager.LoadScene(SaveManager.instance.sceneName);
    }

    public void OnLoad() //Load Game Button
    {
        Debug.Log("OnLoad");
        Debug.Log(saveName);

        if (saveName == "")
            return;

        GetLoadFiles();

        if (CheckSaveFiles(saveName))
        {
            Debug.Log("Check File Success");

            Settings.loadingGame = true;
            LoadFile(saveName);
        }
    }

    public void GetLoadFiles()
    {
        if (!Directory.Exists(Application.dataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/saves/");
        }

        saveFiles = Directory.GetFiles(Application.dataPath + "/saves/");
    }

    public void ShowSavedGameList()//Map with Main Menu Load Save Game Btn
    {
        GetLoadFiles();

        for (int i = 0; i < buttons.Length; i++)
        {
            for (int j = 0; j < saveFiles.Length; j++)
            {
                string filename = saveFiles[j].Replace(Application.dataPath + "/saves/", "");

                if (filename == "savedgame" + i.ToString() + ".sav")
                {
                    buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = filename;

                    break;
                }
            }
        }
    }
}
