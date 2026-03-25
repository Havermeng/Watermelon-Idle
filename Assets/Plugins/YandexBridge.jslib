mergeInto(LibraryManager.library, {
    GetYandexLang: function() {
        var lang = window.yandexLang || "ru";
        var bufferSize = lengthBytesUTF8(lang) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(lang, buffer, bufferSize);
        return buffer;
    }
});