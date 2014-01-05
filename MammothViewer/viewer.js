(function() {
    function main() {
        initialiseFileChoosers();
    }

    var templateFileChooser = document.getElementById("template-file-chooser").innerHTML;

    function initialiseFileChoosers() {
        Array.prototype.forEach.call(
            document.querySelectorAll(".file-chooser"),
            initialiseFileChooser
        );
    }

    function initialiseFileChooser(element) {
        appendHtml(element, templateFileChooser);
        element.querySelector(".action").addEventListener("click", function() {
            element.querySelector(".file-path").value = MammothViewer.openFile();
        });
    }

    function appendHtml(element, html) {
        var fragment = document.createElement("div");
        fragment.innerHTML = html;
        while (fragment.firstChild) {
            element.appendChild(fragment.firstChild);
        }
    }

    main();
})();