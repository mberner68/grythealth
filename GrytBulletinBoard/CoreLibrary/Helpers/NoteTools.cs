using System;
using System.Collections.Generic;
using System.Linq;
using CoreLibrary.Models;
using Newtonsoft.Json;
using Umbraco.Core.Models;

namespace CoreLibrary.Helpers
{
    public class NoteTools
    {
        public NoteTools()
        {
        }
        public class SetUpModel
        {
            
            public static NoteModel GetOriginal (List<Dictionary<string,string>> noteValz, string NodeKey)
            {
                NoteModel CopyNote2 = new NoteModel();
                CopyNote2.NodeKey = Guid.Parse(NodeKey);

                // Only 3 Values that matter - Title, content and classname
                for (int idx = 0; idx < noteValz.Count(); idx++)
                {

                    if (Guid.Parse(noteValz[idx]["key"].ToString()) ==  CopyNote2.NodeKey)
                    {

                        CopyNote2.Title = noteValz[idx]["title"];
                        CopyNote2.Note = noteValz[idx]["content"];
                        CopyNote2.ClassName = noteValz[idx]["className"];
                    }
                }

                return CopyNote2;
            }
            public static NoteModel GetData(NoteModel model, string NodeKey, string BoardKey, string name)
            {
                
                // Parameters needed to copy the note to a new board //  
                model.NodeKey = Guid.Parse(NodeKey);
                model.Board = name;
                model.BoardKey = BoardKey;
                return model; 
            }
            public static NoteModel EditData(string NodeKey, string BoardKey, string name, string PageId)
            {
                NoteModel EditNote = new NoteModel();
                // Note's key
                EditNote = GetData(EditNote, NodeKey, BoardKey, name);
                // Return Page Id
                EditNote.PageId = Int32.Parse(PageId);
                return EditNote;
            }
            public static List<Dictionary<string, string>> GetAllNew(IContent NewParent)
            {
                var Notebooks = new List<Dictionary<string, string>>();
                var notesJSON2 = NewParent.GetValue<string>("notes");
                if (notesJSON2 != null)
                {
                    var noteValz2 = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(notesJSON2);

                    for (int idx = 0; idx < noteValz2.Count(); idx++)
                    {
                        Notebooks.Add(noteValz2[idx]);
                    }
                }
                return Notebooks;
           }           
        }    
    }
}
