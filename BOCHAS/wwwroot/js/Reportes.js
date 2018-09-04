function visor(nombre) {
    $("#servicio").val(nombre);
alertify.genericDialog || alertify.dialog('genericDialog', function () {
    return {
        main:function(content){
            this.setContent(content);
        },
        setup:function(){
            return {
                focus:{
                    element:function(){
                        return this.elements.body.querySelector(this.get('selector'));
                    },
                    select:true
                },
                options:{
                    basic:true,
                    maximizable:false,
                    resizable:false,
                    padding:false
                }
            };
        },
        settings:{
            selector:undefined
        }
    };
});
//force focusing password box
    alertify.genericDialog($('#loginForm')[0]);
}

function Generar() {
    var FecD = $("#FecD").val();
    var FecH = $("#FecH").val();
    if ($("#FecD").val() !== "" && $("#FecH").val() !== "") {

        window.open("/Reportes/" + $("#servicio").val() + "?FecD=" + FecD + "&FecH=" + FecH);
        window.location = "/Reportes/Index";
        /* $.ajax({
            type: "Get",
            data: { FecD, FecH },
            url: "/Reportes/" + $("#servicio").val()            
        });*/

    }

}