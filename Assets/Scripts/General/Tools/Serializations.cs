using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serializations
{
    public static string SerializeListOfInt(List<int> list)
    {
        string serializedList = "{";

        for (int i = 0; i < list.Count; i++)
        {
            serializedList += list[i].ToString();

            if (i < list.Count - 1)
                serializedList += ",";
        }

        serializedList += "}";

        return serializedList;
    }

    public static List<int> UnserializeListOfInt(string serializedList)
    {
        List<int> list = new List<int>();

        int charaCount = 0;
        string recordingString = "";

        while (charaCount < serializedList.Length)
        {
            if (serializedList[charaCount].ToString() != "{" && serializedList[charaCount].ToString() != "}" && serializedList[charaCount].ToString() != ",")
            {
                recordingString += serializedList[charaCount];
            }
            else if (serializedList[charaCount].ToString() == "}" || serializedList[charaCount].ToString() == ",")
            {
                if(recordingString != null && recordingString != "")
                    list.Add(int.Parse(recordingString));
                recordingString = "";
            }

            charaCount++;
        }

        return list;
    }


    public static string SerializeListOfString(List<string> list)
    {
        string serializedList = "{";

        for (int i = 0; i < list.Count; i++)
        {
            serializedList += list[i];

            if (i < list.Count - 1)
                serializedList += ",";
        }

        serializedList += "}";

        return serializedList;
    }

    public static List<string> UnserializeListOfString(string serializedList)
    {
        List<string> list = new List<string>();

        int charaCount = 0;
        string recordingString = "";

        while (charaCount < serializedList.Length)
        {
            if (serializedList[charaCount].ToString() != "{" && serializedList[charaCount].ToString() != "}" && serializedList[charaCount].ToString() != ",")
            {
                recordingString += serializedList[charaCount];
            }
            else if (serializedList[charaCount].ToString() == "}" || serializedList[charaCount].ToString() == ",")
            {
                if (recordingString != "")
                {
                    list.Add(recordingString);
                    recordingString = "";
                }
            }

            charaCount++;
        }

        return list;
    }
}

