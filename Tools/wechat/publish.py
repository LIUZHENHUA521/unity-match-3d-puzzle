#!/usr/bin/env python3
"""Unity menu worker: isolated build, validation, then DevTools upload."""
import argparse
import fcntl
import json
import re
import shutil
import subprocess
import sys
from datetime import datetime
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
CLI = Path('/Applications/wechatwebdevtools.app/Contents/MacOS/cli')


def validate_package(output, config):
    project = json.loads((output / 'project.config.json').read_text())
    if project.get('appid') != config['appid'] or project.get('compileType') != 'game':
        raise ValueError('AppID 或小游戏类型不匹配，停止上传')
    game = json.loads((output / 'game.json').read_text())
    if game.get('deviceOrientation') != config['orientation']:
        raise ValueError('屏幕方向与发布配置不一致')
    if not re.search(r'loadDataPackageFromSubpackage:\s*true', (output / 'game.js').read_text()):
        raise ValueError('资源未打入分包，停止上传')
    for directory in ('wasmcode', 'data-package'):
        if not any(p.stat().st_size for p in (output / directory).glob('*.br')):
            raise ValueError('缺少构建内容：' + directory)


def publish(args):
    if not re.fullmatch(r'\d+\.\d+\.\d+', args.version):
        raise ValueError('版本号格式应为 0.1.12')
    if not CLI.is_file():
        raise ValueError('请安装微信开发者工具到 /Applications/wechatwebdevtools.app')
    config = json.loads((ROOT / 'Tools/wechat/release.config.json').read_text())
    if not (ROOT / config['scene']).is_file():
        raise ValueError('发布场景不存在：' + config['scene'])
    print('发布 AppID：' + config['appid'], flush=True)
    print('发布场景：' + config['scene'], flush=True)
    print('1/3 正在构建独立副本；详细进度见 Builds/wechat-build.log', flush=True)
    # A dedicated stage avoids consuming another build's shared output.
    stage = ROOT / 'Builds/WechatMenuProject'
    subprocess.run([sys.executable, str(ROOT / 'Tools/wechat/build.py'), '--stage', str(stage)],
                   cwd=ROOT, check=True, timeout=3600)
    output = stage / config['output']
    print('2/3 校验发布包', flush=True)
    validate_package(output, config)
    stamp = datetime.now().strftime('%Y%m%d-%H%M%S-%f')
    snapshot = ROOT / 'Builds/Wechat' / ('release-' + args.version + '-' + stamp)
    shutil.copytree(output, snapshot)
    info = snapshot.parent / (snapshot.name + '-upload.json')
    print('3/3 正在上传，请确保开发者工具已登录并开启服务端口', flush=True)
    subprocess.run([str(CLI), 'upload', '--project', str(snapshot), '--version', args.version,
                    '--desc', args.description, '--info-output', str(info)], cwd=ROOT, check=True, timeout=600)
    # A zero exit alone is insufficient; require DevTools' upload receipt too.
    receipt = json.loads(info.read_text())
    if receipt.get('size', {}).get('total', 0) <= 0:
        raise ValueError('未取得有效上传回执，请查看开发者工具；不标记为成功')
    print('开发版本上传成功：' + args.version, flush=True)
    print('请到微信公众平台 → 版本管理 → 设为体验版。未提交审核。', flush=True)
    print('上传回执：' + str(info), flush=True)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--version', required=True)
    parser.add_argument('--description', default='Unity 小游戏更新')
    args = parser.parse_args()
    folder = ROOT / 'Builds/Wechat'
    folder.mkdir(parents=True, exist_ok=True)
    with (folder / 'menu-publish.lock').open('w') as lock:
        try:
            fcntl.flock(lock, fcntl.LOCK_EX | fcntl.LOCK_NB)
        except BlockingIOError:
            return 1
        with (folder / 'menu-publish.log').open('w', buffering=1) as log:
            # Child processes inherit these descriptors, so all diagnostics land here.
            import os
            os.dup2(log.fileno(), 1)
            os.dup2(log.fileno(), 2)
            try:
                publish(args)
            except Exception as error:
                print('发布失败：' + str(error), flush=True)
                return 1
    return 0


if __name__ == '__main__':
    sys.exit(main())
