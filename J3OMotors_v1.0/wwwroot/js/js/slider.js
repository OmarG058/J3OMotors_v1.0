document.addEventListener("DOMContentLoaded", function () {
    const sliderInner = document.querySelector(".slider--inner");
    const prevBtn = document.querySelector(".slider-btn.prev");
    const nextBtn = document.querySelector(".slider-btn.next");
    const images = sliderInner.children.length; // Número total de imágenes
    let index = 0;
    let autoSlide;

    // Función para mover el slider
    function moveSlider(newIndex) {
        index = (newIndex + images) % images; // Asegura que el índice sea válido
        sliderInner.style.transform = `translateX(${-index * 100}%)`;
    }

    // Cambio automático
    function startAutoSlide() {
        autoSlide = setInterval(() => {
            moveSlider(index + 1);
        }, 4000);
    }

    // Detener el cambio automático
    function stopAutoSlide() {
        clearInterval(autoSlide);
    }

    // Botón "Siguiente"
    nextBtn.addEventListener("click", function () {
        stopAutoSlide();
        moveSlider(index + 1);
        startAutoSlide();
    });

    // Botón "Anterior"
    prevBtn.addEventListener("click", function () {
        stopAutoSlide();
        moveSlider(index - 1);
        startAutoSlide();
    });

    // Iniciar el cambio automático al cargar
    startAutoSlide();
});