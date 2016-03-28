/* Create a variable to store the XMLHttpRequest object, but don't use it yet */
var xhrObject = false;

/* If this browser isn't IE 6, stores XMLHttpRequest() in our variable */
if (window.XMLHttpRequest) {
    xhrObject = new XMLHttpRequest();

/* But if this browser is IE 6, store ActiveXObject in our variable */
}else if (window.ActiveXObject){
    xhrObject = new ActiveXObject("Microsoft.XMLHTTP");
}


/* Create a function with 2 parameters: one for copy, one for a <div> to load it into */
function loadCopy(externalTextFile, externalTextDiv){
    /* if we can use AJAX */
    if(xhrObject){
        /* Create a new variable that traverses the DOM and finds the <div> tag we put into our second parameter */
        var pageText = document.getElementById(externalTextDiv);

        /* Use AJAX to place a text file we put in our first parameter */
        xhrObject.open("GET", externalTextFile);

        /* If our AJAX variable found our data and the server is ready to send it out, bundle our data as a text string */
        xhrObject.onreadystatechange = function () {
            if (xhrObject.readyState == 4 && xhrObject.status == 200) {
                pageText.innerHTML = xhrObject.responseText;
            }
        }

        /* Send our data out */
        xhrObject.send(null);
    }
}