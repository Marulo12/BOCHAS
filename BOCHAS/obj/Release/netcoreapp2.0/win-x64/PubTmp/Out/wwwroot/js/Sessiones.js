$(document).ready(function () {

    $("#BtnSessionMostrar").click(function () {
        MostrarSessiones();
    });

});

function MostrarSessiones() {

    if (ComprobarCampos()) {
        var fechadesde = $("#fechadesde").val();
        var fechahasta = $("#fechahasta").val();

        $("#DivSessiones").html('<img src="../images/carga.gif" alt="" width="200" style="margin-left:40%;"/>')

        $.ajax({
            type: "GET",
            url: "/Sessions/MostrarSessiones",
            data: { fechadesde, fechahasta },
            success: function (response) {

                $("#DivSessiones").html(response);
                $("#TablaSessiones").DataTable({
                    searching: true,
                    "scrollX": true,
                    responsive: true,
                    search: "Filtro&nbsp;:",
                    language: {
                        processing: "Procesando",
                        search: "Filtro&nbsp;:",

                        info: "",
                        infoEmpty: "",
                        zeroRecords: "Ningun registro coincide",
                        infoFiltered: "(Filtrado de _MAX_ total de registros)",
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
                });
            }
        });
    }
}

function ComprobarCampos() {
    if ($("#fechadesde").val() === "") {

        alertify.error('No cargo la fecha desde');
        return false;
    }
    if ($("#fechahasta").val() === "") {
        alertify.error('No cargo la fecha hasta');
        return false;
    }
   

    return true;
}