mergeInto(LibraryManager.library, {
  GooseDiagnosticValue: function(field) {
    if (!GameGlobal.gooseInputProbe) {
      var state = GameGlobal.gooseInputProbe = { starts: 0, ends: 0 };
      wx.onTouchStart(function() { state.starts++; });
      wx.onTouchEnd(function() { state.ends++; });
    }
    if (field === 0) return GameGlobal.gooseInputProbe.starts;
    if (field === 1) return GameGlobal.gooseInputProbe.ends;
    if (field === 2) return GameGlobal.isIOSHighPerformanceModePlus ? 2 : (GameGlobal.isIOSHighPerformanceMode ? 1 : 0);
    return -1;
  }
});
