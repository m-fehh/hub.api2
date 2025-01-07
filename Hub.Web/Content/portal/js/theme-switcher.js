// Fun��o para alternar o tema
function toggleTheme() {
    const $htmlElement = $('html'); // Ou voc� pode usar $('body') se preferir
    const currentTheme = $htmlElement.attr('data-theme') === 'dark' ? 'light' : 'dark';

    // Atualiza o atributo data-theme
    $htmlElement.attr('data-theme', currentTheme);

    // Salva o tema no localStorage para persistir na pr�xima visita
    localStorage.setItem('theme', currentTheme);
}

// Fun��o para carregar o tema salvo
function loadTheme() {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme) {
        $('html').attr('data-theme', savedTheme);
    }
}

// Evento de carregamento da p�gina
$(document).ready(function () {
    loadTheme(); 
});

// Alternar tema ao clicar no bot�o
$('#btn-theme').on('click', function () {
    toggleTheme();
});
