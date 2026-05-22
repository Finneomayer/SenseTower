$(document).ready(() => {
    $('#set-password form').submit((e) => {
        e.preventDefault();
        let $form = $('#set-password form');
        let data = $form.serialize();
        axios.post('setpassword', data)
            .then((response) => {
                $form.html(response.data);
                window.onload();
            })
            .catch((error) => {
                toastr.error('Ошибка смены пароля. Подробности в консоли приложения.');
                console.log(error);
            });
    });
});
