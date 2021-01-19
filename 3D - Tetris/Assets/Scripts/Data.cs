using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Data 
{
    public static List<ShapeModel> LoadShapes()
    {
        List<ShapeModel> parsedData = new List<ShapeModel>();

        // Get CSV raw data
        TextAsset csvData = Resources.Load<TextAsset>("ShapesData");

        // Split data to raws
        string[] rawData = csvData.text.Split(new char[] { '\n' });

        // Loop thorugh all data except the first row 
        //(which is used for ease of use and not for data) and the last  row (which is empty)
        for (int i = 1; i < rawData.Length - 1; i++)
        {
            // Split the row to section and ignore comas inside double quotes
            string[] row =
                Regex.Split(rawData[i], ",(?=([^\"]*\"[^\"]*\")*[^\"]*$)");

            #region Block positions
            // Remove Double quotes
            string blockPosColumn = row[1].Substring(1, row[1].Length - 2);

            // Split block positions data
            string[] blocksPosData = blockPosColumn.Split(new char[] { '|' });

            // Create array to store all positions data to pass it later
            Vector3[] blockPositionTarget = new Vector3[blocksPosData.Length];

            for (int o = 0; o < blocksPosData.Length; o++)
            {
                // Split axis values
                string[] blockData = blocksPosData[o].Split(new char[] { ',' });

                int.TryParse(blockData[0], out int x);
                int.TryParse(blockData[1], out int y);
                int.TryParse(blockData[2], out int z);

                blockPositionTarget[o] = new Vector3(x, y, z);
            }

            #endregion

            // Create data container
            ShapeModel shapeData = new ShapeModel
            {
                name = row[0],
                blocksPositions = blockPositionTarget
            };

            string appearanceOddData = row[3].Substring(0, row[3].Length - 1);

            int.TryParse(appearanceOddData, out int appearanceOdd);

            // Add items to loop according to the appearance odd
            for (int x = 0; x < appearanceOdd; x++)
                parsedData.Add(shapeData);
        }

        return parsedData;
    }
}
