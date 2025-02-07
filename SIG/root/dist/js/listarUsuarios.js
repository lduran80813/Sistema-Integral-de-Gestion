$(document).ready(function () {
    $('#tablaUsuarios').DataTable({
        scrollX: false, // Evita la expansión horizontal innecesaria
        autoWidth: false, // Desactiva el ajuste automático de ancho
        responsive: true // Hace que la tabla se adapte mejor en pantallas pequeñas
    });
});