const sortingSelector = '#places-grid thead';
const filteringSelector = '#filter-panel .card-body';

function loadData(page, pageSize) {
    if (!page) {
        page = getCurrentPage();
    }
    if (!pageSize) {
        pageSize = getPageSize();
    }
    let command = {
        page: page,
        pageSize: pageSize,
        sorting: sortingCriteria(sortingSelector),
        filters: filteringCriteria(filteringSelector)
    };
    let $block = blockElement('#places-grid');
    axios.post('getpage', command)
        .then((response) => {
            $('#places-grid').html(response.data);
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
    let ownerId = $panel.find('#OwnerId').val();
    let spaceId = $panel.find('#SpaceId').val();
    let accessType = $panel.find('[name="accessType"]:checked').val();
    return {
        placeName: $panel.find('#PlaceName').val(),
        ownerId: ownerId ? ownerId : null,
        spaceId: spaceId ? spaceId : null,
        publicAccessType: accessType !== 'null' ? parseInt(accessType) : null
    };
}

function filteringClear(selector) {
    let $panel = $(selector);
    $panel.find('input:text').val('');
    $panel.find('select').val('');
    $panel.find('#accessType1').prop('checked', true);
}

function saveFilters(selector) {
    let filters = JSON.stringify(filteringCriteria(selector));
    localStorage.setItem("places-filters", filters);
}

function restoreFilters(selector) {
    let filtersJson = localStorage.getItem("places-filters");
    if (!filtersJson) return;

    let filters = JSON.parse(filtersJson);
    let $panel = $(selector);
    $panel.find('#PlaceName').val(filters.placeName);
    $panel.find('#OwnerId').val(filters.ownerId);
    $panel.find('#SpaceId').val(filters.spaceId);
    $panel.find('[name="accessType"]:checked').removeAttr('checked');
    $panel.find(`[name="accessType"][value="${filters.publicAccessType}"]`).prop('checked', true);
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
    $('#places-grid tbody').find('tr[data-id]').each((i, v) => {
        let $v = $(v);
        $v.click((e) => {
            e.stopPropagation();
            showItem($(e.currentTarget));
        });
        $v.find('a[data-action="delete"]').click((e) => {
            e.stopPropagation();
            placeDelete($v.data('id'));
        });
        $v.find('a[data-action="edit-images"]').click((e) => {
            e.stopPropagation();
            imageList($(e.currentTarget), $v.data('id'));
        });
    });
}

function showItem($tr) {
    let $block = blockElement($tr);
    let id = $tr.data('id');
    axios.get(`get/${id}`)
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
                    saveData(e, $dialog, false);
                });
            }
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Не получены данные помещения. Подробности в консоли приложения.');
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
            toastr.success('Помещение успешно сохранено.');
            loadData();
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка сохранения помещения. Подробности в консоли приложения.');
        });
}

function placeCreate() {
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
            toastr.error('Ошибка получения формы создания помещения. Подробности в консоли приложения.');
        })
        .then(() => {
            unblockElement($block);
        });
}

function placeDelete(id) {
    if (!confirm("Удалить выбранное помещение?"))
        return;
    axios.post(`delete/${id}`)
        .then((response) => {
            toastr.success('Помещение успешно удалено.');
            loadData();
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка удаления помещения. Подробности в консоли приложения.');
        });
}

