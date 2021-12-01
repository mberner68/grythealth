$(document).ready(function () {
        $("#Note").on('keydown', function (e) {
            var words = $.trim(this.value).length ? this.value.match(/\S+/g).length : 0;
            if (words <= 45) {
                $('#display_count').text(words);
                $('#word_left').text(45 - words)
            } else {
                if (e.which !== 8) e.preventDefault();
            }
        });
    });
              
