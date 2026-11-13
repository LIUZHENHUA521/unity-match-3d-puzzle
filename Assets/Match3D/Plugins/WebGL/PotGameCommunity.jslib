mergeInto(LibraryManager.library, {
    $GooseGameCommunity: { busy: false, manager: null, revision: 0 },
    GooseGameCommunityOpen__deps: ['$GooseGameCommunity'],
    GooseGameCommunityOpen: function (linkPointer) {
        function notice(message) {
            if (typeof wx !== 'undefined' && typeof wx.showToast === 'function') wx.showToast({ title: message, icon: 'none' });
            else if (typeof window !== 'undefined' && typeof window.alert === 'function') window.alert(message);
        }
        if (typeof wx === 'undefined') { notice('请在微信小游戏中打开游戏圈'); return; }
        if (typeof wx.createPageManager !== 'function') { notice('请升级微信后打开游戏圈'); return; }
        var openlink = UTF8ToString(linkPointer).trim();
        if (!openlink) { notice('游戏圈暂未配置，请稍后再试'); return; }
        if (GooseGameCommunity.busy) return;
        GooseGameCommunity.busy = true;
        var revision = ++GooseGameCommunity.revision;
        var manager = null;
        function cleanup() {
            GooseGameCommunity.busy = false;
            if (GooseGameCommunity.manager === manager) GooseGameCommunity.manager = null;
            if (manager) { try { manager.destroy(); } catch (e) {} manager = null; }
        }
        function failed(error) {
            if (revision !== GooseGameCommunity.revision) return;
            console.warn('[GameCommunity] open failed', error);
            cleanup();
            notice('游戏圈打开失败，请稍后重试');
        }
        try {
            manager = GooseGameCommunity.manager;
            if (!manager) {
                manager = wx.createPageManager();
                GooseGameCommunity.manager = manager;
                manager.on('destroy', function () {
                    if (GooseGameCommunity.manager === manager) {
                        GooseGameCommunity.manager = null;
                        GooseGameCommunity.revision++;
                        GooseGameCommunity.busy = false;
                    }
                    manager = null;
                });
            }
            Promise.resolve(manager.load({ openlink: openlink })).then(function () {
                if (revision !== GooseGameCommunity.revision || !manager || GooseGameCommunity.manager !== manager) return;
                return manager.show();
            }).then(function () {
                // Keep the page alive until WeChat emits destroy (the user closes it).
                if (revision === GooseGameCommunity.revision) GooseGameCommunity.busy = false;
            }).catch(failed);
        } catch (error) { failed(error); }
    }
});
