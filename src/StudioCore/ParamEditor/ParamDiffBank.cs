using Andre.Formats;
using Microsoft.Extensions.Logging;
using SoulsFormats;
using StudioCore.Editor;
using StudioCore.Platform;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StudioCore.ParamEditor;

/// <summary>
///     Utilities for dealing with global paramdiffs for a game
/// </summary>
public class ParamDiffBank : DataBank
{

    private Dictionary<string, HashSet<int>> _primaryDiffCache; //If param != primaryparam
    private Dictionary<string, HashSet<int>> _vanillaDiffCache; //If param != vanillaparam

    public ParamDiffBank(Project project) : base(project, "ParamDiffs")
    {
    }

    public IReadOnlyDictionary<string, HashSet<int>> VanillaDiffCache
    {
        get
        {
            if (IsLoading)
            {
                return null;
            }

            if (Project.ParentProject == null)
            {
                return null;
            }
            return _vanillaDiffCache;
        }
    }

    public IReadOnlyDictionary<string, HashSet<int>> PrimaryDiffCache
    {
        get
        {
            if (IsLoading)
            {
                return null;
            }

            if (Project == Locator.ActiveProject)
            {
                return null;
            }
            return _primaryDiffCache;
        }
    }

    private void ClearParamDiffCaches()
    {
        _vanillaDiffCache = new Dictionary<string, HashSet<int>>();
        _primaryDiffCache = new Dictionary<string, HashSet<int>>();
        foreach (var param in Project.ParamBank.Params?.Keys)
        {
            _vanillaDiffCache.Add(param, new HashSet<int>());
            _primaryDiffCache.Add(param, new HashSet<int>());
        }
    }

    public static void RefreshAllParamDiffCaches(bool checkAuxVanillaDiff)
    {
        Locator.ActiveProject.ParamDiffBank.RefreshParamDiffCaches(true);
        foreach (KeyValuePair<string, Project> aux in ResDirectory.CurrentGame.AuxProjects)
        {
            aux.Value.ParamDiffBank.RefreshParamDiffCaches(checkAuxVanillaDiff);
        }

        UICache.ClearCaches();
    }

    public void RefreshParamDiffCaches(bool checkVanillaDiff)
    {
        if (Project.ParentProject != null && checkVanillaDiff)
        {
            _vanillaDiffCache = GetParamDiff(Project.ParentProject.ParamBank);
        }

        if (Project.ParentProject == null && Locator.ActiveProject.ParamDiffBank._vanillaDiffCache != null)
        {
            _primaryDiffCache = Locator.ActiveProject.ParamDiffBank._vanillaDiffCache;
        }
        else if (Project != Locator.ActiveProject)
        {
            _primaryDiffCache = GetParamDiff(Locator.ActiveProject.ParamBank);
        }

        UICache.ClearCaches();
    }

    private Dictionary<string, HashSet<int>> GetParamDiff(ParamBank otherBank)
    {
        if (otherBank == null || otherBank.IsLoading)
        {
            return null;
        }

        Dictionary<string, HashSet<int>> newCache = new();
        foreach (var param in Project.ParamBank.Params.Keys)
        {
            HashSet<int> cache = new();
            newCache.Add(param, cache);
            Param p = Project.ParamBank.Params[param];
            if (!otherBank.Params.ContainsKey(param))
            {
                Console.WriteLine("Missing vanilla param " + param);
                continue;
            }

            Param.Row[] rows = Project.ParamBank.Params[param].Rows.OrderBy(r => r.ID).ToArray();
            Param.Row[] vrows = otherBank.Params[param].Rows.OrderBy(r => r.ID).ToArray();

            var vanillaIndex = 0;
            var lastID = -1;
            ReadOnlySpan<Param.Row> lastVanillaRows = default;
            for (var i = 0; i < rows.Length; i++)
            {
                var ID = rows[i].ID;
                if (ID == lastID)
                {
                    RefreshParamRowDiffCache(rows[i], lastVanillaRows, cache);
                }
                else
                {
                    lastID = ID;
                    while (vanillaIndex < vrows.Length && vrows[vanillaIndex].ID < ID)
                    {
                        vanillaIndex++;
                    }

                    if (vanillaIndex >= vrows.Length)
                    {
                        RefreshParamRowDiffCache(rows[i], Span<Param.Row>.Empty, cache);
                    }
                    else
                    {
                        var count = 0;
                        while (vanillaIndex + count < vrows.Length && vrows[vanillaIndex + count].ID == ID)
                        {
                            count++;
                        }

                        lastVanillaRows = new ReadOnlySpan<Param.Row>(vrows, vanillaIndex, count);
                        RefreshParamRowDiffCache(rows[i], lastVanillaRows, cache);
                        vanillaIndex += count;
                    }
                }
            }
        }

        return newCache;
    }

