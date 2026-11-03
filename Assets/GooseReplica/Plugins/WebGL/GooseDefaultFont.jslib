mergeInto(LibraryManager.library, {
    GooseCanLoadSystemFont: function () {
        if (typeof wx === 'undefined' || typeof GameGlobal === 'undefined') return 0;
        try {
            // Match the installed SDK's !isIOS guard in unity-sdk/font/index.js.
            var platform = wx.getSystemInfoSync().platform;
            if (platform === 'ios') return 0;
            var font = GameGlobal.manager && GameGlobal.manager.font;
            return font && typeof font.getCommonFont === 'function' ? 1 : 0;
        } catch (error) { return 0; }
    }
});
