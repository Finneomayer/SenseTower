const sortingSelector = '#invites-grid thead';
const filteringSelector = '#filter-panel .card-body';

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
    let $block = blockElement('#invites-grid');
    axios.post('getpage', command)
        .then((response) => {
            $('#invites-grid').html(response.data);
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
    return {
        statusName: $panel.find('[name="statusName"]:checked').val(),
        authorName: $panel.find('#AuthorName').val(),
        issuerName: $panel.find('#IssuerName').val(),
        userName: $panel.find('#UserName').val()
    };
}

function filteringClear(selector) {
    let $panel = $(selector);
    $panel.find('input:text').val('');
    $panel.find('#statusName1').prop('checked', true);
}

function saveFilters(selector) {
    let filters = JSON.stringify(filteringCriteria(selector));
    localStorage.setItem("invites-filters", filters);
}

function restoreFilters(selector) {
    let filtersJson = localStorage.getItem("invites-filters");
    if (!filtersJson) return;

    let filters = JSON.parse(filtersJson);
    let $panel = $(selector);
    $panel.find('#AuthorName').val(filters.authorName);
    $panel.find('#IssuerName').val(filters.issuerName);
    $panel.find('#UserName').val(filters.userName);
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
    $('#invites-grid tbody').find('tr[data-id]').each((i, v) => {
        let $v = $(v);
        $v.click((e) => {
            e.stopPropagation();
            showInvite($(e.currentTarget));
        });
        $v.find('a').click((e) => {
            e.stopPropagation();
            recallInvite($v.data('id'));
        });
    });
}

function showInvite($tr) {
    let $block = blockElement($tr);
    let id = $tr.data('id');
    axios.get(`get/${id}`)
        .then((response) => {
            let $dialog = $(response.data);
            $dialog.appendTo('#invites-grid');
            $dialog.modal('show');
            window.onload();
            $dialog[0].addEventListener('hidden.bs.modal', (e) => {
                e.stopPropagation();
                $(e.target).remove();
            });
            let $form = $dialog.find('form');
            if ($form.length > 0) {
                $form.submit((e) => {
                    recallPost(e, $dialog);
                });
            }
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Не получены данные приглашения. Подробности в консоли приложения.');
        })
        .then(() => {
            unblockElement($block);
        });
}

function recallInvite(id) {
    axios.get(`recall/${id}`)
        .then((response) => {
            let $dialog = $(response.data);
            $dialog.appendTo('#invites-grid');
            $dialog.modal('show');
            window.onload();
            $dialog.find('form').submit((e) => {
                recallPost(e, $dialog);
            });
            $dialog[0].addEventListener('hidden.bs.modal', (e) => {
                e.stopPropagation();
                $(e.target).remove();
            });
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Не получены данные приглашения. Подробности в консоли приложения.');
        });
}

function recallPost(e, $dialog) {
    e.preventDefault();
    let data = $dialog.find('form').serialize();
    axios.post('recall', data)
        .then((response) => {
            $dialog.modal('hide');
            toastr.success('Приглашение успешно отозвано.');
            loadData();
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка отзыва приглашения. Подробности в консоли приложения.');
        });
}

function userAdd(url) {
    axios.get(url)
        .then((response) => {
            let $dialog = $(response.data);
            $dialog.appendTo('body');
            window.onload();
            $dialog.modal('show');
            $dialog[0].addEventListener('hidden.bs.modal', (e) => {
                e.stopPropagation();
                $(e.target).remove();
            });
            $dialog.find('form').submit((e) => {
                e.preventDefault();
                let data = $dialog.find('form').serialize();
                axios.post('userbatchadd', data)
                    .then((response) => {
                        $dialog.modal('hide');
                        toastr.success('Приглашения успешно выданы: ' + response.data);
                        loadData();
                    })
                    .catch((error) => {
                        console.log(error);
                        toastr.error('Ошибка выдачи приглашений. Подробности в консоли приложения.');
                    });
            });
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка получения формы выдачи приглашений. Подробности в консоли приложения.');
        });
}

$(document).ready(() => {
    $('#admin-batch-add').click((e) => { userAdd('adminbatchadd'); });
    $('#user-batch-add').click((e) => { userAdd('userbatchadd'); });
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
