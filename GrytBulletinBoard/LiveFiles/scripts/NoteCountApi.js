function getNoteCount(BoardId, BoardName, Total, countafter)
{
   
            try
            {
                    var postData = { BoardId,  BoardName }
            
                        $.ajax({
            
                                url: '/umbraco/api/GrytBoardApi/GetNoteCount',
                                type: "POST",
                                data: JSON.stringify(postData),
                                cache: false,
                                dataType: "json",
                                contentType: 'application/json; charset=utf-8'
            
                        })
                
                .done(function (data, textStatus, jqXHR) {window[countafter](data, BoardName);})
                .fail(function (jqXHR, textStatus, errorThrown) {alert(errorThrown);})
                
            }
            catch (err)
            {
                 alert(err.message);
                 return;
            }

  }
    function countAfter(data, BoardName)
    {
            alert ( BoardName+" : "+data+ " Notes" )
    }
                        
