
$(document).ready(function () {
    //debugger;
    $('#images tbody').sortable({ update: Update }).disableSelection();
});

function Update(event, ui) {
    var images = new Array();

    $(".name").each(function () {
//        var imagePosition = new Object();
//        imagePosition.Name = this.innerText;
//        imagePosition.Position = this.parentElement.rowIndex;
//        images.push(imagePosition);

        images.push(this.innerText);
    });

    var ajaxData = ({ __RequestVerificationToken: $('[name=__RequestVerificationToken]').attr('value'), 
        images: images, imageGalleryName : $('#imageGalleryName').val() });    

    $.ajax({
        type: 'POST',
        data: ajaxData,
        url: 'Reorder',
        traditional: true
    });
}

