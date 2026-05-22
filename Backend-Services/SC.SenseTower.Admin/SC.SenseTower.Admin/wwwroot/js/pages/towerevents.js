const sortingSelector = '#tower-events-grid thead';
const filteringSelector = '#filter-panel .card-body';

function loadData(page, pageSize) {
    if (!page) {
        page = getCurrentPage();
    }
    if (!pageSize) {
        pageSize = getPageSize();
    }
    let filters = filteringCriteria(filteringSelector);
    console.log(filters);
    let command = {
        page: page,
        pageSize: pageSize,
        sorting: sortingCriteria(sortingSelector),
        filters: filters
    };
    console.log(command);
    let $block = blockElement('#tower-events-grid');
    axios.post('getpage', command)
        .then((response) => {
            $('#tower-events-grid').html(response.data);
            window.onload();
            initGrid();
            initPaginator();
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка чтения данных. Подробности в консоли приложения.');
        })
        .then(() => {
            unblockElement($block);
        });
}

function filteringCriteria(selector) {
    let $panel = $(selector);
    let state = $panel.find('[name="state"]:checked').val();
    let from = $panel.find('#From').val();
    let upTo = $panel.find('#UpTo').val();
    let spaceId = $panel.find('#SpaceId').val();
    return {
        title: $panel.find('#Title').val(),
        state: state !== 'null' ? parseInt(state) : null,
        from: from ? from : null,
        upTo: upTo ? upTo : null,
        spaceId: spaceId ? spaceId : null
    };
}

function filteringClear(selector) {
    let $panel = $(selector);
    $panel.find('input').val('');
    $panel.find('#state1').prop('checked', true);
    $panel.find('select').val('');
}

function saveFilters(selector) {
    let filters = JSON.stringify(filteringCriteria(selector));
    localStorage.setItem("tower-events-filters", filters);
}

function restoreFilters(selector) {
    let filtersJson = localStorage.getItem("tower-events-filters");
    if (!filtersJson) return;

    let filters = JSON.parse(filtersJson);
    let $panel = $(selector);
    $panel.find('[name="state"]:checked').removeAttr('checked');
    $panel.find(`[name="state"][value="${filters.state}"]`).prop('checked', true);
    $panel.find('#Title').val(filters.title);
    $panel.find('#From').val(filters.from);
    $panel.find('#UpTo').val(filters.upTo);
    $panel.find('#SpaceId').val(filters.spaceId);
}

function initPaginator() {
    $('ul.pagination a.page-link').click((e) => {
        e.stopPropagation();
        loadData($(e.currentTarget).data('page'), getPageSize());
    });
    $('ul.pagination').closest('ul.navbar-nav').find('select').change((e) => {
        loadData(getCurrentPage(), $(e.target).val());
    });
}

function initGrid() {
    initSorting(sortingSelector, loadData);
    $('#tower-events-grid tbody').find('tr[data-id]').each((i, v) => {
        let $v = $(v);
        $v.click((e) => {
            e.stopPropagation();
            showItem($(e.currentTarget));
        });
        $v.find('a[data-action="add-tickets"]').click((e) => {
            e.stopPropagation();
            addTicketsDialog($v);
        });
        $v.find('a[data-action="view-tickets"]').click((e) => {
            e.stopPropagation();
            viewTicketsDialog($v);
        });
        $v.find('a[data-action="delete"]').click((e) => {
            e.stopPropagation();
            towerEventDelete($v.data('id'));
        });
    });
}

function showItem($tr) {
    let $block = blockElement($tr);
    let id = $tr.data('id');
    axios.get(`get/${id}`)
        .then((response) => {
            let $dialog = $(response.data);
            $dialog.appendTo('#tower-events-grid');
            $dialog.modal('show');
            window.onload();
            $dialog[0].addEventListener('hidden.bs.modal', (e) => {
                e.stopPropagation();
                $(e.target).remove();
            });
            $dialog.find('#remove-image').click((e) => {
                $('#ImageId').val('');
                $dialog.find('img').prop('src', '/img/no-photo.jpg');
            });
            let $form = $dialog.find('form');
            if ($form.length > 0) {
                $form.submit((e) => {
                    saveData(e, $dialog, false);
                });
            }
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Не получены данные события. Подробности в консоли приложения.');
        })
        .then(() => {
            unblockElement($block);
        });
}