    private static void RefreshParamRowDiffCache(Param.Row row, ReadOnlySpan<Param.Row> otherBankRows,
        HashSet<int> cache)
    {
        if (IsChanged(row, otherBankRows))
        {
            cache.Add(row.ID);
        }
        else
        {
            cache.Remove(row.ID);
        }
    }

    public void RefreshParamRowDiffs(Param.Row row, string param)
    {
        if (param == null)
        {
            return;
        }

        if (Project.ParentProject.ParamBank.Params.ContainsKey(param) && VanillaDiffCache != null &&
            VanillaDiffCache.ContainsKey(param))
        {
            Param.Row[] otherBankRows = Project.ParentProject.ParamBank.Params[param].Rows.Where(cell => cell.ID == row.ID).ToArray();
            RefreshParamRowDiffCache(row, otherBankRows, VanillaDiffCache[param]);
        }

        if (Project != Locator.ActiveProject)
        {
            return;
        }

        foreach (Project aux in ResDirectory.CurrentGame.AuxProjects.Values)
        {
            if (!aux.ParamBank.Params.ContainsKey(param) || aux.ParamDiffBank.PrimaryDiffCache == null ||
                !aux.ParamDiffBank.PrimaryDiffCache.ContainsKey(param))
            {
                continue; // Don't try for now
            }

            Param.Row[] otherBankRows = aux.ParamBank.Params[param].Rows.Where(cell => cell.ID == row.ID).ToArray();
            RefreshParamRowDiffCache(row, otherBankRows, aux.ParamDiffBank.PrimaryDiffCache[param]);
        }
    }

    private static bool IsChanged(Param.Row row, ReadOnlySpan<Param.Row> vanillaRows)
    {
        //List<Param.Row> vanils = vanilla.Rows.Where(cell => cell.ID == row.ID).ToList();
        if (vanillaRows.Length == 0)
        {
            return true;
        }

        foreach (Param.Row vrow in vanillaRows)
        {
            if (row.RowMatches(vrow))
            {
                return false; //if we find a matching vanilla row
            }
        }

        return true;
    }
    private static readonly HashSet<int> EMPTYSET = new();

    public HashSet<int> GetVanillaDiffRows(string param)
    {
        IReadOnlyDictionary<string, HashSet<int>> allDiffs = VanillaDiffCache;
        if (allDiffs == null || !allDiffs.ContainsKey(param))
        {
            return EMPTYSET;
        }

        return allDiffs[param];
    }

    public HashSet<int> GetPrimaryDiffRows(string param)
    {
        IReadOnlyDictionary<string, HashSet<int>> allDiffs = PrimaryDiffCache;
        if (allDiffs == null || !allDiffs.ContainsKey(param))
        {
            return EMPTYSET;
        }

        return allDiffs[param];
    }

    public override void Save()
    {
    }

    protected override void Load()
    {
        RefreshParamDiffCaches(true);
        UICache.ClearCaches();
    }

    protected override IEnumerable<StudioResource> GetDependencies(Project project)
    {    
        return [project.ParamBank, project.ParentProject.ParamBank];
    }
}
