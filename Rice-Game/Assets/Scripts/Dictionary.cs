using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Xml;
using System;

public static class Dictionary
{
	public static bool hasBeenInit = false;

    static Dictionary<string, string> words;
	public enum TextType { NORMAL, FirstUpper, Upper };
	private static List<SystemLanguage> m_languages = new List<SystemLanguage>{ SystemLanguage.English, SystemLanguage.Catalan, SystemLanguage.Spanish };

    public static void init()
    {
        words = new Dictionary<string, string>();
        TextAsset file = Resources.Load("xml/" + getLanguageFile(), typeof(TextAsset)) as TextAsset;
        if (file != null) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(file.text);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes) {
                string key = node.Attributes["name"].Value;
                string text = node.InnerText;
                try {
                    words.Add(key, text);
                }
                catch(Exception ex) {
                    Debug.LogError("---Exception dictionary---");
                    Debug.LogError("key::" + key);
                    Debug.LogError(ex.Message);
                }
            }
			hasBeenInit = true;
        }
        else {
            Debug.Log("File xml dictionary not loaded");
        }
        
    }

    //lowr case
    public static string getString(string key)
    {
		if (!hasBeenInit) init ();
		
        try {
            if (words.ContainsKey(key)) {
                return words[key];
            }
            else {
                return " NOT_FOUND_DICTIONARY " + key;
            }
        }
        catch(Exception ex) {
            Debug.LogError("getString exception with key::" + key);
            Debug.LogError(ex.Message);
            return " NOT_FOUND_DICTIONARY ";
        }
        
    }

    //upper case
    public static string getStringUC(string key)
	{
		if (!hasBeenInit) init ();
		
        if (words.ContainsKey(key)) {
            return words[key].ToUpper();
        }
        else {
            return key + " NOT_FOUND_DICTIONARY";
        }

    }

    //First upper case string
    public static string getStringFUC(string key)
    {
		if (!hasBeenInit) init ();

        if (words.ContainsKey(key)) {
            string str = words[key];
            return char.ToUpper(str[0]) + str.Substring(1);
        }
        else {
            return key + " NOT_FOUND_DICTIONARY";
        }

    }

    public static string getLanguageFile()
    {
        string file = "cat";
		SystemLanguage currentLanguage;
		if (PlayerPrefs.HasKey ("CurrentLanguage")) {
			currentLanguage = m_languages [PlayerPrefs.GetInt ("CurrentLanguage")];
		} else {
			currentLanguage = Application.systemLanguage;
		}

		switch(currentLanguage) {
            case SystemLanguage.English:
                file = "en";
                break;
            case SystemLanguage.Catalan:
                file = "cat";
                break;
            case SystemLanguage.Spanish:
                file = "es";
                break;
        }

        return file;
        //return "cat";
    }
}
