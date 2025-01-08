var itr = 1;

// Defini��o da fun��o next
function next(id) {
    if (id == '') {
    } else {
        var text = $('#' + id).val();
        if (text.length < 3) {
            add_remove_effects($('#' + id).parent(), 'empty');
            return;
        }
    }
    $('#cube').css('transform', "translateZ(-25px) rotateX(" + (itr * 90) + "deg)");
    itr++;
}

// Fun��o para adicionar/remover efeitos (como classes)
var add_remove_effects = function (ref, classname) {
    ref.addClass(classname);
    setTimeout(function () {
        ref.removeClass(classname);
    }, 450);
}

// Adicionando os atalhos de Enter e Tab usando jQuery
$(document).ready(function () {
    // Focar o bot�o de login assim que a p�gina carregar
    $('#loginBtn').focus();

    // Ao pressionar Enter ou Tab, chama a fun��o next
    $('#username').on('keydown', function (event) {
        if (event.key === 'Enter' || event.key === 'Tab') {
            next('username');
        }
    });

    $('#password').on('keydown', function (event) {
        if (event.key === 'Enter' || event.key === 'Tab') {
            next('password');
        }
    });

    // Para o bot�o de login (front) tamb�m responder ao Enter/Tab
    $('#loginBtn').on('keydown', function (event) {
        if (event.key === 'Enter' || event.key === 'Tab') {
            next('');
        }
    });
});