$(document).ready(function () {
    // Inicializar DataTable
    $('#vacacionesTable').DataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthChange": false,
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.16/i18n/Spanish.json"
        }
    });
});

// Función para pasar el motivo de rechazo al formulario
$('textarea').on('input', function () {
    var solicitudId = $(this).closest('.modal').attr('id').split('_')[1];
    $('#motivoRechazoInput_' + solicitudId).val($(this).val());
});