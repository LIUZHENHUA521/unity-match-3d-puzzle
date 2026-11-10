mergeInto(LibraryManager.library, {
  GooseDailyPreviewEvent: function (json) {
    if (window.goosePreviewEvent) window.goosePreviewEvent(JSON.parse(UTF8ToString(json)));
  }
});
