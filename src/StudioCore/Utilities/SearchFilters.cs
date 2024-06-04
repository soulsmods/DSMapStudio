﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Utilities;

public static class SearchFilters
{
    /// <summary>
    /// Returns true is the input string (whole or part) matches a filename, reference name or tag.
    /// </summary>
    public static bool IsSearchMatch(string rawInput, string rawRefId, string rawRefName, List<string> rawRefTags,
        bool matchAssetCategory = false, // Match AEG categories passed in input
        bool stripParticlePrefix = false, // Remove f and preceding zeroes from checked string against input
        bool splitWithDelimiter = false, // Split entry by passed delimiter and check against input
        string delimiter = "_" // Delimiter to split entry by
        )
    {
        bool match = false;

        string input = rawInput.Trim().ToLower();
        string refId = rawRefId.ToLower();
        string refName = rawRefName.ToLower();

        if (rawRefTags == null)
        {
            rawRefTags = new List<string>() { "" };
        }

        if (input.Equals(""))
        {
            match = true; // If input is empty, show all
            return match;
        }

        string[] inputParts = input.Split("+");
        bool[] partTruth = new bool[inputParts.Length];

        for (int i = 0; i < partTruth.Length; i++)
        {
            string entry = inputParts[i];

            // Match: Filename/ID
            if (entry == refId)
                partTruth[i] = true;

            if (stripParticlePrefix)
            {
                // Get the number from the f000000000 string
                if (refId.Contains("f"))
                {
                    refId = refId.Replace("f", "");
                    try
                    {
                        refId = int.Parse(refId).ToString();
                    }
                    catch
                    {
                        refId = refId.ToLower();
                    }
                }
            }

            // Match: Reference Name
            if (entry == refName)
                partTruth[i] = true;

            if (splitWithDelimiter)
            {
                var refParts = refName.Split(delimiter);
                foreach (var refPart in refParts)
                {
                    if (entry == refPart)
                    {
                        partTruth[i] = true;
                    }
                }
            }

            // Match: Reference Segments
            string[] refSegments = refName.Split(" ");
            foreach (string refStr in refSegments)
            {
                string curString = refStr;

                // Remove common brackets so the match ignores them
                if (curString.Contains('('))
                    curString = curString.Replace("(", "");

                if (curString.Contains(')'))
                    curString = curString.Replace(")", "");

                if (curString.Contains('{'))
                    curString = curString.Replace("{", "");

                if (curString.Contains('}'))
                    curString = curString.Replace("}", "");

                if (curString.Contains('('))
                    curString = curString.Replace("(", "");

                if (curString.Contains('['))
                    curString = curString.Replace("[", "");

                if (curString.Contains(']'))
                    curString = curString.Replace("]", "");

                if (entry == curString.Trim())
                    partTruth[i] = true;
            }

            // Match: Tags
            foreach (string tagStr in rawRefTags)
            {
                if (entry == tagStr.ToLower())
                    partTruth[i] = true;
            }

            // Match: AEG Category
            if (matchAssetCategory)
            {
                if (!entry.Equals("") && entry.All(char.IsDigit))
                {
                    if (refId.Contains("aeg") && refId.Contains("_"))
                    {
                        string[] parts = refId.Split("_");
                        string aegCategory = parts[0].Replace("aeg", "");

                        if (entry == aegCategory)
                        {
                            partTruth[i] = true;
                        }
                    }
                }
            }
        }

        match = true;

        foreach (bool entry in partTruth)
        {
            if (!entry)
                match = false;
        }

        return match;
    }

    public static bool IsEditorSearchMatch(string rawInput, string checkInput, string delimiter)
    {
        bool match = false;

        string cleanRawInput = rawInput.Trim().ToLower();
        string cleanCheckInput = checkInput.Trim().ToLower();

        if (cleanRawInput.Equals(""))
        {
            match = true; // If input is empty, show all
            return match;
        }

        string[] inputParts = cleanRawInput.Split("+");
        bool[] partTruth = new bool[inputParts.Length];

        for (int i = 0; i < partTruth.Length; i++)
        {
            string entry = inputParts[i];

            if (entry == cleanCheckInput)
                partTruth[i] = true;

            var refParts = cleanCheckInput.Split($"{delimiter}");
            foreach (var refPart in refParts)
            {
                if (entry == refPart)
                {
                    partTruth[i] = true;
                }
            }
        }

        match = true;

        foreach (bool entry in partTruth)
        {
            if (!entry)
                match = false;
        }

        return match;
    }

    public static bool IsIdSearchMatch(string rawInput, string checkInput)
    {
        bool match = false;

        int rawInputNum = -1;
        int.TryParse(rawInput, out rawInputNum);

        int checkInputNum = -1;
        int.TryParse(checkInput, out checkInputNum);

        string[] inputParts = rawInput.Split("+");
        bool[] partTruth = new bool[inputParts.Length];

        if (rawInput.Equals(""))
        {
            match = true; // If input is empty, show all
            return match;
        }

        for (int i = 0; i < partTruth.Length; i++)
        {
            int entry;
            int.TryParse(inputParts[i], out entry);

            if (entry == checkInputNum)
                partTruth[i] = true;
        }

        // Act as OR
        if (partTruth.Contains(true))
        {
            match = true;
        }

        return match;
    }

    public static bool IsAssetBrowserSearchMatch(string rawInput, string rawRefId, string rawRefName, List<string> rawRefTags)
    {
        bool match = false;

        string input = rawInput.Trim().ToLower();
        string refId = rawRefId.ToLower();
        string refName = rawRefName.ToLower();

        if (rawRefTags == null)
        {
            rawRefTags = new List<string>() { "" };
        }

        if (input.Equals(""))
        {
            match = true; // If input is empty, show all
            return match;
        }

        string[] inputParts = input.Split("+");
        bool[] partTruth = new bool[inputParts.Length];

        for (int i = 0; i < partTruth.Length; i++)
        {
            string entry = inputParts[i];

            // Match: ID
            if (entry == refId)
                partTruth[i] = true;

            var refIdParts = refId.Split("_");
            foreach (var refIdPart in refIdParts)
            {
                if (entry == refIdPart)
                {
                    partTruth[i] = true;
                }
            }

            // Match: Name
            if (entry == refName)
                partTruth[i] = true;

            var refParts = refName.Split("_");
            foreach (var refPart in refParts)
            {
                if (entry == refPart)
                {
                    partTruth[i] = true;
                }
            }

            // Match: Reference Segments
            string[] refSegments = refName.Split(" ");
            foreach (string refStr in refSegments)
            {
                string curString = refStr;

                // Remove common brackets so the match ignores them
                if (curString.Contains('('))
                    curString = curString.Replace("(", "");

                if (curString.Contains(')'))
                    curString = curString.Replace(")", "");

                if (curString.Contains('{'))
                    curString = curString.Replace("{", "");

                if (curString.Contains('}'))
                    curString = curString.Replace("}", "");

                if (curString.Contains('('))
                    curString = curString.Replace("(", "");

                if (curString.Contains('['))
                    curString = curString.Replace("[", "");

                if (curString.Contains(']'))
                    curString = curString.Replace("]", "");

                if (entry == curString.Trim())
                    partTruth[i] = true;
            }

            // Match: Tags
            foreach (string tagStr in rawRefTags)
            {
                if (entry == tagStr.ToLower())
                    partTruth[i] = true;
            }
        }

        match = true;

        foreach (bool entry in partTruth)
        {
            if (!entry)
                match = false;
        }

        return match;
    }
}
