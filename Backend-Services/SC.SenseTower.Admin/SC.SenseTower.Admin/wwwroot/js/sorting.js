function initSorting(selector, sortChanged) {
    $(selector).find('[sorting]').each((i, v) => {
        $(v).click((e) => {
            e.preventDefault();
            e.stopPropagation();
            switchSorting(selector, $(e.target), e.ctrlKey, sortChanged);
        });
        adjustSortingPriority(selector);
        redrawSorting(selector);
    });
}

function switchSorting(selector, $el, ctrlKey, sortChanged) {
    if (ctrlKey) {
        clearSelection();
        if ($el.hasClass('sorted')) {
            if ($el.hasClass('sorted-up')) {
                $el.removeClass('sorted-up').addClass('sorted-down');
            } else {
                $el.removeClass('sorted-down').removeClass('sorted').data('sortPriority', '');
                adjustSortingPriority(selector);
                redrawSorting(selector);
            }
        } else {
            let $element = $(selector);
            $element.find('.sorted').filter((i, v) => {
                let $v = $(v);
                return ($v.hasClass('sorted-up') || $v.hasClass('sorted-down')) && !$v.data('sortPriority');
            }).data('sortPriority', 1);
            let priority = 0;
            $element.find('.sorted').filter((i, v) => {
                let $v = $(v);
                return $v.hasClass('sorted-up') || $v.hasClass('sorted-down');
            }).each((i, v) => {
                let n = parseInt($(v).data('sortPriority'));
                if (n > priority) {
                    priority = n;
                }
            });
            $el.addClass('sorted').addClass('sorted-up').data('sortPriority', ++priority);
            adjustSortingPriority(selector);
            redrawSorting(selector);
        }
    } else {
        let sorted = $el.hasClass('sorted');
        let sortedUp = $el.hasClass('sorted-up');
        $(selector).find('.sorted').removeClass('sorted-up').removeClass('sorted-down').removeClass('sorted').data('sortPriority', '');
        if (sorted) {
            if (sortedUp) {
                $el.addClass('sorted').addClass('sorted-down');
            }
        } else {
            $el.addClass('sorted').addClass('sorted-up');
        }
        redrawSorting(selector);
    }
    if (typeof sortChanged === 'function') {
        sortChanged();
    }
}

function adjustSortingPriority(selector) {
    let $els = $(selector).find('.sorted');
    if ($els.length === 1) {
        $(selector).find('[sorting]').data('sortPriority', '');
    } else {
        $els.sort((a, b) => {
            let x = $(a).data('sortPriority');
            let y = $(b).data('sortPriority');
            return x - y;
        }).each((i, el) => $(el).data('sortPriority', i + 1));
    }
}

function redrawSorting(selector) {
    $(selector)
        .find('[sorting]')
        .each((i, v) => {
            let $v = $(v);
            let $ind = $v.find('.sort-priority');
            if (!$ind || $ind.length === 0) {
                $ind = $('<span>').addClass('sort-priority');
                $ind.appendTo($v);
            }
            let priority = $v.data('sortPriority');
            if (priority) {
                $ind.html($v.data('sortPriority'));
            } else {
                $ind.empty();
            }
        });
}

function sortingCriteria(selector) {
    let criteria = $(selector)
        .find('[sorting]')
        .filter((i, v) => $(v).hasClass('sorted'))
        .map((i, v) => {
            let $v = $(v);
            let result = {
                propertyName: $v.data('sortBy'),
                ascending: $v.hasClass('sorted-up'),
                sortOrder: parseInt('0' + $v.data('sortPriority'))
            };
            return result;
        })
        .get();
    return criteria;
}
