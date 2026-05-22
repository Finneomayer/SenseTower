function initPage() {
    let $form = $('form');
    initField('#Password', $form);
    initField('#ConfirmPassword', $form);
}

function initField(selector, $form) {
    let $input = $form.find(selector);
    let $button = $input.next('button');
    $button.click((e) => {
        switchVisibility(e);
    });
}

function switchVisibility(e) {
    let $btn = $(e.currentTarget);
    let $input = $btn.parent().find('input');
    let $icon = $btn.children(':first');//.first();
    let isHidden = $icon.hasClass('fa-eye');
    if (isHidden) {
        $icon.removeClass('fa-eye').addClass('fa-eye-slash');
        switchMask($input);
    } else {
        $icon.removeClass('fa-eye-slash').addClass('fa-eye');
        switchMask($input);
    }
    $input.focus();
}

function switchMask($input) {
    if ($input[0].type === 'password') {
        $input[0].type = 'text';
    } else {
        $input[0].type = 'password';
    }
}

$(document).ready(() => {
    initPage();
});
