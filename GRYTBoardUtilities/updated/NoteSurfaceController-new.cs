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
using System.IO;

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
        public ActionResult AddNoteWithImage(string BoardKey, string name, string PageId)
        {
            NoteModel AvatarNote = new NoteModel();
            AvatarNote.Board = name;
            AvatarNote.PageId = Int32.Parse(PageId);
            AvatarNote.BoardKey = BoardKey;
            return PartialView("/Views/NoteSurface/_NoteFormImage.cshtml", AvatarNote);
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
        public ActionResult CreateNoteImage(NoteModel notes)
        {
            if (ModelState.IsValid)
            {
                if (notes.File != null && notes.File.ContentLength > 0)
                {
                    if (notes.File.ContentLength > 1024000)
                    {
                        ModelState.AddModelError("notes.File", "The size of the file should not exceed 1 MB");
                        return RedirectToCurrentUmbracoUrl();
                    }
                    var supportedtypes = new[] { "jpg", "jpeg", "png", "gif" };
                    var filext = Path.GetExtension(notes.File.FileName).Substring(1);
                    if (!supportedtypes.Contains(filext))
                    {
                        ModelState.AddModelError("notes.File", "Invalid type, only Jps or Jpeg please");
                        return CurrentUmbracoPage();

                    }

                    // Folder where you want to store the media item.
                    var parentMediaFolder = Services.MediaService.GetById(2210);
                    var avatarUdi = mediaUploadService.CreateMediaItemFromFileUpload(notes.File, parentMediaFolder, "Image");
                    notes.Avatar = avatarUdi;

                }
                // Grab User information
                notes.Creator = contentNoteService.GetUser();
                bool createnote = contentNoteService.CreateNewNoteItem("notes", notes);
                ModelState.Clear();
                return CurrentUmbracoPage();
            }
            else
            {
                return CurrentUmbracoPage();
            }

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
