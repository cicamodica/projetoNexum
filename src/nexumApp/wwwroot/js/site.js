// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function previewImagemUnica(inputEl, targetImgId) {
    console.log('1. Preview iniciado para alvo: ' + targetImgId);

    if (inputEl.files && inputEl.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var imgPreview = document.getElementById(targetImgId);
            if (imgPreview) {
                console.log('2. Atualizando SRC da imagem...');
                imgPreview.src = e.target.result;
                imgPreview.style.display = 'block';
            } else {
                console.error('ERRO: Imagem com ID ' + targetImgId + ' não encontrada.');
            }
        };
        reader.readAsDataURL(inputEl.files[0]);
    }
}