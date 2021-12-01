function createNoteList(BoardId, BoardName, listafter)
{
            
            try
            {
                    var postData = { BoardId,  BoardName }
            
                        $.ajax({
            
                                url: '/umbraco/api/GrytBoardApi/CreateNoteList',
                                type: "POST",
                                data: JSON.stringify(postData),
                                cache: false,
                                dataType: "json",
                                contentType: 'application/json; charset=utf-8'
            
                        })
                
                .done(function (data, textStatus, jqXHR) {window[listafter](BoardName, data);})
                .fail(function (jqXHR, textStatus, errorThrown) {alert(errorThrown);})
            }
            catch (err)
            {
                 alert(err.message);
                 return;
            }

  }
    function listAfter(BoardName, data)
    {
        
        var tab = window.open('about:blank', '_blank');
        
        tab.document.writeln(BoardName + '\r\n'  + JSON.stringify(data, null, 5)); 
        tab.document.close(); 
   

         
    }
             
