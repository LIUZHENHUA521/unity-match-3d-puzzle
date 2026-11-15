mergeInto(LibraryManager.library, {
    $PotSameGamesNavigation: { busy: false, revision: 0 },
    PotSameGamesOpen__deps: ['$PotSameGamesNavigation'],
    PotSameGamesOpen: function (appIdPointer) {
        function notice(message) {
            if (typeof wx !== 'undefined' && typeof wx.showToast === 'function')
                wx.showToast({ title: message, icon: 'none' });
        }
        if (typeof wx === 'undefined' || typeof wx.navigateToMiniProgram !== 'function') {
            notice('请在微信小游戏中打开疯狂动物消');
            return;
        }
        if (PotSameGamesNavigation.busy) return;
        PotSameGamesNavigation.busy = true;
        var revision = ++PotSameGamesNavigation.revision;
        var timer;
        function reset() {
            if (revision !== PotSameGamesNavigation.revision) return;
            PotSameGamesNavigation.busy = false;
            clearTimeout(timer);
        }
        timer = setTimeout(reset, 10000);
        try {
            wx.navigateToMiniProgram({
                appId: UTF8ToString(appIdPointer),
                envVersion: 'release',
                extraData: { source: 'pot', placement: 'same_games' },
                fail: function (error) {
                    if (revision !== PotSameGamesNavigation.revision) return;
                    reset();
                    if (!/cancel/i.test(error && error.errMsg || ''))
                        notice('暂时无法打开，请稍后再试');
                },
                complete: reset
            });
        } catch (error) {
            reset();
            notice('暂时无法打开，请稍后再试');
        }
    }
});
