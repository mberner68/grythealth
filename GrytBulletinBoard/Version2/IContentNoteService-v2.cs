using System;
using Umbraco.Core.Models;
using CoreLibrary.Models;
using GrytCore.Controllers;

namespace CoreLibrary.Services
{
    public interface IContentNoteService
    {
        bool CreateNewNoteItem(string PropertyAlias, NoteModel Model);
        Guid CreateNewKey();
        IContent GetParent(string Key);
        bool EditNoteItem (string PropertyAlias, NoteModel Model);
        bool CopyNoteItem (string PropertyAlias, NoteModel Model);
        bool MoveNoteItem (string PropertyAlias, NoteModel Model);
        string GetUser();
        string DeleteNoteItem(DeleteRequest data, string PropertyAlias);
        NoteModel GetNoteItem(string PropertyAlias, NoteModel Model);
    }

}
