using System;
using System.Web;

namespace CoreLibrary.Models
{
    public class NoteModel
    {
        public int Id { get; set; }
        public string Board { get; set; }
        public Guid NodeKey { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public string ClassName { get; set; }
        public string Creator { get; set; }
        public string BoardKey { get; set; }
        public int PageId { get; set; }
        public string NewBoardKey { get; set; }
        public string question1 {get; set;}
      //  public string Avatar { get; set; }
      //  public HttpPostedFileBase File { get; set; }
        public bool Marketing1 { get; set; }
        public string Marketing { get; set; }
        public NoteModel() { }
        
    }
}
