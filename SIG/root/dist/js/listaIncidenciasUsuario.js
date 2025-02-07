$(document).ready(function () {
    $("#tablaTickets").DataTable({
        language: {
            url: '//cdn.datatables.net/plug-ins/2.0.2/i18n/es-ES.json'
        }
    });
})
$(document).on("click", ".AbrirModal", function () {
    $("#id_ticket").val($(this).attr('data-id'));
    $("#Nombre").text($(this).attr('data-name'));
});