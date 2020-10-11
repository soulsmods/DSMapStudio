using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Linq;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;
using ImGuiNET;
using SoulsFormats;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// Interface for decorating param rows with additional information (such as english
    /// strings sourced from FMG files)
    /// </summary>
    public interface IParamDecorator
    {
        public void DecorateParam(PARAM.Row row);

        public void DecorateContextMenu(PARAM.Row row);
    }

    public class FMGItemParamDecorator : IParamDecorator
    {
        private FMGBank.ItemCategory _category = FMGBank.ItemCategory.None;

        private Dictionary<int, FMG.Entry> _entryCache = new Dictionary<int, FMG.Entry>();

        public FMGItemParamDecorator(FMGBank.ItemCategory cat)
        {
            _category = cat;
        }

        public void DecorateParam(PARAM.Row row)
        {
            if (!_entryCache.ContainsKey((int)row.ID))
            {
                _entryCache.Add((int)row.ID, FMGBank.LookupItemID((int)row.ID, _category));
            }
            var entry = _entryCache[(int)row.ID];
            if (entry != null)
            {
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), $@" <{entry.Text}>");
            }
        }

        public void DecorateContextMenu(PARAM.Row row)
        {
            if (!_entryCache.ContainsKey((int)row.ID))
            {
                return;
            }
            if (ImGui.BeginPopupContextItem(row.ID.ToString()))
            {
                if (ImGui.Selectable($@"Goto {_category.ToString()} Text"))
                {
                    EditorCommandQueue.AddCommand($@"text/select/{_category.ToString()}/{row.ID}");
                }
                ImGui.EndPopup();
            }
        }
    }

    public class ParamEditorScreen : EditorScreen
    {
        public ActionManager EditorActionManager = new ActionManager();

        private string _activeParam = null;
        private PARAM.Row _activeRow = null;
        private string currentMEditinput = "PARAM: (id VALUE | name ROW | prop FIELD VALUE | propref FIELD ROW): FIELD: ((=|+|-|*|/) VALUE | ref ROW);";
        private string lastMEditinput = "";
        //eg "EquipParamWeapon: "
        private static string paramfilterRx = $@"^(?<paramrx>[^\s:]+):\s+";
        //eg "id (100|200)00"
        private static string rowfilteridRx = $@"(id\s+(?<rowidexp>[^:]+))";
        //eg "name \s+ Arrow"
        private static string rowfilternameRx = $@"(name\s+(?<rownamerx>[^:]+))";
        //eg "prop sellValue 100"
        private static string rowfilterpropRx = $@"(prop\s+(?<rowpropfield>[^\s]+)\s+(?<rowpropvalexp>[^:]+))";
        //eg "propref originalEquipWep0 Dagger\[.+\]"
        private static string rowfilterproprefRx = $@"(propref\s+(?<rowpropreffield>[^\s]+)\s+(?<rowproprefnamerx>[^:]+))";
        private static string rowfilterRx = $@"({rowfilteridRx}|{rowfilternameRx}|{rowfilterpropRx}|{rowfilterproprefRx}):\s+";
                        //eg "correctFaith: "
        private static string fieldRx = $@"(?<fieldrx>[^\:]+):\s+";
                        //eg "* 2;
        private static string operationRx = $@"(?<op>=|\+|-|\*|/|ref)\s+(?<opparam>[^;]+);";
        private static Regex commandRx = new Regex($@"{paramfilterRx}{rowfilterRx}{fieldRx}{operationRx}");

        private PropertyEditor _propEditor = null;

        private Dictionary<string, IParamDecorator> _decorators = new Dictionary<string, IParamDecorator>();

        private ProjectSettings _projectSettings = null;
        public ParamEditorScreen(Sdl2Window window, GraphicsDevice device)
        {
            _propEditor = new PropertyEditor(EditorActionManager);

            _decorators.Add("EquipParamAccessory", new FMGItemParamDecorator(FMGBank.ItemCategory.Rings));
            _decorators.Add("EquipParamGoods", new FMGItemParamDecorator(FMGBank.ItemCategory.Goods));
            _decorators.Add("EquipParamProtector", new FMGItemParamDecorator(FMGBank.ItemCategory.Armor));
            _decorators.Add("EquipParamWeapon", new FMGItemParamDecorator(FMGBank.ItemCategory.Weapons));
        }

        public override void DrawEditorMenu()
        {
            bool openMEdit = false;
            //Menu Options
            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Undo", "CTRL+Z", false, EditorActionManager.CanUndo()))
                {
                    EditorActionManager.UndoAction();
                }
                if (ImGui.MenuItem("Redo", "Ctrl+Y", false, EditorActionManager.CanRedo()))
                {
                    EditorActionManager.RedoAction();
                }
                if (ImGui.MenuItem("Delete", "Delete", false, _activeParam != null && _activeRow != null))
                {
                    if (_activeParam != null && _activeRow != null)
                    {
                        var act = new DeleteParamsAction(ParamBank.Params[_activeParam], new List<PARAM.Row>() { _activeRow });
                        EditorActionManager.ExecuteAction(act);
                        _activeRow = null;
                    }
                }
                if (ImGui.MenuItem("Duplicate", "Ctrl+D", false, _activeParam != null && _activeRow != null))
                {
                    if (_activeParam != null && _activeRow != null)
                    {
                        var act = new CloneParamsAction(ParamBank.Params[_activeParam], _activeParam, new List<PARAM.Row>() { _activeRow }, true);
                        EditorActionManager.ExecuteAction(act);
                    }
                }
                if(ImGui.MenuItem("Mass Edit", null, false, true)){
                    openMEdit = true;
                }
                ImGui.EndMenu();
            }
            //Menu Popups
            if(openMEdit){
                ImGui.OpenPopup("massEditMenu");
            }
            MassEditPopup();
        }

        public void MassEditPopup(){
            if(ImGui.BeginPopup("massEditMenu")){
                ImGui.InputTextMultiline("MEditInput", ref currentMEditinput, 65536, new Vector2(1024, 256));
                if(ImGui.Selectable("Submit")){

                    string[] commands = currentMEditinput.Split('\n');
                    MultiplePropertiesChangedAction action = new MultiplePropertiesChangedAction();
                    foreach(string command in commands){
                        Match comm = commandRx.Match(command);
                        if(comm.Success){

                            Regex paramRx = new Regex(comm.Groups["paramrx"].Value);
                            Regex fieldRx = new Regex(comm.Groups["fieldrx"].Value);
                            string op = comm.Groups["op"].Value;
                            string opparam = comm.Groups["opparam"].Value;
                            Group rowidexp = comm.Groups["rowidexp"];
                            Group rownamerx = comm.Groups["rownamerx"];
                            Group rowpropfield = comm.Groups["rowpropfield"];
                            Group rowpropreffield = comm.Groups["rowpropreffield"];

                            List<PARAM> affectedParams = getMatchingParams(paramRx);
                            List<PARAM.Row> affectedRows = new List<PARAM.Row>();
                            foreach(PARAM param in affectedParams){//not ideal to loop here as we rebuild regexes/recheck which method we use, but it's clean
                                if(rowidexp.Success){
                                    affectedRows = getMatchingParamRows(param, rowidexp.Value);
                                }else if(rownamerx.Success){
                                    affectedRows = getMatchingParamRows(param, new Regex(rownamerx.Value));
                                }else if(rowpropfield.Success){
                                    affectedRows = getMatchingParamRows(param, rowpropfield.Value, comm.Groups["rowpropvalexp"].Value);
                                }else if(rowpropreffield.Success){
                                    affectedRows = getMatchingParamRows(param, rowpropreffield.Value, new Regex(comm.Groups["rowproprefnamerx"].Value));
                                }
                                //somehow matched but also failed to identify a row address
                            }
                            List<PARAM.Cell> affectedCells = getMatchingCells(affectedRows, fieldRx);
                            Console.WriteLine($@"{affectedCells.Count} cells changed");
                            foreach(PARAM.Cell cell in affectedCells){
                                object newval = performOperation(cell, op, opparam);
                                if(newval == null){
                                    //failed to perform op
                                    Console.WriteLine("An operation failed");
                                    return;
                                }
                                action.AddPropertyChange(cell, cell.GetType().GetProperty("Value"), newval);
                            }
                        }else{
                            //invalid command. tell user at some point maybe.
                            Console.WriteLine("Bad command");
                        }
                    }
                    EditorActionManager.ExecuteAction(action);
                    lastMEditinput = currentMEditinput;
                    currentMEditinput = "";
                }
                ImGui.Text(lastMEditinput);//more of an output thing
                ImGui.EndPopup();
            }
        }
        public List<PARAM> getMatchingParams(Regex paramrx){
            List<PARAM> plist = new List<PARAM>();
            foreach(string name in ParamBank.Params.Keys){
                if(paramrx.Match(name).Success){
                    plist.Add(ParamBank.Params[name]);
                }
            }
            return plist;
        }
        public List<PARAM.Row> getMatchingParamRows(PARAM param, string rowvalexp){
            List<PARAM.Row> rlist = new List<PARAM.Row>();
            foreach(PARAM.Row row in param.Rows){
                if(matchNumExp(row.ID, rowvalexp)){
                    rlist.Add(row);
                }
            }
            return rlist;
        }
        public List<PARAM.Row> getMatchingParamRows(PARAM param, Regex rownamerx){
            List<PARAM.Row> rlist = new List<PARAM.Row>();
            foreach(PARAM.Row row in param.Rows){
                if(rownamerx.Match(row.Name==null?"":row.Name).Success){
                    rlist.Add(row);
                }
            }
            return rlist;
        }
        public List<PARAM.Row> getMatchingParamRows(PARAM param, string rowfield, string rowvalexp){
            List<PARAM.Row> rlist = new List<PARAM.Row>();
            foreach(PARAM.Row row in param.Rows){
                PARAM.Cell c = row[rowfield];
                if(c!=null){
                    if(matchNumExp(c.Value, rowvalexp)){
                        rlist.Add(row);
                    }
                }
            }
            return rlist;
        }
        public List<PARAM.Row> getMatchingParamRows(PARAM param, string rowfield, Regex rownamerx){
            List<PARAM.Row> rlist = new List<PARAM.Row>();
            foreach(PARAM.Row row in param.Rows){
                PARAM.Cell c = row[rowfield];
                if(c!=null){
                    int val = (int)c.Value;//assume int value
                    foreach(string rt in c.Def.RefTypes){
                        if(!ParamBank.Params.ContainsKey(rt)){
                            continue;
                        }
                        PARAM.Row r = ParamBank.Params[rt][val];
                        if(r!=null && rownamerx.Match(r.Name==null?"":r.Name).Success){
                            rlist.Add(row);//don't add the ref'd row lol
                            break;//no need to check other refs
                        }
                    }
                }
            }
            return rlist;
        }
        public List<PARAM.Cell> getMatchingCells(List<PARAM.Row> rows, Regex fieldrx){
            List<PARAM.Cell> clist = new List<PARAM.Cell>();
            foreach(PARAM.Row row in rows){//we love seeing loops on top of loops so brazenly
                foreach(PARAM.Cell c in row.Cells){
                    if(fieldrx.Match(c.Def.DisplayName).Success){//using displayname instead of internal. questionable?
                        clist.Add(c);
                        //intentionally not breaking - potential for changing multiple cells at once eg. originalweapon0-10
                    }
                }
            }
            return clist;
        }
        public bool matchNumExp(object val, string valexp){
            try{
                Regex rx = new Regex(valexp);//just use regex even though it's not great for numbers
                return rx.Match(val.ToString()).Success;
                //return val.Equals(long.Parse(valexp));//basic right now and assumes long type, to be extended with x<y/100<z syntax nonsense
            }catch(FormatException f){
                //format error
                return false;
            }
        }
        public object performOperation(PARAM.Cell cell, string op, string opparam){
            try{
                if(op.Equals("ref")){
                    if(cell.Value.GetType()==typeof(int)){
                        foreach(string reftype in cell.Def.RefTypes){
                            PARAM p = ParamBank.Params[reftype];
                            if(p==null){
                                continue;
                            }
                            foreach(PARAM.Row r in p.Rows){
                                if(r.Name==null){
                                    continue;
                                }
                                if(r.Name.Equals(opparam)){
                                    return (int)r.ID;
                                }
                            }
                        }
                    }
                }
                if(op.Equals("=")){
                    if(cell.Value.GetType()==typeof(bool)){
                        return bool.Parse(opparam);
                    }else if(cell.Value.GetType()==typeof(string)){
                        return opparam;
                    }
                }
                if(cell.Value.GetType()==typeof(long)){
                    return performBasicOperation<long>(cell, op, double.Parse(opparam));
                }else if(cell.Value.GetType()==typeof(int)){
                    return performBasicOperation<int>(cell, op, double.Parse(opparam));
                }else if(cell.Value.GetType()==typeof(short)){
                    return performBasicOperation<short>(cell, op, double.Parse(opparam));
                }else if(cell.Value.GetType()==typeof(ushort)){
                    return performBasicOperation<ushort>(cell, op, double.Parse(opparam));
                }else if(cell.Value.GetType()==typeof(sbyte)){
                    return performBasicOperation<sbyte>(cell, op, double.Parse(opparam));
                }else if(cell.Value.GetType()==typeof(byte)){
                    return performBasicOperation<byte>(cell, op, double.Parse(opparam));
                }else if(cell.Value.GetType()==typeof(float)){
                    return performBasicOperation<float>(cell, op, double.Parse(opparam));
                }
            }catch(FormatException f){
                Console.WriteLine("Poorly formatted operation");
            }
            return null;
        }
        public T performBasicOperation<T>(PARAM.Cell c, string op, double opparam) where T : struct, IFormattable{
            try{
                dynamic val = c.Value;
                dynamic opp = opparam;
                //this is a hellish mess
                //I feel like the cs grad meme
                if(op.Equals("=")){
                    return (T)(opp);
                }else if(op.Equals("+")){
                    return (T)(val+opp);
                }else if(op.Equals("-")){
                    return (T)(val-opp);
                }else if(op.Equals("*")){
                    return (T)(val*opp);
                }else if(op.Equals("/")){
                    return (T)(val/opp);
                }
            }catch(Exception e){
                //Operation error
            }
            return default(T);
        }

        public void OnGUI(string[] initcmd)
        {
            // Keyboard shortcuts
            if (EditorActionManager.CanUndo() && InputTracker.GetControlShortcut(Key.Z))
            {
                EditorActionManager.UndoAction();
            }
            if (EditorActionManager.CanRedo() && InputTracker.GetControlShortcut(Key.Y))
            {
                EditorActionManager.RedoAction();
            }
            if (InputTracker.GetControlShortcut(Key.D))
            {
                if (_activeParam != null && _activeRow != null)
                {
                    var act = new CloneParamsAction(ParamBank.Params[_activeParam], _activeParam, new List<PARAM.Row>() { _activeRow }, true);
                    EditorActionManager.ExecuteAction(act);
                }
            }
            if (InputTracker.GetKeyDown(Key.Delete))
            {
                if (_activeParam != null && _activeRow != null)
                {
                    var act = new DeleteParamsAction(ParamBank.Params[_activeParam], new List<PARAM.Row>() { _activeRow });
                    EditorActionManager.ExecuteAction(act);
                    _activeRow = null;
                }
            }

            if (ParamBank.Params == null)
            {
                return;
            }

            bool doFocus = false;
            // Parse select commands
            if (initcmd != null && initcmd[0] == "select")
            {
                if (initcmd.Length > 1 && ParamBank.Params.ContainsKey(initcmd[1]))
                {
                    doFocus = true;
                    _activeParam = initcmd[1];
                    if (initcmd.Length > 2)
                    {
                        var p = ParamBank.Params[_activeParam];
                        int id;
                        var parsed = int.TryParse(initcmd[2], out id);
                        if (parsed)
                        {
                            var r = p.Rows.FirstOrDefault(r => r.ID == id);
                            if (r != null)
                            {
                                _activeRow = r;
                            }
                        }
                    }
                }
            }

            ImGui.Columns(3);
            ImGui.BeginChild("params");
            foreach (var param in ParamBank.Params)
            {
                if (ImGui.Selectable(param.Key, param.Key == _activeParam))
                {
                    _activeParam = param.Key;
                    _activeRow = null;
                }
                if (doFocus && param.Key == _activeParam)
                {
                    ImGui.SetScrollHereY();
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();
            ImGui.BeginChild("rows");
            if (_activeParam == null)
            {
                ImGui.Text("Select a param to see rows");
            }
            else
            {
                IParamDecorator decorator = null;
                if (_decorators.ContainsKey(_activeParam))
                {
                    decorator = _decorators[_activeParam];
                }
                var p = ParamBank.Params[_activeParam];
                foreach (var r in p.Rows)
                {
                    if (ImGui.Selectable($@"{r.ID} {r.Name}", _activeRow == r))
                    {
                        _activeRow = r;
                    }
                    if (decorator != null)
                    {
                        decorator.DecorateContextMenu(r);
                        decorator.DecorateParam(r);
                    }
                    if (doFocus && _activeRow == r)
                    {
                        ImGui.SetScrollHereY();
                    }
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();
            ImGui.BeginChild("columns");
            if (_activeRow == null)
            {
                ImGui.Text("Select a row to see properties");
            }
            else
            {
                _propEditor.PropEditorParamRow(_activeRow);
            }
            ImGui.EndChild();
        }

        public override void OnProjectChanged(ProjectSettings newSettings)
        {
            _projectSettings = newSettings;
            _activeParam = null;
            _activeRow = null;
        }

        public override void Save()
        {
            if (_projectSettings != null)
            {
                ParamBank.SaveParams(_projectSettings.UseLooseParams);
            }
        }

        public override void SaveAll()
        {
            if (_projectSettings != null)
            {
                ParamBank.SaveParams(_projectSettings.UseLooseParams);
            }
        }
    }
}
