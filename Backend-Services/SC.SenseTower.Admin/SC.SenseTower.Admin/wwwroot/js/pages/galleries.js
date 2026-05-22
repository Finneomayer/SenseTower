const sortingSelector = '#galleries-grid thead';
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
    let $block = blockElement('#galleries-grid');
    axios.post('getpage', command)
        .then((response) => {
            $('#galleries-grid').html(response.data);
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
    let isVisible = $panel.find('[name="isVisible"]:checked').val();
    return {
        name: $panel.find('#Name').val(),
        isVisible: isVisible !== 'null' ? isVisible === 'true' : null
    };
}

function filteringClear(selector) {
    let $panel = $(selector);
    $panel.find('input:text').val('');
    $panel.find('#isVisible1').prop('checked', true);
}

function saveFilters(selector) {
    let filters = JSON.stringify(filteringCriteria(selector));
    localStorage.setItem("galleries-filters", filters);
}

function restoreFilters(selector) {
    let filtersJson = localStorage.getItem("galleries-filters");
    if (!filtersJson) return;

    let filters = JSON.parse(filtersJson);
    let $panel = $(selector);
    $panel.find('[name="isVisible"]:checked').removeAttr('checked');
    $panel.find(`[name="isVisible"][value="${filters.isVisible}"]`).prop('checked', true);
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
    $('#galleries-grid tbody').find('tr[data-id]').each((i, v) => {
        let $v = $(v);
        $v.click((e) => {
            e.stopPropagation();
            showItem($(e.currentTarget));
        });
        $v.find('a[data-action="delete"]').click((e) => {
            e.stopPropagation();
            galleryDelete($v.data('id'));
        });
        $v.find('a[data-action="image-list"]').click((e) => {
            e.stopPropagation();
            imageList($(e.currentTarget), $v.data('id'));
        });
        $v.find('a[data-action="image-add"]').click((e) => {
            e.stopPropagation();
            imageAdd($(e.currentTarget), $v.data('id'));
        });
    });
}

function showItem($tr) {
    let $block = blockElement($tr);
    let id = $tr.data('id');
    axios.get(`get/${id}`)
        .then((response) => {
            let $dialog = $(response.data);
            $dialog.appendTo('#galleries-grid');
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
            toastr.error('Не получены данные галереи. Подробности в консоли приложения.');
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
            toastr.success('Галерея успешно сохранена.');
            loadData();
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка сохранения галереи. Подробности в консоли приложения.');
        });
}

function galleryCreate() {
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

function galleryDelete(id) {
    if (!confirm("Удалить выбранную галерею?"))
        return;
    axios.post(`delete/${id}`)
        .then((response) => {
            toastr.success('Галерея успешно удалена.');
            loadData();
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка удаления галереи. Подробности в консоли приложения.');
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
                let places = $('.gallery-image-place');
                let images = [];
                for (let i = 0; i < places.length; i++) {
                    let $imageBlock = $(places[i]).find('.gallery-image-list-item');
                    if ($imageBlock.length == 1) {
                        let image = {
                            imageId: $imageBlock.find('#ImageId').val(),
                            position: i,
                            name: $imageBlock.find('#Name').val(),
                            author: $imageBlock.find('#Author').val(),
                            description: $imageBlock.find('#Description').val(),
                            passepartoutWidthInMeters: $imageBlock.find('#PassepartoutWidthInMeters').val(),
                            pictureWidthInMeters: $imageBlock.find('#PictureWidthInMeters').val()
                        };
                        images.push(image);
                    }
                };
                let data = new FormData();
                images.forEach((v, i) => {
                    data.append(`images[${i}][author]`, v.author);
                    data.append(`images[${i}][description]`, v.description);
                    data.append(`images[${i}][imageId]`, v.imageId);
                    data.append(`images[${i}][name]`, v.name);
                    data.append(`images[${i}][position]`, v.position);
                    data.append(`images[${i}][passepartoutWidthInMeters]`, v.passepartoutWidthInMeters);
                    data.append(`images[${i}][pictureWidthInMeters]`, v.pictureWidthInMeters);
                })
                axios.post(`images/${id}`, data)
                    .then(() => {
                        $dialog.modal('hide');
                        toastr.success('Галерея успешно сохранена.');
                    })
                    .catch((error) => {
                        console.log(error);
                        toastr.error('Ошибка сохранения галереи. Подробности в консоли приложения.');
                    });
            });
            $('.gallery-image-list-item button[name="remove-image"]').click((e) => {
                if (!confirm('Удалить изображение из галереи?')) {
                    return;
                }
                let $blockRemoveImage = blockElement($('.gallery-image-list'));
                let command = new FormData();
                command.append('position', $(e.currentTarget).data('position'));
                command.append('galleryId', $('.gallery-image-list').data('id'));
                axios.post('removeimage', command)
                    .then(() => {
                        $(e.target).closest('.gallery-image-list-item').remove();
                        loadData();
                    })
                    .catch((error) => {
                        console.log(error);
                        toastr.error('Ошибка удаления изображения из галереи. Подробности в консоли приложения.');
                    })
                    .then(() => {
                        unblockElement($blockRemoveImage);
                    });
            });
            $('.gallery-image-list-item').draggable({
                axis: 'y',
                containment: '.gallery-image-list',
                start: (e, ui) => {
                    $(e.target).find('.mover').css('cursor', 'grabbing');
                },
                stop: (e, ui) => {
                    $(e.target).find('.mover').css('cursor', 'grab');
                }
            });
            $('.gallery-image-place').droppable({
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
        })
        .catch((error) => {
            console.log(error);
            toastr.error('Ошибка получения списка изображений галереи. Подробности в консоли приложения.');
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
    $('#add-gallery').click(() => { galleryCreate(); });
    initGrid();
    initPaginator();
    restoreFilters(filteringSelector);
    loadData(1);
});