function saveData(e, $dialog, isNew) {
    e.preventDefault();
    let $form = $dialog.find('form');
    let data = $form.serialize();
    let url = isNew ? 'create' : 'update';
    axios.post(url, data)
        .then((response) => {
            $dialog.modal('hide');
            toastr.success('Событие успешно сохранено.');
            loadData();
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка сохранения события. Подробности в консоли приложения.');
        });
}

function towerEventCreate() {
    let $block = blockElement($('body'));
    axios.get('create')
        .then((response) => {
            let $dialog = $(response.data);
            $dialog.appendTo('body');
            $dialog.modal('show');
            window.onload();
            $dialog[0].addEventListener('hidden.bs.modal', (e) => {
                e.stopPropagation();
                $(e.target).remove();
            });
            $dialog.find('#remove-image').click((e) => {
                $('#ImageId').val('');
                $dialog.find('img').prop('src', '/img/no-photo.jpg');
            });
            let $form = $dialog.find('form');
            if ($form.length > 0) {
                $form.submit((e) => {
                    saveData(e, $dialog, true);
                });
            }
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка получения формы создания галереи. Подробности в консоли приложения.');
        })
        .then(() => {
            unblockElement($block);
        });
}

function towerEventDelete(id) {
    if (!confirm("Удалить выбранное событие?"))
        return;
    axios.post(`delete/${id}`)
        .then((response) => {
            toastr.success('Событие успешно удалено.');
            loadData();
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка удаления события. Подробности в консоли приложения.');
        });
}

function addTicketsDialog($tr) {
    let $block = blockElement($tr);
    let id = $tr.data('id');
    axios.get(`addtickets/${id}`)
        .then((response) => {
            let $dialog = $(response.data);
            $dialog.appendTo('body');
            $dialog.modal('show');
            window.onload();
            $dialog[0].addEventListener('hidden.bs.modal', (e) => {
                e.stopPropagation();
                $(e.target).remove();
            });
            let $form = $dialog.find('form');
            if ($form.length > 0) {
                $form.submit((e) => {
                    addTickets(e, $dialog);
                });
            }
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Не получена форма добавления билетов. Подробности в консоли приложения.');
        })
        .then(() => {
            unblockElement($block);
        });
}

function addTickets(e, $dialog) {
    e.preventDefault();
    let $form = $dialog.find('form');
    let data = $form.serialize();
    axios.post('addtickets', data)
        .then((response) => {
            $dialog.modal('hide');
            toastr.success('Билеты успешно добавлены.');
            loadData();
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка добавления билетов. Подробности в консоли приложения.');
        });
}

function viewTicketsDialog($tr) {
    let $block = blockElement($tr);
    let id = $tr.data('id');
    axios.get(`viewtickets/${id}`)
        .then((response) => {
            let $dialog = $(response.data);
            $dialog.appendTo('body');
            $dialog.modal('show');
            window.onload();
            $dialog[0].addEventListener('hidden.bs.modal', (e) => {
                e.stopPropagation();
                $(e.target).remove();
            });
            $dialog.find('#remove-image').click((e) => {
                $('#ImageId').val('');
                $dialog.find('img').prop('src', '/img/no-photo.jpg');
            });
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Не получена форма просмотра билетов. Подробности в консоли приложения.');
        })
        .then(() => {
            unblockElement($block);
        });
}

$(document).ready(() => {
    $('#apply-filter').click((e) => {
        saveFilters(filteringSelector);
        loadData(1);
        hideFilters(filteringSelector);
    })
    $('#clear-filter').click((e) => {
        filteringClear(filteringSelector);
        saveFilters(filteringSelector);
        loadData(1);
        hideFilters(filteringSelector);
    });
    $('#add-tower-event').click(() => { towerEventCreate(); });
    initGrid();
    initPaginator();
    restoreFilters(filteringSelector);
    loadData(1);
});