function imageList($tr, id) {
    let $block = blockElement($tr);
    axios.get(`images/${id}`)
        .then((response) => {
            let $dialog = $(response.data);
            $dialog.appendTo('body');
            $dialog.modal('show');
            window.onload();
            $dialog[0].addEventListener('hidden.bs.modal', (e) => {
                e.stopPropagation();
                $(e.target).remove();
            });
            $('#save-image-list').click((e) => {
                let places = $('.place-image-list-item');
                let images = [];
                for (let i = 0; i < places.length; i++) {
                    let $imageSelect = $(places[i]).find('[name="image-id"]');
                    if ($imageSelect.val()) {
                        let image = {
                            imageId: $imageSelect.val(),
                            location: i
                        };
                        images.push(image);
                    }
                };
                let data = new FormData();
                images.forEach((v, i) => {
                    data.append(`images[${i}][imageId]`, v.imageId);
                    data.append(`images[${i}][location]`, v.location);
                })
                axios.post(`images/${id}`, data)
                    .then(() => {
                        $dialog.modal('hide');
                        toastr.success('Изображения успешно сохранены.');
                        loadData();
                    })
                    .catch((error) => {
                        console.log(error);
                        toastr.error('Ошибка сохранения изображений. Подробности в консоли приложения.');
                    });
            });
            $('.place-image-list-item button[name="remove-image"]').click((e) => {
                let $imagePanel = $(e.currentTarget).closest('.place-image-list-item');
                let $select = $imagePanel.find('select');
                $select.val('');
                let $img = $imagePanel.find('img');
                let staticRootUrl = $('#static-root-url').val();
                $img[0].src = `${staticRootUrl}img/no-photo.jpg`;
            });
            $('.place-image-list-item').draggable({
                axis: 'y',
                containment: '.place-image-list',
                start: (e, ui) => {
                    $(e.target).find('.mover').css('cursor', 'grabbing');
                },
                stop: (e, ui) => {
                    $(e.target).find('.mover').css('cursor', 'grab');
                }
            });
            $('.place-image-place').droppable({
                over: (e, ui) => {
                    $(e.target).css('background-color', 'orange');
                },
                out: (e, ui) => {
                    $(e.target).css('background-color', 'inherit');
                },
                drop: (e, ui) => {
                    $(e.target).css('background-color', 'inherit');
                    let $draggedImage = $(ui.draggable[0]);
                    let $sourcePlace = $draggedImage.parent();
                    let $targetPlace = $(e.target);
                    $draggedImage.detach();
                    if ($targetPlace[0].children.length > 0) {
                        let $targetImage = $($targetPlace[0].children[0]);
                        $targetImage.detach();
                        $targetImage.appendTo($sourcePlace);
                    }
                    $draggedImage.removeAttr('style');
                    $draggedImage.appendTo($targetPlace);
                }
            });
            $dialog.find('select[name="image-id"]').change((e) => {
                let $select = $(e.currentTarget);
                let imageId = $select.val();
                let $img = $select.closest('.place-image-list-item').find('img');
                let rootUrl = $('#image-root-url').val();
                let staticRootUrl = $('#static-root-url').val();
                $img[0].src = imageId ? `${rootUrl}api/v1/images/download/${imageId}?preview=true` : `${staticRootUrl}img/no-photo.jpg`;
            });
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка получения списка изображений. Подробности в консоли приложения.');
        })
        .then(() => {
            unblockElement($block);
        });
}

function imageAdd($tr, id) {
    let $block = blockElement($tr);
    axios.get(`addimage/${id}`)
        .then((response) => {
            let $dialog = $(response.data);
            $dialog.appendTo('#galleries-grid');
            $dialog.modal('show');
            window.onload();
            $dialog[0].addEventListener('hidden.bs.modal', (e) => {
                e.stopPropagation();
                $(e.target).remove();
            });
            let $form = $dialog.find('form');
            if ($form.length > 0) {
                $form.submit((e) => {
                    addImage(e, $dialog);
                });
            }
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка получения списка изображений. Подробности в консоли приложения.');
        })
        .then(() => {
            unblockElement($block);
        });
}

function addImage(e, $dialog) {
    e.preventDefault();
    let $form = $dialog.find('form');
    let $name = $form.find('#Name');
    if (!$name.val()) {
        $name.val() = $form.find('#ImageId option:selected').text();
    }
    let data = $form.serialize();
    axios.post('addimage', data)
        .then((response) => {
            $dialog.modal('hide');
            toastr.success('Изображение успешно добавлено.');
            loadData();
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка добавления изображения. Подробности в консоли приложения.');
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
    $('#add-place').click(() => { placeCreate(); });
    initGrid();
    initPaginator();
    restoreFilters(filteringSelector);
    loadData(1);
});
