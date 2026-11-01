/* CRUD genérico: modales de crear/editar, detalle y eliminar. */
(function () {
    'use strict';

    function showValue(el, value) {
        if (value === null || value === undefined || value === '') {
            el.textContent = '—';
        } else if (typeof value === 'boolean') {
            el.textContent = value ? 'Sí' : 'No';
        } else {
            el.textContent = String(value).replace('T', ' ');
        }
    }

    function fetchDetails(url, id) {
        return fetch(url + '?id=' + encodeURIComponent(id), {
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            credentials: 'same-origin'
        }).then(function (response) {
            if (!response.ok) { throw new Error('No se pudo cargar el registro.'); }
            return response.json();
        });
    }

    function fillForm(form, data) {
        Array.prototype.forEach.call(form.elements, function (el) {
            if (!el.name || !(el.name in data) || el.type === 'hidden' && el.name !== 'Id') { return; }
            var value = data[el.name];
            if (el.type === 'checkbox') {
                el.checked = value === true;
            } else if (el.tagName === 'SELECT' && el.multiple) {
                var selected = (value || []).map(String);
                Array.prototype.forEach.call(el.options, function (opt) {
                    opt.selected = selected.indexOf(opt.value) >= 0;
                });
            } else {
                el.value = value === null || value === undefined ? '' : value;
            }
        });
    }

    function resetForm(form) {
        form.reset();
        form.elements['Id'].value = 0;
    }

    function init() {
        var root = document.getElementById('crud-root');
        if (!root) { return; }

        var formModalEl = document.getElementById('formModal');
        var detailsModalEl = document.getElementById('detailsModal');
        var deleteModalEl = document.getElementById('deleteModal');
        var form = document.getElementById('entityForm');
        var title = document.getElementById('formModalTitle');
        var detailsUrl = root.getAttribute('data-details-url');
        var singular = root.getAttribute('data-singular');

        document.addEventListener('click', function (event) {
            var button = event.target.closest('[data-crud]');
            if (!button) { return; }

            var action = button.getAttribute('data-crud');
            var id = button.getAttribute('data-id');

            if (action === 'create') {
                resetForm(form);
                form.action = form.getAttribute('data-create-action');
                title.textContent = 'Agregar ' + singular;
                bootstrap.Modal.getOrCreateInstance(formModalEl).show();
            } else if (action === 'edit') {
                fetchDetails(detailsUrl, id).then(function (data) {
                    resetForm(form);
                    fillForm(form, data);
                    form.action = form.getAttribute('data-edit-action');
                    title.textContent = 'Editar ' + singular;
                    bootstrap.Modal.getOrCreateInstance(formModalEl).show();
                }).catch(function (e) { alert(e.message); });
            } else if (action === 'details') {
                fetchDetails(detailsUrl, id).then(function (data) {
                    Array.prototype.forEach.call(detailsModalEl.querySelectorAll('[data-field]'), function (el) {
                        showValue(el, data[el.getAttribute('data-field')]);
                    });
                    bootstrap.Modal.getOrCreateInstance(detailsModalEl).show();
                }).catch(function (e) { alert(e.message); });
            } else if (action === 'delete') {
                document.getElementById('deleteId').value = id;
                document.getElementById('deleteName').textContent = button.getAttribute('data-name') || ('#' + id);
                bootstrap.Modal.getOrCreateInstance(deleteModalEl).show();
            }
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
}());
