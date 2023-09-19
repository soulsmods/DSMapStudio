using ImGuiNET;
using SoulsFormats.KF4;
using StudioCore.HelpMenu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Help
{
    public class HelpMenu
    {
        private string _id;
        private AssetLocator _locator;

        public bool MenuOpenState = false;

        private string textSectionSplitter = "[-----]";
        private int textSectionForceSplitCharCount = 2760;

        // Core
        private string _selectedTitle_Core = null;
        private string _selectedTags_Core = null;
        private List<string> _selectedContents_Core = null;
        private string _inputStr_Core = "";
        private string _inputStrCache_Core = "";

        // Tutorial
        private string _selectedTitle_Tutorial = null;
        private string _selectedTags_Tutorial = null;
        private List<string> _selectedContents_Tutorial = null;
        private string _inputStr_Tutorial = "";
        private string _inputStrCache_Tutorial = "";

        public HelpMenu(string id, AssetLocator locator)
        {
            _id = id;
            _locator = locator;
        }

        public void Display()
        {
            float scale = MapStudioNew.GetUIScale();

            if (!MenuOpenState)
                return;

            ImGui.SetNextWindowSize(new Vector2(900.0f, 800.0f) * scale, ImGuiCond.FirstUseEver);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0f, 0f, 0f, 0.98f));
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, new Vector4(0.25f, 0.25f, 0.25f, 1.0f));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10.0f, 10.0f) * scale);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(20.0f, 10.0f) * scale);
            ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, 20.0f * scale);

            if (ImGui.Begin("Help Menu##Popup", ref MenuOpenState, ImGuiWindowFlags.NoDocking))
            {
                ImGui.BeginTabBar("#HelpMenuTabBar");
                ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0.3f, 0.3f, 0.6f, 0.4f));
                ImGui.PushItemWidth(300f);

                DisplayCore();
                DisplayTutorials();
                DisplayCredits();

                ImGui.PopItemWidth();
                ImGui.PopStyleColor();
                ImGui.EndTabBar();
            }
            ImGui.End();

            ImGui.PopStyleVar(3);
            ImGui.PopStyleColor(2);
        }

        private void DisplayCore()
        {
            if (ImGui.BeginTabItem("Help"))
            {
                // Search Area
                ImGui.InputText($"Search", ref _inputStr_Core, 255);

                // Selection Area
                ImGui.BeginChild("HelpSectionList", new Vector2(300, 100));

                if (_inputStr_Core.ToLower() != _inputStrCache_Core.ToLower())
                {
                    _inputStrCache_Core = _inputStr_Core.ToLower();
                }

                string inputStr = _inputStr_Core.ToLower();

                foreach (ResourceEntry entry in HelpMenuResource.Static.Entries)
                {
                    string sectionName = entry.Title;
                    List<string> contents = entry.Contents;
                    List<string> tags = entry.Tags;

                    // Section Title Segments
                    List<string> sectionNameSegments = new List<string>();

                    foreach (string segment in sectionName.Split(" ").ToList())
                    {
                        List<string> segmentParts = segment.Split(" ").ToList();
                        foreach (string part in segmentParts)
                        {
                            sectionNameSegments.Add(part.ToLower());
                        }
                    }

                    // Content Segments
                    List<string> coreSegments = new List<string>();

                    foreach(string segment in contents)
                    {
                        List<string> segmentParts = segment.Split(" ").ToList();
                        foreach(string part in segmentParts)
                        {
                            coreSegments.Add(part.ToLower());
                        }
                    }

                    // Tags
                    List<string> tagList = new List<string>();

                    foreach (string segment in tags)
                    {
                        tagList.Add(segment.ToLower());
                    }

                    // Only show if input matches any title or content segments, or if it is blank
                    if (inputStr.ToLower() == "" || sectionNameSegments.Contains(inputStr) || coreSegments.Contains(inputStr) || tagList.Contains(inputStr) )
                    {
                        if (ImGui.Selectable(sectionName))
                        {

                        }
                        if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                        {
                            _selectedTitle_Core = sectionName;
                            _selectedContents_Core = contents;
                        }
                    }
                }

                ImGui.EndChild();

                // Section Title
                ImGui.Separator();

                string titleText = "No title.";

                if (_selectedTitle_Core != null)
                    titleText = _selectedTitle_Core;

                ImGui.Text(titleText);

                ImGui.Separator();

                // Section Contents
                List<string> coreTextSections = new List<string> { "No section selected." };

                if (_selectedContents_Core != null)
                    coreTextSections = GetDisplayTextSections(_selectedContents_Core);

                foreach (string textSection in coreTextSections)
                {
                    //ImGui.Separator();
                    ImGui.Text(textSection);
                }

                ImGui.Separator();
                ImGui.EndTabItem();
            }
        }

        private void DisplayTutorials()
        {
            if (ImGui.BeginTabItem("Tutorials"))
            {
                // Search Area
                ImGui.InputText($"Search", ref _inputStr_Tutorial, 255);

                // Selection Area
                ImGui.BeginChild("HelpSectionList", new Vector2(300, 100));

                if (_inputStr_Tutorial.ToLower() != _inputStrCache_Tutorial.ToLower())
                {
                    _inputStrCache_Tutorial = _inputStr_Tutorial.ToLower();
                }

                string inputStr = _inputStr_Tutorial.ToLower();

                foreach (ResourceEntry entry in HelpTutorialResource.Static.Entries)
                {
                    string sectionName = entry.Title;
                    List<string> contents = entry.Contents;
                    List<string> tags = entry.Tags;

                    // Section Title Segments
                    List<string> sectionNameSegments = new List<string>();

                    foreach (string segment in sectionName.Split(" ").ToList())
                    {
                        List<string> segmentParts = segment.Split(" ").ToList();
                        foreach (string part in segmentParts)
                        {
                            sectionNameSegments.Add(part.ToLower());
                        }
                    }

                    // Content Segments
                    List<string> coreSegments = new List<string>();

                    foreach (string segment in contents)
                    {
                        List<string> segmentParts = segment.Split(" ").ToList();
                        foreach (string part in segmentParts)
                        {
                            coreSegments.Add(part.ToLower());
                        }
                    }

                    // Tags
                    List<string> tagList = new List<string>();

                    foreach (string segment in tags)
                    {
                        tagList.Add(segment.ToLower());
                    }

                    // Only show if input matches any title or content segments, or if it is blank
                    if (inputStr.ToLower() == "" || sectionNameSegments.Contains(inputStr) || coreSegments.Contains(inputStr) || tagList.Contains(inputStr))
                    {
                        if (ImGui.Selectable(sectionName))
                        {

                        }
                        if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(0))
                        {
                            _selectedTitle_Tutorial = sectionName;
                            _selectedContents_Tutorial = contents;
                        }
                    }
                }

                ImGui.EndChild();

                // Section Title
                ImGui.Separator();

                string titleText = "No title.";

                if (_selectedTitle_Tutorial != null)
                    titleText = _selectedTitle_Tutorial;

                ImGui.Text(titleText);

                ImGui.Separator();

                // Section Contents
                List<string> coreTextSections = new List<string> { "No tutorial selected." };

                if (_selectedContents_Tutorial != null)
                    coreTextSections = GetDisplayTextSections(_selectedContents_Tutorial);

                foreach (string textSection in coreTextSections)
                {
                    //ImGui.Separator();
                    ImGui.Text(textSection);
                }

                ImGui.Separator();
                ImGui.EndTabItem();
            }
        }

        private void DisplayCredits()
        {
            if (ImGui.BeginTabItem("Credits"))
            {
                ImGui.Indent();

                ImGui.Text(GetDisplayText(CreditMenuResource.Static.Text));

                ImGui.Unindent();
                ImGui.EndTabItem();
            }
        }

        private string GetDisplayText(List<string> stringList)
        {
            int charCount = 0;

            string displayText = "";
            foreach (string str in stringList)
            {
                displayText = displayText + str + "\n";
                charCount = charCount + str.Length;

                // Force seperator if text length is close to the Imgui.Text character limit.
                if (charCount >= textSectionForceSplitCharCount)
                {
                    charCount = 0;
                    displayText = displayText + textSectionSplitter + "";
                }
                else
                {
                    // If the current str already includes a separator, reset the count
                    // As GetDisplayTextSections will handle the separator instead
                    if (str.Contains(textSectionSplitter))
                    {
                        charCount = 0;
                    }
                }
            }

            return displayText;
        }

        private List<string> GetDisplayTextSections(List<string> stringList)
        {
            string displayTextFull = GetDisplayText(stringList);

            List<string> displayTextSegments = displayTextFull.Split(textSectionSplitter).ToList();

            return displayTextSegments;
        }
    }
}
