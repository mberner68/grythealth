using System;
using System.Collections.Generic;
using System.Web.Security;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Composing;
using CoreLibrary.Models;
using Newtonsoft.Json;
using System.Linq;
using CoreLibrary.Helpers;
using GrytCore.Controllers;

namespace CoreLibrary.Services
{
    public class ContentNoteService : IContentNoteService
    {
        private readonly IContentService _contentService;
        private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;

        public ContentNoteService(IContentService contentService,
            IContentTypeBaseServiceProvider contentTypeBaseServiceProvider)
        {
            _contentService = contentService;
            _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
        }
        /// <param name="Key">Search for parent</param>
        /// <param name="PropertyAlias">The notes alias</param>
        /// <param name="model">the notes information</param>
        /// <returns>The udi for the new media item</returns>
        public bool CreateNewNoteItem(string PropertyAlias, NoteModel Model)
        {
            var Notebooks = new List<Dictionary<string, string>>();
            var NewKey = CreateNewKey();
            string Key = Model.BoardKey;
            IContent Parent = GetParent(Key);

            var notesJSON = Parent.GetValue<string>(PropertyAlias);
            if (notesJSON != null)
            {
                var noteValz = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(notesJSON);

                for (int idx = 0; idx < noteValz.Count(); idx++)
                {
                    Notebooks.Add(noteValz[idx]);
                }
            }
            Notebooks.Add(new Dictionary<string, string>() {
                        {"ncContentTypeAlias","boardNote"},
                        {"title", Model.Title},
                        {"content", Model.Note},
                        {"className", Model.ClassName },
                        {"question1", Model.question1 },
                        {"creator", Model.Creator },
                        {"key", NewKey.ToString()}
                });

            Parent.SetValue(PropertyAlias, JsonConvert.SerializeObject(Notebooks));
            _contentService.SaveAndPublish(Parent);

            return true;
        }
        public Guid CreateNewKey ()
        {
            Guid NewKey = Guid.NewGuid();
            return NewKey;
        }
        public IContent GetParent (string Key)
        {
            IContent Parent = _contentService.GetById(Guid.Parse(Key));
            return Parent;
        }
        public bool EditNoteItem(string PropertyAlias, NoteModel Model)
        {
            string Key = Model.BoardKey;
            var Notebooks = new List<Dictionary<string, string>>();
            Guid NewKey = CreateNewKey();
            IContent Parent = GetParent(Key);
            var notesJSON = Parent.GetValue<string>(PropertyAlias);
            var noteValz = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(notesJSON);

            for (int idx = 0; idx < noteValz.Count(); idx++)
            {

                if (Guid.Parse(noteValz[idx]["key"].ToString()) == Model.NodeKey)
                {
                    //noteValz[idx]["title"] = Model.Title;
                    noteValz[idx]["content"] = Model.Note;
                    noteValz[idx]["className"] = Model.ClassName;
                }
                Notebooks.Add(noteValz[idx]);
            }

            Parent.SetValue(PropertyAlias, JsonConvert.SerializeObject(Notebooks));
            _contentService.SaveAndPublish(Parent);
            return true;
            
        }
        public bool CopyNoteItem(string PropertyAlias, NoteModel Model)
        {
            var Notebooks = new List<Dictionary<string, string>>();
            string Key = Model.BoardKey;
            string NewBKey = Model.NewBoardKey;
            IContent NewParent = GetParent(NewBKey);
            Notebooks = NoteTools.SetUpModel.GetAllNew(NewParent);

            /* Grab Note to be moved to new board and add it to the list*/
            Guid NewKey = CreateNewKey();
            IContent Parent = GetParent(Key);
            var notesJSON = Parent.GetValue<string>(PropertyAlias);
            var noteValz = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(notesJSON);

            for (int idx = 0; idx < noteValz.Count(); idx++)
            {
                if (Guid.Parse(noteValz[idx]["key"].ToString()) == Model.NodeKey)
                {
                    Notebooks.Add(new Dictionary<string, string>() {
                        {"ncContentTypeAlias","boardNote"},
                        {"title", noteValz[idx]["title"]},
                        {"content", noteValz[idx]["content"]},
                        {"className", noteValz[idx]["className"] },
                        {"creator", noteValz[idx]["creator"] },
                        {"key", NewKey.ToString()}
                        });
                }
            }
            NewParent.SetValue(PropertyAlias, JsonConvert.SerializeObject(Notebooks));
            _contentService.SaveAndPublish(NewParent);
            return true;
        }
        public bool MoveNoteItem(string PropertyAlias, NoteModel Model)
        {
            var Notebooks = new List<Dictionary<string, string>>();
            string Key = Model.BoardKey;
            string NewBKey = Model.NewBoardKey;
            //  Delete and Save Note to Move

            IContent Parent = GetParent(Key);
            Guid NoteKey = Model.NodeKey;
            var notesJSON = Parent.GetValue<string>(PropertyAlias);
            var noteValz = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(notesJSON);

            for (int idx = 0; idx < noteValz.Count(); idx++)
            {
                if (Guid.Parse(noteValz[idx]["key"].ToString()) == Model.NodeKey)
                {
                    Notebooks.Add(new Dictionary<string, string>() {
                        {"ncContentTypeAlias","boardNote"},
                        {"title", noteValz[idx]["title"]},
                        {"content", noteValz[idx]["content"]},
                        {"className", noteValz[idx]["className"] },
                        {"creator", noteValz[idx]["creator"] },
                        {"key", Model.NodeKey.ToString()}
                        });
                    noteValz.Remove(noteValz[idx]);
                }

            }
            Parent.SetValue(PropertyAlias, JsonConvert.SerializeObject(noteValz));
            _contentService.SaveAndPublish(Parent);

            // Move Note to new Board
           
            IContent NewParent = GetParent(NewBKey);
            /* grab from new board */
            int Parentid = NewParent.ParentId;

            var notesJSON2 = NewParent.GetValue<string>(PropertyAlias);
            if (notesJSON2 != null)
            {
                var noteValz2 = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(notesJSON2);

                for (int idx = 0; idx < noteValz2.Count(); idx++)
                {
                    Notebooks.Add(noteValz2[idx]);
                }
            }
            NewParent.SetValue(PropertyAlias, JsonConvert.SerializeObject(Notebooks));
            _contentService.SaveAndPublish(NewParent);

            return true;
