$(document).ready(function () {

    console.log("Jquery Listo");


    
    $("#btnAplicarFiltros").click(function () {
        console.log($("#hNombre").val());
        $("#hNombre").val($("#nombre").val());
        console.log($("#hNombre").val());
    });


});
