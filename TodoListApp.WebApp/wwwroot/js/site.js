// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const hamburger = document.querySelector("#toggle-btn");
const sidebar = document.getElementById('sidebar');
const searchIcon = document.querySelector('.sidebar-link ion-icon');

hamburger.addEventListener("click", function () {
    document.querySelector("#sidebar").classList.toggle("expand");
});

searchIcon.addEventListener('click', function (event) {
    if (!sidebar.classList.contains('expand')) {
        sidebar.classList.add('expand');
    }

    event.preventDefault();
});


 