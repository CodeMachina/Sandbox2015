"use strict";

var appScript = document.createElement("script");
appScript.src = "webpack_app.js?v=" + Date.now();
document.head.appendChild(appScript);