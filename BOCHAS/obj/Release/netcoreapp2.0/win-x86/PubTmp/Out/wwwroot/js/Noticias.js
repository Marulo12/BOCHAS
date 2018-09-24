$("#AdministrarNoticias").click(function () {
    $("#AdministrarNoticias").tooltip("hide");
     
    TraerNoticias();

});

function TraerNoticias() {
    $("#ModalNoticias").modal();
    $.ajax({
        type: "GET",
        url: "/Noticias/ConocerNoticias",       
        success: function (response) {
           
            var armatabla = '<table id=TablaNoti class="table" style="width:100%;"><thead  style="background-color: rgba(158, 44, 44, 0.9);color:white"><tr><th>Imagen</th><th>Título</th><th>Descripcion</th><th>Opción</th></tr></thead><tbody>';
            for (var i = 0; i < response.length; i++) {
                armatabla += ' <tr><td><img style="width:220px;height:100px;" src="' + response[i].url + '" /></td><td>' + response[i].titulo + '</td><td>' + response[i].descripcion + '</td><td><button class="btn btn-sm btn-danger" data-toggle="tooltip" title="Baja" data-placement="top" onclick="confirmarBajaNoti(' + response[i].id + ');"><i class="fas fa-trash-alt"></i></button></td></tr>';
            }
            armatabla += '</tbody></table>';
            $("#ModalNoticias .modal-body").empty();
         
            $("#ModalNoticias .modal-body").append(armatabla);
            $("#TablaNoti").DataTable({
              
                responsive: true,                      
                lengthMenu: [2, 5, 20, 75, 100],
                language: {
                    processing: "Procesando",
                    search: "Filtro&nbsp;:",
                    info: "Pagina _PAGE_ de _PAGES_  / <b>Total de Registros: _MAX_</b> ",
                    infoEmpty: "",
                    infoFiltered: "",
                    zeroRecords: "Ningun registro coincide",
                    lengthMenu: "Mostrar _MENU_ registros",
                    infoPostFix: "",
                    loadingRecords: "Cargando...",
                    emptyTable: "No hay registros",
                    paginate: {
                        first: "Primero",
                        previous: "Anterior",
                        next: "Siguiente",
                        last: "Ultimo"
                    }
                }
            }  );
          
            
        }


    });


}

function confirmarBajaNoti(id) {
    var ids = id;
    alertify.confirm('Noticias', 'Seguro desea eliminar la Noticia?', function () {
        $.ajax({
            type: "POST",
            url: "/Noticias/BajadeNoticia",
            data: { id: ids },
            success: function (response) {

                if (response === "OK") {
                    alertify.success("Noticia Eliminada");

                } else {
                    alertify.error("Ocurrio un error en la operacion");
                }
                
                setTimeout(function () { window.location = "/Home/Index"; }, 1000);
            },
            error: function (e) {
                window.alert(e.responseText + e.responseJSON);
                console.log(e.responseText + e.responseJSON);
            }

        });

    }
        , function () { alertify.error('Operacion Cancelada') });

}

