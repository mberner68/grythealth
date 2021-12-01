using System;
using Umbraco.Web.WebApi;
using System.Collections.Generic;
using CoreLibrary.Services;
using Umbraco.Core.Services;
using CoreLibrary.Models;
using Newtonsoft.Json;

namespace GrytCore.Controllers
{
   

    public class DeleteRequest
    {
        public string NodeKey { get; set; }
        public string BoardId { get; set; }
    }
    public class CreateFile
    {
        public string BoardId { get; set; }
        public string BoardName { get; set; }
    }
    public class GrabBoardCount
    {
        public string BoardId { get; set; }
        public string BoardName { get; set; }
        public int Total { get; set; }
    }


    public class GrytBoardApiController : UmbracoApiController
    {
        private readonly IContentNoteService contentNoteService;
        private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;

        public GrytBoardApiController(IContentNoteService contentNoteService, IContentTypeBaseServiceProvider contentTypeBaseServiceProvider)
        {
            this.contentNoteService = contentNoteService;

            _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
        }

        private string PartialView(string v, NoteModel editNote)
        {
            throw new NotImplementedException();
        }
       
        //umbraco/api/GrytBoardApi/DeleteNote
        [System.Web.Http.HttpPost]
        public string DeleteNote([System.Web.Http.FromBody] DeleteRequest data)
        {
            if (!string.IsNullOrEmpty(data.NodeKey))
            {
                string NoteId = contentNoteService.DeleteNoteItem(data, "notes");
                return "You have have called api using " + NoteId;
            }
            else
            {
                return "Nothing Deleted";
            }
        }
        //umbraco/api/GrytBoardApi/CreateNoteList
        [System.Web.Http.HttpPost]
        public List<Dictionary<string, string>> CreateNoteList([System.Web.Http.FromBody] CreateFile data)
        {
            var noteValz = new List<Dictionary<string, string>>();
            var Parent = contentNoteService.GetParent(data.BoardId);
            var notesJSON = Parent.GetValue<string>("notes");
            if (notesJSON != null)
            {
                noteValz = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(notesJSON);
               

            }
            return noteValz;

        }
        //umbraco/api/GrytBoardApi/CreateFile
        [System.Web.Http.HttpPost]
        public int GetNoteCount([System.Web.Http.FromBody] GrabBoardCount data)
        {
            int NoteCount = 0;
            data.Total = 0;
            var Parent = contentNoteService.GetParent(data.BoardId);
            var notesJSON = Parent.GetValue<string>("notes");
            if (notesJSON != null)
            {
                var noteValz = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(notesJSON);
               NoteCount = noteValz.Count;
                data.Total = NoteCount;
                
            }
            return data.Total;

        }

    }  


    
}


