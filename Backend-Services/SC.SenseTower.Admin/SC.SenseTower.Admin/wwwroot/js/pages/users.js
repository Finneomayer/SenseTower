const sortingSelector = '#users-grid thead';
const filteringSelector = '#filter-panel .card-body';

//function fixAccounts() {
//    axios.post('fixaccounts')
//        .then((response) => {
//            toastr.success('Починено!');
//        })
//        .catch((error) => {
//            console.log(error);
//            toastr.error('Что-то пошло не так...');
//        });
//}

function loadData(page, pageSize) {
    if (!page) {
        page = getCurrentPage();
    }
    if (!pageSize) {
        pageSize = getPageSize();
    }
    let command = {
        currentPage: page,
        pageSize: pageSize,
        sorting: sortingCriteria(sortingSelector),
        filters: filteringCriteria(filteringSelector)
    };
    let $block = blockElement('#users-grid');
    axios.post('getpage', command)
        .then((response) => {
            $('#users-grid').html(response.data);
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
    let roleId = $panel.find('#RoleId').val();
    return {
        userName: $panel.find('#UserName').val(),
        email: $panel.find('#Email').val(),
        roleId: roleId ? roleId : null,
        statusName: $panel.find('[name="statusName"]:checked').val()
    };
}

function filteringClear(selector) {
    let $panel = $(selector);
    $panel.find('input:text').val('');
    $panel.find('select')[0].selectedIndex = 0;
    $panel.find('#statusName1').prop('checked', true);
}

function saveFilters(selector) {
    let filters = JSON.stringify(filteringCriteria(selector));
    localStorage.setItem("users-filters", filters);
}

function restoreFilters(selector) {
    let filtersJson = localStorage.getItem("users-filters");
    if (!filtersJson) return;

    let filters = JSON.parse(filtersJson);
    let $panel = $(selector);
    $panel.find('#UserName').val(filters.userName);
    $panel.find('#Email').val(filters.email);
    $panel.find('#RoleId').val(filters.roleId);
    $panel.find('[name="statusName"]:checked').removeAttr('checked');
    $panel.find(`[name="statusName"][value="${filters.statusName}"]`).prop('checked', true);
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
    $('#users-grid tbody').find('tr[data-id]').each((i, v) => {
        let $v = $(v);
        $v.click((e) => {
            e.stopPropagation();
            showItem($(e.currentTarget));
        });
        $v.find('a').click((e) => {
            e.stopPropagation();
            itemAction($(e.currentTarget));
        });
    });
}

function showItem($tr) {
    let $block = blockElement($tr);
    let id = $tr.data('id');
    axios.get(`get/${id}`)
        .then((response) => {
            let $dialog = $(response.data);
            $dialog.appendTo('#users-grid');
            $dialog.modal('show');
            window.onload();
            $dialog[0].addEventListener('hidden.bs.modal', (e) => {
                e.stopPropagation();
                $(e.target).remove();
            });
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Не получены данные пользователя. Подробности в консоли приложения.');
        })
        .then(() => {
            unblockElement($block);
        });
}

function itemAction($link) {
    let $tr = $link.closest('tr');
    let id = $tr.data('id');
    switch ($link.data('action')) {
        case 'upload-image':
            showImageDialog(id, $tr);
            break;
        case 'ban':
            userBan(id);
            break;
        case 'unban':
            userUnban(id);
            break;
        case 'delete':
            userDelete(id);
            break;
    };
}

function showImageDialog(id, $tr) {
    let $block = blockElement($tr);
    axios.get(`upload/${id}`)
        .then((response) => {
            let $dialog = $(response.data);
            $dialog.appendTo('#users-grid');
            $dialog.modal('show');
            window.onload();
            $dialog[0].addEventListener('hidden.bs.modal', (e) => {
                e.stopPropagation();
                $(e.target).remove();
            });
            let $form = $dialog.find('form');
            if ($form.length > 0) {
                $form.submit((e) => {
                    uploadImage(e, $dialog);
                });
            }
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Не получена форма загрузки изображения. Подробности в консоли приложения.');
        })
        .then(() => {
            unblockElement($block);
        });
}

function uploadImage(e, $dialog) {
    e.preventDefault();
    let formData = new FormData();
    formData.append("name", $dialog.find('#name').val());
    formData.append("file", $dialog.find('#file')[0].files[0]);
    formData.append("userId", $dialog.find('#userId').val());
    let token = $dialog.find('#token').val();
    let rootUrl = $dialog.find('#imagesRootUrl').val();
    axios.post(rootUrl + 'images/add', formData, {
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'multipart/form-data'
        }
    })
        .then((response) => {
            $dialog.modal('hide');
            toastr.success('Изображение успешно загружено.');
            loadData();
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка загрузки изображения. Подробности в консоли приложения.');
        });
}

function userBan(id) {
    axios.get(`ban/${id}`)
        .then((response) => {
            let $dialog = $(response.data);
            $dialog.appendTo('#users-grid');
            $dialog.modal('show');
            window.onload();
            $dialog.find('#IsPermanent').change((e) => {
                $dialog.find('#LockoutEnd').prop('disabled', $(e.target).is(':checked'));
            });
            $dialog.find('form').submit((e) => {
                e.preventDefault();
                let data = $dialog.find('form').serialize();
                axios.post('ban', data)
                    .then((response) => {
                        toastr.success('Пользователь заблокирован.');
                        loadData();
                        $dialog.modal('hide');
                    })
                    .catch((error) => {
                        console.log(error);
                        toastr.error('Ошибка блокировки пользователя. Подробности в консоли приложения.');
                    });
            });
            $dialog[0].addEventListener('hidden.bs.modal', (e) => {
                e.stopPropagation();
                $(e.target).remove();
            });
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка чтения формы блокировки пользователя. Подробности в консоли приложения.');
        });
}

function userUnban(id) {
    let $block = blockElement($(`#users-grid tr[data-id="${id}"]`));
    axios.post(`unban/${id}`)
        .then((response) => {
            toastr.success('Пользователь разблокирован.');
            loadData();
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка разблокировки пользователя. Подробности в консоли приложения.');
        })
        .then(() => {
            unblockElement($block);
        });
}

function userDelete(id) {
    if (!confirm("Удалить текущего пользователя?"))
        return;
    let $block = blockElement($(`#users-grid tr[data-id="${id}"]`));
    axios.post(`delete/${id}`)
        .then((response) => {
            toastr.success('Пользователь удалён.');
            loadData();
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка удаления пользователя. Подробности в консоли приложения.');
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
    initGrid();
    initPaginator();
    restoreFilters(filteringSelector);
    loadData(1);
});
