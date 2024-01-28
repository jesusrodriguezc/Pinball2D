/*!
 * modernizr v3.6.0
 * Build https://modernizr.com/download?-backgroundcliptext-backgroundsize-blobconstructor-canvas-canvastext-cors-cssanimations-csscolumns-cssgradients-cssgrid_cssgridlegacy-cssmask-csstransforms-csstransforms3d-dataview-exiforientation-fileinput-filereader-flexbox-flexboxlegacy-formvalidation-fullscreen-history-input-localstorage-sessionstorage-srcset-todataurljpeg_todataurlpng_todataurlwebp-touchevents-video-domprefixes-prefixed-prefixes-setclasses-shiv-testallprops-testprop-teststyles-dontmin
 *
 * Copyright (c)
 *  Faruk Ates
 *  Paul Irish
 *  Alex Sexton
 *  Ryan Seddon
 *  Patrick Kettner
 *  Stu Cox
 *  Richard Herrera

 * MIT License
 */
(function(e,t,n){var r=[];var a=[];var i={_version:"3.6.0",_config:{classPrefix:"",enableClasses:true,enableJSClass:true,usePrefixes:true},_q:[],on:function(e,t){var n=this;setTimeout((function(){t(n[e])}),0)},addTest:function(e,t,n){a.push({name:e,fn:t,options:n})},addAsyncTest:function(e){a.push({name:null,fn:e})}};var o=function(){};o.prototype=i;o=new o;
/*!
{
  "name": "Blob constructor",
  "property": "blobconstructor",
  "aliases": ["blob-constructor"],
  "builderAliases": ["blob_constructor"],
  "caniuse": "blobbuilder",
  "notes": [{
    "name": "W3C spec",
    "href": "https://w3c.github.io/FileAPI/#constructorBlob"
  }],
  "polyfills": ["blobjs"]
}
!*/o.addTest("blobconstructor",(function(){try{return!!new Blob}catch(e){return false}}),{aliases:["blob-constructor"]});
/*!
{
  "name": "Cross-Origin Resource Sharing",
  "property": "cors",
  "caniuse": "cors",
  "authors": ["Theodoor van Donge"],
  "notes": [{
    "name": "MDN documentation",
    "href": "https://developer.mozilla.org/en-US/docs/HTTP/Access_control_CORS"
  }],
  "polyfills": ["pmxdr", "ppx", "flxhr"]
}
!*/o.addTest("cors","XMLHttpRequest"in e&&"withCredentials"in new XMLHttpRequest);
/*!
{
  "name": "DataView",
  "property": "dataview",
  "authors": ["Addy Osmani"],
  "builderAliases": ["dataview_api"],
  "notes": [{
    "name": "MDN documentation",
    "href": "https://developer.mozilla.org/en/JavaScript_typed_arrays/DataView"
  }],
  "polyfills": ["jdataview"]
}
!*/o.addTest("dataview",typeof DataView!=="undefined"&&"getFloat64"in DataView.prototype);
/*!
{
  "name": "History API",
  "property": "history",
  "caniuse": "history",
  "tags": ["history"],
  "authors": ["Hay Kranen", "Alexander Farkas"],
  "notes": [{
    "name": "W3C Spec",
    "href": "https://www.w3.org/TR/html51/browsers.html#the-history-interface"
  }, {
    "name": "MDN documentation",
    "href": "https://developer.mozilla.org/en-US/docs/Web/API/window.history"
  }],
  "polyfills": ["historyjs", "html5historyapi"]
}
!*/o.addTest("history",(function(){var t=navigator.userAgent;if((t.indexOf("Android 2.")!==-1||t.indexOf("Android 4.0")!==-1)&&t.indexOf("Mobile Safari")!==-1&&t.indexOf("Chrome")===-1&&t.indexOf("Windows Phone")===-1&&location.protocol!=="file:"){return false}return e.history&&"pushState"in e.history}));
/*!
{
  "name": "File API",
  "property": "filereader",
  "caniuse": "fileapi",
  "notes": [{
    "name": "W3C Working Draft",
    "href": "https://www.w3.org/TR/FileAPI/"
  }],
  "tags": ["file"],
  "builderAliases": ["file_api"],
  "knownBugs": ["Will fail in Safari 5 due to its lack of support for the standards defined FileReader object"]
}
!*/o.addTest("filereader",!!(e.File&&e.FileList&&e.FileReader));
/*!
{
  "name": "Local Storage",
  "property": "localstorage",
  "caniuse": "namevalue-storage",
  "tags": ["storage"],
  "knownBugs": [],
  "notes": [],
  "warnings": [],
  "polyfills": [
    "joshuabell-polyfill",
    "cupcake",
    "storagepolyfill",
    "amplifyjs",
    "yui-cacheoffline"
  ]
}
!*/o.addTest("localstorage",(function(){var e="modernizr";try{localStorage.setItem(e,e);localStorage.removeItem(e);return true}catch(e){return false}}));
/*!
{
  "name": "Session Storage",
  "property": "sessionstorage",
  "tags": ["storage"],
  "polyfills": ["joshuabell-polyfill", "cupcake", "sessionstorage"]
}
!*/o.addTest("sessionstorage",(function(){var e="modernizr";try{sessionStorage.setItem(e,e);sessionStorage.removeItem(e);return true}catch(e){return false}}));var s=i._config.usePrefixes?" -webkit- -moz- -o- -ms- ".split(" "):["",""];i._prefixes=s;function l(e,t){return typeof e===t}function f(){var e;var t;var n;var i;var s;var f;var u;for(var c in a){if(a.hasOwnProperty(c)){e=[];t=a[c];if(t.name){e.push(t.name.toLowerCase());if(t.options&&t.options.aliases&&t.options.aliases.length){for(n=0;n<t.options.aliases.length;n++){e.push(t.options.aliases[n].toLowerCase())}}}i=l(t.fn,"function")?t.fn():t.fn;for(s=0;s<e.length;s++){f=e[s];u=f.split(".");if(u.length===1){o[u[0]]=i}else{if(o[u[0]]&&!(o[u[0]]instanceof Boolean)){o[u[0]]=new Boolean(o[u[0]])}o[u[0]][u[1]]=i}r.push((i?"":"no-")+u.join("-"))}}}}var u=t.documentElement;var c=u.nodeName.toLowerCase()==="svg";function d(e){var t=u.className;var n=o._config.classPrefix||"";if(c){t=t.baseVal}if(o._config.enableJSClass){var r=new RegExp("(^|\\s)"+n+"no-js(\\s|$)");t=t.replace(r,"$1"+n+"js$2")}if(o._config.enableClasses){t+=" "+n+e.join(" "+n);if(c){u.className.baseVal=t}else{u.className=t}}}var p;if(!c){(function(e,t){var n="3.7.3";var r=e.html5||{};var a=/^<|^(?:button|map|select|textarea|object|iframe|option|optgroup)$/i;var i=/^(?:a|b|code|div|fieldset|h1|h2|h3|h4|h5|h6|i|label|li|ol|p|q|span|strong|style|table|tbody|td|th|tr|ul)$/i;var o;var s="_html5shiv";var l=0;var f={};var u;(function(){try{var e=t.createElement("a");e.innerHTML="<xyz></xyz>";o="hidden"in e;u=e.childNodes.length==1||function(){t.createElement("a");var e=t.createDocumentFragment();return typeof e.cloneNode=="undefined"||typeof e.createDocumentFragment=="undefined"||typeof e.createElement=="undefined"}()}catch(e){o=true;u=true}})();function c(e,t){var n=e.createElement("p"),r=e.getElementsByTagName("head")[0]||e.documentElement;n.innerHTML="x<style>"+t+"</style>";return r.insertBefore(n.lastChild,r.firstChild)}function d(){var e=E.elements;return typeof e=="string"?e.split(" "):e}function p(e,t){var n=E.elements;if(typeof n!="string"){n=n.join(" ")}if(typeof e!="string"){e=e.join(" ")}E.elements=n+" "+e;h(t)}function v(e){var t=f[e[s]];if(!t){t={};l++;e[s]=l;f[l]=t}return t}function m(e,n,r){if(!n){n=t}if(u){return n.createElement(e)}if(!r){r=v(n)}var o;if(r.cache[e]){o=r.cache[e].cloneNode()}else if(i.test(e)){o=(r.cache[e]=r.createElem(e)).cloneNode()}else{o=r.createElem(e)}return o.canHaveChildren&&!a.test(e)&&!o.tagUrn?r.frag.appendChild(o):o}function A(e,n){if(!e){e=t}if(u){return e.createDocumentFragment()}n=n||v(e);var r=n.frag.cloneNode(),a=0,i=d(),o=i.length;for(;a<o;a++){r.createElement(i[a])}return r}function g(e,t){if(!t.cache){t.cache={};t.createElem=e.createElement;t.createFrag=e.createDocumentFragment;t.frag=t.createFrag()}e.createElement=function(n){if(!E.shivMethods){return t.createElem(n)}return m(n,e,t)};e.createDocumentFragment=Function("h,f","return function(){"+"var n=f.cloneNode(),c=n.createElement;"+"h.shivMethods&&("+d().join().replace(/[\w\-:]+/g,(function(e){t.createElem(e);t.frag.createElement(e);return'c("'+e+'")'}))+");return n}")(E,t.frag)}function h(e){if(!e){e=t}var n=v(e);if(E.shivCSS&&!o&&!n.hasCSS){n.hasCSS=!!c(e,"article,aside,dialog,figcaption,figure,footer,header,hgroup,main,nav,section{display:block}"+"mark{background:#FF0;color:#000}"+"template{display:none}")}if(!u){g(e,n)}return e}var E={elements:r.elements||"abbr article aside audio bdi canvas data datalist details dialog figcaption figure footer header hgroup main mark meter nav output picture progress section summary template time video",version:n,shivCSS:r.shivCSS!==false,supportsUnknownElements:u,shivMethods:r.shivMethods!==false,type:"default",shivDocument:h,createElement:m,createDocumentFragment:A,addElements:p};e.html5=E;h(t);if(typeof module=="object"&&module.exports){module.exports=E}})(typeof e!=="undefined"?e:this,t)}var v="Moz O ms Webkit";var m=i._config.usePrefixes?v.toLowerCase().split(" "):[];i._domPrefixes=m;function A(e){return e.replace(/([a-z])-([a-z])/g,(function(e,t,n){return t+n.toUpperCase()})).replace(/^-/,"")}function g(){if(typeof t.createElement!=="function"){return t.createElement(arguments[0])}else if(c){return t.createElementNS.call(t,"http://www.w3.org/2000/svg",arguments[0])}else{return t.createElement.apply(t,arguments)}}
/*!
{
  "name": "Canvas",
  "property": "canvas",
  "caniuse": "canvas",
  "tags": ["canvas", "graphics"],
  "polyfills": ["flashcanvas", "excanvas", "slcanvas", "fxcanvas"]
}
!*/
o.addTest("canvas",(function(){var e=g("canvas");return!!(e.getContext&&e.getContext("2d"))}));
/*!
{
  "name": "Canvas text",
  "property": "canvastext",
  "caniuse": "canvas-text",
  "tags": ["canvas", "graphics"],
  "polyfills": ["canvastext"]
}
!*/o.addTest("canvastext",(function(){if(o.canvas===false){return false}return typeof g("canvas").getContext("2d").fillText=="function"}));
/*!
{
  "name": "HTML5 Video",
  "property": "video",
  "caniuse": "video",
  "tags": ["html5"],
  "knownBugs": [
    "Without QuickTime, `Modernizr.video.h264` will be `undefined`; https://github.com/Modernizr/Modernizr/issues/546"
  ],
  "polyfills": [
    "html5media",
    "mediaelementjs",
    "sublimevideo",
    "videojs",
    "leanbackplayer",
    "videoforeverybody"
  ]
}
!*/o.addTest("video",(function(){var e=g("video");var t=false;try{t=!!e.canPlayType;if(t){t=new Boolean(t);t.ogg=e.canPlayType('video/ogg; codecs="theora"').replace(/^no$/,"");t.h264=e.canPlayType('video/mp4; codecs="avc1.42E01E"').replace(/^no$/,"");t.webm=e.canPlayType('video/webm; codecs="vp8, vorbis"').replace(/^no$/,"");t.vp9=e.canPlayType('video/webm; codecs="vp9"').replace(/^no$/,"");t.hls=e.canPlayType('application/x-mpegURL; codecs="avc1.42E01E"').replace(/^no$/,"")}}catch(e){}return t}));
/*!
{
  "name": "canvas.toDataURL type support",
  "property": ["todataurljpeg", "todataurlpng", "todataurlwebp"],
  "tags": ["canvas"],
  "builderAliases": ["canvas_todataurl_type"],
  "async" : false,
  "notes": [{
    "name": "MDN article",
    "href": "https://developer.mozilla.org/en-US/docs/Web/API/HTMLCanvasElement.toDataURL"
  }]
}
!*/var h=g("canvas");o.addTest("todataurljpeg",(function(){return!!o.canvas&&h.toDataURL("image/jpeg").indexOf("data:image/jpeg")===0}));o.addTest("todataurlpng",(function(){return!!o.canvas&&h.toDataURL("image/png").indexOf("data:image/png")===0}));o.addTest("todataurlwebp",(function(){var e=false;try{e=!!o.canvas&&h.toDataURL("image/webp").indexOf("data:image/webp")===0}catch(e){}return e}));
/*!
{
  "name": "CSS Gradients",
  "caniuse": "css-gradients",
  "property": "cssgradients",
  "tags": ["css"],
  "knownBugs": ["False-positives on webOS (https://github.com/Modernizr/Modernizr/issues/202)"],
  "notes": [{
    "name": "Webkit Gradient Syntax",
    "href": "https://webkit.org/blog/175/introducing-css-gradients/"
  },{
    "name": "Linear Gradient Syntax",
    "href": "https://developer.mozilla.org/en-US/docs/Web/CSS/linear-gradient"
  },{
    "name": "W3C Gradient Spec",
    "href": "https://drafts.csswg.org/css-images-3/#gradients"
  }]
}
!*/o.addTest("cssgradients",(function(){var e="background-image:";var t="gradient(linear,left top,right bottom,from(#9f9),to(white));";var n="";var r;for(var a=0,i=s.length-1;a<i;a++){r=a===0?"to ":"";n+=e+s[a]+"linear-gradient("+r+"left top, #9f9, white);"}if(o._config.usePrefixes){n+=e+"-webkit-"+t}var l=g("a");var f=l.style;f.cssText=n;return(""+f.backgroundImage).indexOf("gradient")>-1}));
/*!
{
  "name": "input[file] Attribute",
  "property": "fileinput",
  "caniuse" : "forms",
  "tags": ["file", "forms", "input"],
  "builderAliases": ["forms_fileinput"]
}
!*/o.addTest("fileinput",(function(){if(navigator.userAgent.match(/(Android (1.0|1.1|1.5|1.6|2.0|2.1))|(Windows Phone (OS 7|8.0))|(XBLWP)|(ZuneWP)|(w(eb)?OSBrowser)|(webOS)|(Kindle\/(1.0|2.0|2.5|3.0))/)){return false}var e=g("input");e.type="file";return!e.disabled}));
/*!
{
  "name": "srcset attribute",
  "property": "srcset",
  "tags": ["image"],
  "notes": [{
    "name": "Smashing Magazine Article",
    "href": "https://en.wikipedia.org/wiki/APNG"
    },{
    "name": "Generate multi-resolution images for srcset with Grunt",
    "href": "https://addyosmani.com/blog/generate-multi-resolution-images-for-srcset-with-grunt/"
    }]
}
!*/o.addTest("srcset","srcset"in g("img"));var E=g("input");
/*!
{
  "name": "Input attributes",
  "property": "input",
  "tags": ["forms"],
  "authors": ["Mike Taylor"],
  "notes": [{
    "name": "WHATWG spec",
    "href": "https://html.spec.whatwg.org/multipage/forms.html#input-type-attr-summary"
  }],
  "knownBugs": ["Some blackberry devices report false positive for input.multiple"]
}
!*/var y="autocomplete autofocus list placeholder max min multiple pattern required step".split(" ");var B={};o.input=function(t){for(var n=0,r=t.length;n<r;n++){B[t[n]]=!!(t[n]in E)}if(B.list){B.list=!!(g("datalist")&&e.HTMLDataListElement)}return B}(y);
/*!
{
  "name": "CSS Supports",
  "property": "supports",
  "caniuse": "css-featurequeries",
  "tags": ["css"],
  "builderAliases": ["css_supports"],
  "notes": [{
    "name": "W3 Spec",
    "href": "http://dev.w3.org/csswg/css3-conditional/#at-supports"
  },{
    "name": "Related Github Issue",
    "href": "https://github.com/Modernizr/Modernizr/issues/648"
  },{
    "name": "W3 Info",
    "href": "http://dev.w3.org/csswg/css3-conditional/#the-csssupportsrule-interface"
  }]
}
!*/var b="CSS"in e&&"supports"in e.CSS;var x="supportsCSS"in e;o.addTest("supports",b||x);var w=i._config.usePrefixes?v.split(" "):[];i._cssomPrefixes=w;var Q=function(t){var r=s.length;var a=e.CSSRule;var i;if(typeof a==="undefined"){return n}if(!t){return false}t=t.replace(/^@/,"");i=t.replace(/-/g,"_").toUpperCase()+"_RULE";if(i in a){return"@"+t}for(var o=0;o<r;o++){var l=s[o];var f=l.toUpperCase()+"_"+i;if(f in a){return"@-"+l.toLowerCase()+"-"+t}}return false};i.atRule=Q;function T(e,t){return!!~(""+e).indexOf(t)}function C(){var e=t.body;if(!e){e=g(c?"svg":"body");e.fake=true}return e}function S(e,n,r,a){var i="modernizr";var o;var s;var l;var f;var c=g("div");var d=C();if(parseInt(r,10)){while(r--){l=g("div");l.id=a?a[r]:i+(r+1);c.appendChild(l)}}o=g("style");o.type="text/css";o.id="s"+i;(!d.fake?c:d).appendChild(o);d.appendChild(c);if(o.styleSheet){o.styleSheet.cssText=e}else{o.appendChild(t.createTextNode(e))}c.id=i;if(d.fake){d.style.background="";d.style.overflow="hidden";f=u.style.overflow;u.style.overflow="hidden";u.appendChild(d)}s=n(c,e);if(d.fake){d.parentNode.removeChild(d);u.style.overflow=f;u.offsetHeight}else{c.parentNode.removeChild(c)}return!!s}var k=i.testStyles=S;
/*!
{
  "name": "Touch Events",
  "property": "touchevents",
  "caniuse" : "touch",
  "tags": ["media", "attribute"],
  "notes": [{
    "name": "Touch Events spec",
    "href": "https://www.w3.org/TR/2013/WD-touch-events-20130124/"
  }],
  "warnings": [
    "Indicates if the browser supports the Touch Events spec, and does not necessarily reflect a touchscreen device"
  ],
  "knownBugs": [
    "False-positive on some configurations of Nokia N900",
    "False-positive on some BlackBerry 6.0 builds – https://github.com/Modernizr/Modernizr/issues/372#issuecomment-3112695"
  ]
}
!*/o.addTest("touchevents",(function(){var n;if("ontouchstart"in e||e.DocumentTouch&&t instanceof DocumentTouch){n=true}else{var r=["@media (",s.join("touch-enabled),("),"heartz",")","{#modernizr{top:9px;position:absolute}}"].join("");k(r,(function(e){n=e.offsetTop===9}))}return n}));
/*!
{
  "name": "Form Validation",
  "property": "formvalidation",
  "tags": ["forms", "validation", "attribute"],
  "builderAliases": ["forms_validation"]
}
!*/o.addTest("formvalidation",(function(){var t=g("form");if(!("checkValidity"in t)||!("addEventListener"in t)){return false}if("reportValidity"in t){return true}var n=false;var r;o.formvalidationapi=true;t.addEventListener("submit",(function(t){if(!e.opera||e.operamini){t.preventDefault()}t.stopPropagation()}),false);t.innerHTML='<input name="modTest" required="required" /><button></button>';k("#modernizr form{position:absolute;top:-99999em}",(function(e){e.appendChild(t);r=t.getElementsByTagName("input")[0];r.addEventListener("invalid",(function(e){n=true;e.preventDefault();e.stopPropagation()}),false);o.formvalidationmessage=!!r.validationMessage;t.getElementsByTagName("button")[0].click()}));return n}));var R;(function(){var e={}.hasOwnProperty;if(!l(e,"undefined")&&!l(e.call,"undefined")){R=function(t,n){return e.call(t,n)}}else{R=function(e,t){return t in e&&l(e.constructor.prototype[t],"undefined")}}})();i._l={};i.on=function(e,t){if(!this._l[e]){this._l[e]=[]}this._l[e].push(t);if(o.hasOwnProperty(e)){setTimeout((function(){o._trigger(e,o[e])}),0)}};i._trigger=function(e,t){if(!this._l[e]){return}var n=this._l[e];setTimeout((function(){var e,r;for(e=0;e<n.length;e++){r=n[e];r(t)}}),0);delete this._l[e]};function P(e,t){if(typeof e=="object"){for(var n in e){if(R(e,n)){P(n,e[n])}}}else{e=e.toLowerCase();var r=e.split(".");var a=o[r[0]];if(r.length==2){a=a[r[1]]}if(typeof a!="undefined"){return o}t=typeof t=="function"?t():t;if(r.length==1){o[r[0]]=t}else{if(o[r[0]]&&!(o[r[0]]instanceof Boolean)){o[r[0]]=new Boolean(o[r[0]])}o[r[0]][r[1]]=t}d([(!!t&&t!=false?"":"no-")+r.join("-")]);o._trigger(e,t)}return o}o._q.push((function(){i.addTest=P}));
/*!
{
  "name": "EXIF Orientation",
  "property": "exiforientation",
  "tags": ["image"],
  "builderAliases": ["exif_orientation"],
  "async": true,
  "authors": ["Paul Sayre"],
  "notes": [{
    "name": "Article by Dave Perrett",
    "href": "http://recursive-design.com/blog/2012/07/28/exif-orientation-handling-is-a-ghetto/"
  },{
    "name": "Article by Calvin Hass",
    "href": "http://www.impulseadventure.com/photo/exif-orientation.html"
  }]
}
!*/o.addAsyncTest((function(){var e=new Image;e.onerror=function(){P("exiforientation",false,{aliases:["exif-orientation"]})};e.onload=function(){P("exiforientation",e.width!==2,{aliases:["exif-orientation"]})};e.src="data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAYABgAAD/4QAiRXhpZgAASUkqAAgAAAABABIBAwABAAAABgASAAAAAAD/2wBDAAEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQH/2wBDAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQH/wAARCAABAAIDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD+/iiiigD/2Q=="}));function D(e,t){return function(){return e.apply(t,arguments)}}function L(e,t,n){var r;for(var a in e){if(e[a]in t){if(n===false){return e[a]}r=t[e[a]];if(l(r,"function")){return D(r,n||t)}return r}}return false}var _={elem:g("modernizr")};o._q.push((function(){delete _.elem}));var F={style:_.elem.style};o._q.unshift((function(){delete F.style}));function O(e){return e.replace(/([A-Z])/g,(function(e,t){return"-"+t.toLowerCase()})).replace(/^ms-/,"-ms-")}function j(t,n,r){var a;if("getComputedStyle"in e){a=getComputedStyle.call(e,t,n);var i=e.console;if(a!==null){if(r){a=a.getPropertyValue(r)}}else{if(i){var o=i.error?"error":"log";i[o].call(i,"getComputedStyle returning null, its possible modernizr test results are inaccurate")}}}else{a=!n&&t.currentStyle&&t.currentStyle[r]}return a}function N(t,r){var a=t.length;if("CSS"in e&&"supports"in e.CSS){while(a--){if(e.CSS.supports(O(t[a]),r)){return true}}return false}else if("CSSSupportsRule"in e){var i=[];while(a--){i.push("("+O(t[a])+":"+r+")")}i=i.join(" or ");return S("@supports ("+i+") { #modernizr { position: absolute; } }",(function(e){return j(e,null,"position")=="absolute"}))}return n}function I(e,t,r,a){a=l(a,"undefined")?false:a;if(!l(r,"undefined")){var i=N(e,r);if(!l(i,"undefined")){return i}}var o,s,f,u,c;var d=["modernizr","tspan","samp"];while(!F.style&&d.length){o=true;F.modElem=g(d.shift());F.style=F.modElem.style}function p(){if(o){delete F.style;delete F.modElem}}f=e.length;for(s=0;s<f;s++){u=e[s];c=F.style[u];if(T(u,"-")){u=A(u)}if(F.style[u]!==n){if(!a&&!l(r,"undefined")){try{F.style[u]=r}catch(e){}if(F.style[u]!=c){p();return t=="pfx"?u:true}}else{p();return t=="pfx"?u:true}}}p();return false}var U=i.testProp=function(e,t,r){return I([e],n,t,r)};function z(e,t,n,r,a){var i=e.charAt(0).toUpperCase()+e.slice(1),o=(e+" "+w.join(i+" ")+i).split(" ");if(l(t,"string")||l(t,"undefined")){return I(o,t,r,a)}else{o=(e+" "+m.join(i+" ")+i).split(" ");return L(o,t,n)}}i.testAllProps=z;var M=i.prefixed=function(e,t,n){if(e.indexOf("@")===0){return Q(e)}if(e.indexOf("-")!=-1){e=A(e)}if(!t){return z(e,"pfx")}else{return z(e,t,n)}};
/*!
{
  "name": "Fullscreen API",
  "property": "fullscreen",
  "caniuse": "fullscreen",
  "notes": [{
    "name": "MDN documentation",
    "href": "https://developer.mozilla.org/en/API/Fullscreen"
  }],
  "polyfills": ["screenfulljs"],
  "builderAliases": ["fullscreen_api"]
}
!*/o.addTest("fullscreen",!!(M("exitFullscreen",t,false)||M("cancelFullScreen",t,false)));function q(e,t,r){return z(e,n,n,t,r)}i.testAllProps=q;
/*!
{
  "name": "CSS Animations",
  "property": "cssanimations",
  "caniuse": "css-animation",
  "polyfills": ["transformie", "csssandpaper"],
  "tags": ["css"],
  "warnings": ["Android < 4 will pass this test, but can only animate a single property at a time"],
  "notes": [{
    "name" : "Article: 'Dispelling the Android CSS animation myths'",
    "href": "https://goo.gl/OGw5Gm"
  }]
}
!*/o.addTest("cssanimations",q("animationName","a",true));
/*!
{
  "name": "Background Size",
  "property": "backgroundsize",
  "tags": ["css"],
  "knownBugs": ["This will false positive in Opera Mini - https://github.com/Modernizr/Modernizr/issues/396"],
  "notes": [{
    "name": "Related Issue",
    "href": "https://github.com/Modernizr/Modernizr/issues/396"
  }]
}
!*/o.addTest("backgroundsize",q("backgroundSize","100%",true));
/*!
{
  "name": "CSS Columns",
  "property": "csscolumns",
  "caniuse": "multicolumn",
  "polyfills": ["css3multicolumnjs"],
  "tags": ["css"]
}
!*/(function(){o.addTest("csscolumns",(function(){var e=false;var t=q("columnCount");try{e=!!t;if(e){e=new Boolean(e)}}catch(e){}return e}));var e=["Width","Span","Fill","Gap","Rule","RuleColor","RuleStyle","RuleWidth","BreakBefore","BreakAfter","BreakInside"];var t,n;for(var r=0;r<e.length;r++){t=e[r].toLowerCase();n=q("column"+e[r]);if(t==="breakbefore"||t==="breakafter"||t=="breakinside"){n=n||q(e[r])}o.addTest("csscolumns."+t,n)}})();
/*!
{
  "name": "CSS Grid (old & new)",
  "property": ["cssgrid", "cssgridlegacy"],
  "authors": ["Faruk Ates"],
  "tags": ["css"],
  "notes": [{
    "name": "The new, standardized CSS Grid",
    "href": "https://www.w3.org/TR/css3-grid-layout/"
  }, {
    "name": "The _old_ CSS Grid (legacy)",
    "href": "https://www.w3.org/TR/2011/WD-css3-grid-layout-20110407/"
  }]
}
!*/o.addTest("cssgridlegacy",q("grid-columns","10px",true));o.addTest("cssgrid",q("grid-template-rows","none",true));
/*!
{
  "name": "Flexbox",
  "property": "flexbox",
  "caniuse": "flexbox",
  "tags": ["css"],
  "notes": [{
    "name": "The _new_ flexbox",
    "href": "http://dev.w3.org/csswg/css3-flexbox"
  }],
  "warnings": [
    "A `true` result for this detect does not imply that the `flex-wrap` property is supported; see the `flexwrap` detect."
  ]
}
!*/o.addTest("flexbox",q("flexBasis","1px",true));
/*!
{
  "name": "Flexbox (legacy)",
  "property": "flexboxlegacy",
  "tags": ["css"],
  "polyfills": ["flexie"],
  "notes": [{
    "name": "The _old_ flexbox",
    "href": "https://www.w3.org/TR/2009/WD-css3-flexbox-20090723/"
  }]
}
!*/o.addTest("flexboxlegacy",q("boxDirection","reverse",true));
/*!
{
  "name": "CSS Mask",
  "caniuse": "css-masks",
  "property": "cssmask",
  "tags": ["css"],
  "builderAliases": ["css_mask"],
  "notes": [
    {
      "name": "Webkit blog on CSS Masks",
      "href": "https://webkit.org/blog/181/css-masks/"
    },
    {
      "name": "Safari Docs",
      "href": "https://developer.apple.com/library/safari/#documentation/InternetWeb/Conceptual/SafariVisualEffectsProgGuide/Masks/Masks.html"
    },
    {
      "name": "CSS SVG mask",
      "href": "https://developer.mozilla.org/en-US/docs/Web/CSS/mask"
    },
    {
      "name": "Combine with clippaths for awesomeness",
      "href": "https://generic.cx/for/webkit/test.html"
    }
  ]
}
!*/o.addTest("cssmask",q("maskRepeat","repeat-x",true));
/*!
{
  "name": "CSS Transforms",
  "property": "csstransforms",
  "caniuse": "transforms2d",
  "tags": ["css"]
}
!*/o.addTest("csstransforms",(function(){return navigator.userAgent.indexOf("Android 2.")===-1&&q("transform","scale(1)",true)}));
/*!
{
  "name": "CSS Transforms 3D",
  "property": "csstransforms3d",
  "caniuse": "transforms3d",
  "tags": ["css"],
  "warnings": [
    "Chrome may occassionally fail this test on some systems; more info: https://code.google.com/p/chromium/issues/detail?id=129004"
  ]
}
!*/o.addTest("csstransforms3d",(function(){return!!q("perspective","1px",true)}));
/*!
{
  "name": "CSS Background Clip Text",
  "property": "backgroundcliptext",
  "authors": ["ausi"],
  "tags": ["css"],
  "notes": [
    {
      "name": "CSS Tricks Article",
      "href": "https://css-tricks.com/image-under-text/"
    },
    {
      "name": "MDN Docs",
      "href": "https://developer.mozilla.org/en-US/docs/Web/CSS/background-clip"
    },
    {
      "name": "Related Github Issue",
      "href": "https://github.com/Modernizr/Modernizr/issues/199"
    }
  ]
}
!*/o.addTest("backgroundcliptext",(function(){return q("backgroundClip","text")}));f();d(r);delete i.addTest;delete i.addAsyncTest;for(var H=0;H<o._q.length;H++){o._q[H]()}e.Modernizr=o})(window,document);