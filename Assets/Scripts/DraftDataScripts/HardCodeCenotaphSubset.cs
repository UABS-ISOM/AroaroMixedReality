using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class HardCodeCenotaphSubset : MonoBehaviour
{
    public GameObject myPrefab;
    public string fileName;

    List<DataItemCenotaph> CenotaphDataItems;
    private void Awake()
    {
        CenotaphDataItems = HardCodeCSVReader.Read(fileName);
    }


    // Start is called before the first frame update
    void Start()
    {
        foreach (DataItemCenotaph objectToPlot in CenotaphDataItems)
        {
            var holdNewObj = (GameObject)Instantiate(myPrefab, new Vector3(objectToPlot.X, objectToPlot.Y, objectToPlot.Z), Quaternion.identity);
        }
    }

   
    // Update is called once per frame
    void Update()
    {
        
    }

    public class HardCodeCSVReader
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        public static List<DataItemCenotaph> Read(string file)
        {
            List<DataItemCenotaph> list = new List<DataItemCenotaph>(); //can just use var as type
            TextAsset data = Resources.Load(file) as TextAsset;

            var lines = Regex.Split(data.text, LINE_SPLIT_RE);

            var firstLine = Regex.Split(lines[0], SPLIT_RE);


            //for all lines in csv...
            for (int i = 0; i < lines.Length; i++)
            {
                DataItemCenotaph dataItem = new DataItemCenotaph();
                var rowValues = Regex.Split(lines[i], SPLIT_RE);

                //check if there is a blank row, instead of breaking continue to next iteration
                if (rowValues.Length == 0 || rowValues[0] == "") continue;

                //in the current line, look at each row
                float fx;
                if (float.TryParse(rowValues[0], out fx))
                {
                    dataItem.X = fx;
                }

                float fy;
                if (float.TryParse(rowValues[1], out fy))
                {
                    dataItem.Y = fy;
                }

                float fz;
                if (float.TryParse(rowValues[2], out fz))
                {
                    dataItem.Z = fz;
                }

                float fWhen;
                if (float.TryParse(rowValues[3], out fWhen))
                {
                    dataItem.When = fWhen;

             
                }

                dataItem.Colour = rowValues[4];

                float fSize;
                if (float.TryParse(rowValues[5], out fSize))
                {
                    dataItem.Size = fSize;


                }

                dataItem.Shape = rowValues[6];

                dataItem.Name = rowValues[7];

                //add to list of items
                list.Add(dataItem);
            }

            return list;
            
        }

    }
}
