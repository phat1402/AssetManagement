$(document).ready(function () {
    var navbar = document.getElementById("assetNav");
    var items = navbar.getElementsByClassName("classic-menu-dropdown");
    for (var i = 0; i < items.length; i++) {
        items[i].addEventListener("click", function () {
            var current = document.getElementsByClassName("active");
            current[0].className = current[0].className.replace(" active", "");
            this.className += " active";
        });
    }
});