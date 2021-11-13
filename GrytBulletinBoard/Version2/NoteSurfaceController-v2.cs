using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using CoreLibrary.Models;
using Umbraco.Core.Services;
using Newtonsoft.Json;
using CoreLibrary.Helpers;
using CoreLibrary.Services;
using Umbraco.Core.Models;


namespace CoreLibrary.Controllers
{

    public class Note2SurfaceController : SurfaceController
    {

        public readonly IContentService contentService;
        public Note2SurfaceController(IContentService contentService)
        {
            this.contentService = contentService;
        }
        private readonly IContentNoteService contentNoteService;
        private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;
        public readonly IMediaUploadService mediaUploadService;
        public Note2SurfaceController(IContentNoteService contentNoteService, IMediaUploadService mediaUploadService, IContentTypeBaseServiceProvider contentTypeBaseServiceProvider)
        {
            this.contentNoteService = contentNoteService;
            this.mediaUploadService = mediaUploadService;
            _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
        }
     
        public ActionResult AddNote()
        {
            return PartialView("/Views/NoteSurface/_NoteForm.cshtml", new NoteModel());
        }
        
        public ActionResult CopyYourNote(string NodeKey, string BoardKey, string name, string PageId)
        {
            NoteModel CopyNote = new NoteModel();  
            IContent Parent = contentNoteService.GetParent(BoardKey);
            var notesJSON = Parent.GetValue<string>("notes");
            var noteValz = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(notesJSON);
            CopyNote = NoteTools.SetUpModel.GetOriginal(noteValz, NodeKey);
            CopyNote = NoteTools.SetUpModel.GetData(CopyNote, NodeKey, BoardKey, name);  
            CopyNote.PageId = Int32.Parse(PageId);
            return PartialView("/Views/NoteSurface/_CopyNoteForm.cshtml", CopyNote);
        }
        public ActionResult MoveYourNote(string NodeKey, string BoardKey, string name, string PageId)
        {
            NoteModel model = new NoteModel();
            IContent Parent = contentNoteService.GetParent(BoardKey);
            var notesJSON = Parent.GetValue<string>("notes");
            var noteValz = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(notesJSON);
            model = NoteTools.SetUpModel.GetOriginal(noteValz, NodeKey);
            model = NoteTools.SetUpModel.GetData(model, NodeKey, BoardKey, name);
            model.PageId = Int32.Parse(PageId);
            return PartialView("/Views/NoteSurface/_MoveNoteForm.cshtml", model);
        }
        public ActionResult EditYourNote(string NodeKey, string BoardKey, string name, string PageId)
        {

            NoteModel EditNote = new NoteModel();

            EditNote = NoteTools.SetUpModel.EditData(NodeKey, BoardKey, name, PageId);
            // Sets values in EditNote Model to be sent to form 
            EditNote = contentNoteService.GetNoteItem("notes", EditNote);
            return PartialView("/Views/NoteSurface/_EditNoteForm.cshtml", EditNote);
        }
        [HttpPost]
        public ActionResult CreateNote(NoteModel notes)
        {
            notes.Creator = contentNoteService.GetUser();
            bool createnote = contentNoteService.CreateNewNoteItem("notes", notes);
            ModelState.Clear();
            return CurrentUmbracoPage();
        }
        
        [HttpPost]
        public ActionResult EditNote(NoteModel notes)
        {
            var Page = notes.PageId;
            bool complete = contentNoteService.EditNoteItem("notes", notes);
            return RedirectToUmbracoPage(Page);
        }
        [HttpPost]
        public ActionResult CopyNote(NoteModel notes)
        {
            
            int Page = notes.PageId;
            bool complete = contentNoteService.CopyNoteItem("notes", notes);
            return RedirectToUmbracoPage(Page);
        }
        [HttpPost]
        public ActionResult MoveNote(NoteModel notes)
        {
            
            int Page = notes.PageId;
            bool complete = contentNoteService.MoveNoteItem("notes", notes); 
            return RedirectToUmbracoPage(Page);
        }
        
    }
}
