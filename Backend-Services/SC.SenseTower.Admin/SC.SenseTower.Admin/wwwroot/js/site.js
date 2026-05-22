function clearSelection() {
    if (document.selection && document.selection.empty) {
        document.selection.empty();
    } else if (window.getSelection) {
        var sel = window.getSelection();
        sel.removeAllRanges();
    }
}

function blockElement(selector) {
    let $wrapper = $('<div style="position:absolute;top:0;bottom:0;left:0;right:0;z-index=2000"></div>');
    let $div = $('<div class="bg-secondary opacity-2" style="position:absolute;top:0;bottom:0;left:0;right:0;z-index=2001"></div>');
    let $spinner = $('<div class="d-flex justify-content-center align-items-center" style="position:absolute;top:0;bottom:0;left:0;right:0;z-index=2002"><div class="spinner-border text-info" role="status"><span class="visually-hidden">Loading...</span></div></div > ');
    $div.appendTo($wrapper);
    $spinner.appendTo($wrapper);
    $wrapper.appendTo(selector);
    return $wrapper;
}

function unblockElement($spinner) {
    $spinner.remove();
}

function getCurrentPage() {
    return $('ul.pagination li.active a').data('page');
}

function getPageSize() {
    return $('ul.pagination').closest('ul.navbar-nav').find('select').val();
}

function hideFilters(selector) {
    $(selector).closest('.fixed-plugin').removeClass('show');
}
