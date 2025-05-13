let prevScrollpos = window.pageYOffset; // Captura la posición inicial del scroll
const headern = document.querySelector(".headern"); // Selecciona tu header

window.addEventListener("scroll", () => {
    const currentScrollPos = window.pageYOffset; // Posición actual del scroll

    if (prevScrollpos > currentScrollPos) {
        // Scroll hacia arriba
        headern.style.top = "0"; // Muestra el menú
    } else {
        // Scroll hacia abajo
        headern.style.top = `-${headern.offsetHeight}px`; // Oculta el menú (altura dinámica)
    }

    prevScrollpos = currentScrollPos; // Actualiza la posición anterior
});