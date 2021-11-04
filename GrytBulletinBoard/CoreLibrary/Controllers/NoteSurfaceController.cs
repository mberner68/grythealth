using System.Web.Mvc;
using Umbraco.Web.Mvc;
using CoreLibrary.Models;
using Umbraco.Core.Services;
using CoreLibrary.Helpers;
using CoreLibrary.Services;


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
        public string GetPageStringProperty(string PropertyName, string defaultVal)
        {
            
            var page = CurrentPage;
            if (page == null) { return "xxx"; }
            string result = (string)page.GetProperty(PropertyName).GetValue();
            if (result == null || result == "") { return defaultVal; }
            return result;
        }

        public ActionResult AddNote()
        {
            NoteModel UseModel = new NoteModel();
            UseModel.question1 = GetPageStringProperty("question1","How are you feeling");
            //  return PartialView("/Views/NoteSurface/_NoteForm.cshtml", UseModel);
                return PartialView("Bulletin Board/NotePopUpForm", UseModel);
        }

        public ActionResult EditYourNote(string NodeKey, string BoardKey, string name, string PageId)
        {

            NoteModel EditNote = new NoteModel();

            EditNote = NoteTools.SetUpModel.EditData(NodeKey, BoardKey, name, PageId);
            // Sets values in EditNote Model to be sent to form 
            EditNote = contentNoteService.GetNoteItem("notes", EditNote);
            return PartialView("Bulletin Board/NoteEditPopUpForm", EditNote);
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
        
        
    }
}
