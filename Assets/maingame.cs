using UnityEngine;
using System.IO;
[System.Serializable]
public class Save
{
    public Vector3 player_position;
    public int weapon;
    public Vector3 gun_position;
    public Vector3 squid_position;
    public bool squid_active;
}
public static class maingame
{
    static public Save data;
    static maingame(){
        data = new Save();
        data.player_position = new Vector3(0, 0, -10);
        data.squid_active = false;
        data.weapon = 0;
        data.gun_position = new Vector3(0, 0, -10);
        data.squid_position = new Vector3(-5.054f, -1.149f, -4.061f);
    }
    static string path = Path.Combine(Application.persistentDataPath, "savefile.json");
    public static void LoadGame()
    {
        if (File.Exists(path))
        {
            Debug.Log("it's there");
            string tex = File.ReadAllText(path);
            data = JsonUtility.FromJson<Save>(tex);
        }
        Debug.Log(path);
    }
    public static void SaveGame()
    {
        string tex = JsonUtility.ToJson(data);
        File.WriteAllText(path, tex);
    }
}
