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
        string GetUser();
        string DeleteNoteItem(DeleteRequest data, string PropertyAlias);
        NoteModel MarketingCheckbox(NoteModel Model);
        string GetPageStringProperty(string PropertyName, string defaultVal, Umbraco.Core.Models.PublishedContent.IPublishedContent page);
        NoteModel GetNoteItem(string PropertyAlias, NoteModel Model);
    }

}
