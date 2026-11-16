function checkUpdate() {
    if (typeof wx === 'undefined' || typeof wx.getUpdateManager !== 'function'
        || typeof wx.showModal !== 'function') return;
    const updateManager = wx.getUpdateManager();
    let modalOpen = false;
    let applying = false;
    updateManager.onUpdateReady(() => {
        if (modalOpen || applying) return;
        modalOpen = true;
        wx.showModal({
            title: '更新提示',
            content: '一锅消不完有新版本啦！已下载完成，是否立即重启更新？',
            confirmText: '立即更新',
            cancelText: '稍后',
            success(res) {
                if (res.confirm) {
                    applying = true;
                    updateManager.applyUpdate();
                }
            },
            complete() { modalOpen = false; },
        });
    });
    updateManager.onUpdateFailed(() => {
        if (modalOpen || applying) return;
        modalOpen = true;
        wx.showModal({
            title: '更新失败',
            content: '新版本下载失败，请检查网络后重新打开游戏再试。',
            showCancel: false,
            confirmText: '知道了',
            complete() { modalOpen = false; },
        });
    });
}
checkUpdate();
