using Andre.Formats;
using StudioCore.ParamEditor;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StudioCore.Editor;

public class DuplicateFMGEntryAction : EditorAction
{
    private readonly FMGEntryGroup EntryGroup;
    private FMGEntryGroup NewEntryGroup;

    public DuplicateFMGEntryAction(FMGEntryGroup entryGroup)
    {
        EntryGroup = entryGroup;
    }

    public override ActionEvent Execute()
    {
        NewEntryGroup = EntryGroup.DuplicateFMGEntries();
        NewEntryGroup.SetNextUnusedID();
        return ActionEvent.NoEvent;
    }

    public override ActionEvent Undo()
    {
        NewEntryGroup.DeleteEntries();
        return ActionEvent.NoEvent;
    }
}

public class DeleteFMGEntryAction : EditorAction
{
    private FMGEntryGroup BackupEntryGroup = new();
    private FMGEntryGroup EntryGroup;

    public DeleteFMGEntryAction(FMGEntryGroup entryGroup)
    {
        EntryGroup = entryGroup;
    }

    public override ActionEvent Execute()
    {
        BackupEntryGroup = EntryGroup.CloneEntryGroup();
        EntryGroup.DeleteEntries();
        return ActionEvent.NoEvent;
    }

    public override ActionEvent Undo()
    {
        EntryGroup = BackupEntryGroup;
        EntryGroup.ImplementEntryGroup();
        return ActionEvent.NoEvent;
    }
}
