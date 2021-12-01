using System;
using System.Collections.Generic;
using System.Web.Security;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Composing;
using CoreLibrary.Models;
using Newtonsoft.Json;
using System.Linq;
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
            Model = MarketingCheckbox(Model);

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
                        {"content", Model.Note},
                        {"className", Model.ClassName },
                        {"question1", Model.question1 },
                        {"Marketing", Model.Marketing },
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
            Model = MarketingCheckbox(Model);
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
                    noteValz[idx]["Marketing"] = Model.Marketing;
                }
                Notebooks.Add(noteValz[idx]);
            }

            Parent.SetValue(PropertyAlias, JsonConvert.SerializeObject(Notebooks));
            _contentService.SaveAndPublish(Parent);
            return true;
            
        }
        
        
        public string GetUser()
        {
            string C = "";
            if (Membership.GetUser() != null)
            {
                var currentMember = Current.Services.MemberService.GetByUsername(Membership.GetUser().UserName);
                if (currentMember != null)
                {
                    C = "umb://member/" + currentMember.Key.ToString().Replace("-", "");
                }
            }
            return C;
        }
        public string DeleteNoteItem(DeleteRequest data, string PropertyAlias)
        {
           string NoteId = data.NodeKey;
           

            var Parent = GetParent(data.BoardId);

            var notesJSON = Parent.GetValue<string>(PropertyAlias);
            var noteValz = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(notesJSON);

            for (int idx = 0; idx < noteValz.Count(); idx++)
            {
                if (noteValz[idx]["key"].ToString() == data.NodeKey)
                {
                    noteValz.Remove(noteValz[idx]);
                    break;
                }
            }
            

            Parent.SetValue(PropertyAlias, JsonConvert.SerializeObject(noteValz));
            _contentService.SaveAndPublish(Parent);

            return NoteId;

        }
        public NoteModel MarketingCheckbox (NoteModel Model)
        {
            if (Model.Marketing1 == true)
            {
                Model.Marketing = "Yes";
                return Model;
            }
            else
            {
                Model.Marketing = "No";
                return Model;
            }


        }
        public string GetPageStringProperty(string PropertyName, string defaultVal, Umbraco.Core.Models.PublishedContent.IPublishedContent page)
        {


            if (page == null) { return "xxx"; }
            string result = (string)page.GetProperty(PropertyName).GetValue();
            if (result == null || result == "") { return defaultVal; }
            return result;
        }
        public NoteModel GetNoteItem(string PropertyAlias, NoteModel Model)
        {
            IContent Parent = GetParent(Model.BoardKey);
            var notesJSON = Parent.GetValue<string>(PropertyAlias);
            var noteValz = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(notesJSON);
            // Only 3 Values that matter - Title, content and classname
            for (int idx = 0; idx < noteValz.Count(); idx++)
            {

                if (Guid.Parse(noteValz[idx]["key"].ToString()) == Model.NodeKey)
                {

                    //Model.Title = noteValz[idx]["title"];
                    Model.Note = noteValz[idx]["content"];
                    Model.ClassName = noteValz[idx]["className"];
                    Model.Marketing = noteValz[idx]["Marketing"];
                    Model.Creator = noteValz[idx]["creator"];

                }
            }
            return Model;

        }
    }
}
