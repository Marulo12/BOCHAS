$('#InputImagen').change(function (e) {
    addImage(e);
});

function addImage(e) {
    var file = e.target.files[0],
        imageType = /image.*/;

    if (!file.type.match(imageType))
        return;

    var reader = new FileReader();
    reader.onload = fileOnload;
    reader.readAsDataURL(file);
}

function fileOnload(e) {
    var result = e.target.result;
    $('#imgSalida').attr("src", result);
    $('#imgSalida').css("display","inline-block");
}



$.ajax({
    type: "GET",
    url: "/Noticias/Alertas",
    success: function (response) {

        var li = '';
        var count = 0;
        for (var i = 0; i < response.reservas.length; i++) {
            li += '<li><a href="#" class="notification-item"><span class="dot bg-warning"></span>Se caduco la reserva confirmada N°' + response.reservas[i].numero + ' de ' + response.reservas[i].nombre + '</a></li>';
            count++;
        }
         for (var ii = 0; ii < response.clases.length; ii++) {
           li += '<li><a href="#" class="notification-item"><span class="dot bg-warning"></span>Se caduco la clase particular confirmada N°' + response.clases[ii].numero + ' de ' + response.clases[ii].nombre + ' </a></li>';
          count++;
         }
        if (count > 0) {
            $("#LAlerta").html(String(count));
            $("#Alertas").html(li);
        }
        else {
            $("#LAlerta").html("0");
        }


    }

});



