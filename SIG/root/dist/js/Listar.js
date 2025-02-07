$(document).ready(function () {
    $('#tablaUsuarios').DataTable({
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.16/i18n/Spanish.json"
        },
        "searching": true,
        "pageLength": 20,
        "lengthMenu": [[10, 25, 50, 100], [10, 25, 50, 100]],
   

    });
});




function eliminarEmpleado(id) {
    if (confirm('¿Está seguro de que desea eliminar este usuario?')) {
        $.ajax({
            url: '@Url.Action("EliminarEmpleado", "Empleado")',
            type: 'POST',
            data: { id: id },
            success: function (result) {

                location.reload();
            },
            error: function (xhr, status, error) {
                alert('Ocurrió un error al eliminar el usuario: ' + error);
            }
        });
    }
}





function restaurarEmpleado(id) {
    if (confirm('¿Está seguro de que desea restaurar este usuario?')) {
        window.location.href = '@Url.Action("RestaurarEmpleado", "Login")/' + id;
    }
}



$(document).ready(function () {
    $('#tablaEntregas').DataTable({
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.16/i18n/Spanish.json"
        },
        "paging": true,
        "searching": true,
        "ordering": true,
        "lengthChange": false,
        "pageLength": 10,
    });
});
