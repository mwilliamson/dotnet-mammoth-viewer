(function() {
    var templateFileChooser = document.getElementById("template-file-chooser").innerHTML;
    Array.prototype.forEach.call(document.querySelectorAll(".file-chooser"), initialiseFileChooser);
    function initialiseFileChooser(element) {
        var fragment = document.createElement("div");
        fragment.innerHTML = templateFileChooser;
        while (fragment.firstChild) {
            element.appendChild(fragment.firstChild);
        }
        element.querySelector(".action").addEventListener("click", function() {
            element.querySelector(".file-path").value = MammothViewer.openFile();
        });
    }
})();