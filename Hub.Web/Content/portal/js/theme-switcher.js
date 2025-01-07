// Função para alternar o tema
function toggleTheme() {
    const $htmlElement = $('html'); // Ou você pode usar $('body') se preferir
    const currentTheme = $htmlElement.attr('data-theme') === 'dark' ? 'light' : 'dark';

    // Atualiza o atributo data-theme
    $htmlElement.attr('data-theme', currentTheme);

    // Salva o tema no localStorage para persistir na próxima visita
    localStorage.setItem('theme', currentTheme);
}

// Função para carregar o tema salvo
function loadTheme() {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme) {
        $('html').attr('data-theme', savedTheme);
    }
}

// Evento de carregamento da página
$(document).ready(function () {
    loadTheme(); 
});

// Alternar tema ao clicar no botão
$('#btn-theme').on('click', function () {
    toggleTheme();
});
