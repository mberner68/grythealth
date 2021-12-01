function deleteNote(NodeKey, BoardId, doafter)
{
   if (confirm("Do you really want to delete your note?"))
   {
            try
            {
                    var postData = { NodeKey, BoardId }
            
                        $.ajax({
            
                                url: '/umbraco/api/GrytBoardApi/DeleteNote',
                                type: "POST",
                                data: JSON.stringify(postData),
                                cache: false,
                                dataType: "json",
                                contentType: 'application/json; charset=utf-8'
            
                        })
                
                .done(function (data, textStatus, jqXHR) {window[doafter](NodeKey);})
                .fail(function (jqXHR, textStatus, errorThrown) {alert(errorThrown);})
        
            }
            catch (err)
            {
                 alert(err.message);
                 return;
            }

    }
    else 
    {
        alert ("Nothing Deleted!")
        
    }
  }
    function doAfter(NodeKey)
    {
            var Node = document.getElementById(NodeKey);
            if (Node != null)
            {
                Node.parentNode.removeChild(Node);
                alert ("Note has been deleted!")
            }
    }

                     
