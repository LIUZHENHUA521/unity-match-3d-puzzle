"""Release safety checks; never builds or uploads real code."""
import importlib.util
import json
import subprocess
import tempfile
import unittest
from pathlib import Path
from types import SimpleNamespace
from unittest.mock import patch

spec = importlib.util.spec_from_file_location('publish', Path(__file__).with_name('publish.py'))
publish = importlib.util.module_from_spec(spec)
spec.loader.exec_module(publish)


class PublishTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.config = dict(appid='test-app', orientation='portrait', scene='Assets/test.unity', output='Builds/Wechat/minigame')
        (self.root / 'Assets').mkdir()
        (self.root / self.config['scene']).touch()
        config_dir = self.root / 'Tools/wechat'
        config_dir.mkdir(parents=True)
        (config_dir / 'release.config.json').write_text(json.dumps(self.config))
        self.output = self.root / 'Builds/WechatMenuProject' / self.config['output']
        self.output.mkdir(parents=True)
        (self.output / 'project.config.json').write_text(json.dumps(dict(appid='test-app', compileType='game')))
        (self.output / 'game.json').write_text(json.dumps(dict(deviceOrientation='portrait')))
        (self.output / 'game.js').write_text('loadDataPackageFromSubpackage: true')
        for name in ('wasmcode', 'data-package'):
            (self.output / name).mkdir()
            (self.output / name / 'test.br').write_bytes(b'content')
        self.cli = self.root / 'cli'
        self.cli.touch()
        self.args = SimpleNamespace(version='1.2.3', description='中文说明 "引号" $(literal)')

    def run_publish(self, action):
        with patch.object(publish, 'ROOT', self.root), patch.object(publish, 'CLI', self.cli), patch.object(publish.subprocess, 'run', side_effect=action) as run:
            publish.publish(self.args)
            return run

    def test_upload_uses_validated_isolated_snapshot_and_literal_description(self):
        def action(argv, **kwargs):
            if argv[0] == str(self.cli):
                snapshot = Path(argv[argv.index('--project') + 1])
                self.assertNotEqual(snapshot, self.output)
                self.assertEqual((snapshot / 'game.js').read_text(), 'loadDataPackageFromSubpackage: true')
                self.assertEqual(argv[argv.index('--desc') + 1], self.args.description)
                Path(argv[argv.index('--info-output') + 1]).write_text('{"size":{"total":12}}')
        self.assertEqual(self.run_publish(action).call_count, 2)

    def test_build_failure_never_uploads(self):
        calls = []
        def action(argv, **kwargs):
            calls.append(argv)
            raise subprocess.CalledProcessError(1, argv)
        with self.assertRaises(subprocess.CalledProcessError):
            self.run_publish(action)
        self.assertEqual(len(calls), 1)

    def test_wrong_appid_never_uploads(self):
        (self.output / 'project.config.json').write_text('{"appid":"wrong","compileType":"game"}')
        calls = []
        with self.assertRaises(ValueError):
            self.run_publish(lambda argv, **kwargs: calls.append(argv))
        self.assertEqual(len(calls), 1)

    def test_missing_receipt_is_failure(self):
        with self.assertRaises(FileNotFoundError):
            self.run_publish(lambda *args, **kwargs: None)

    def test_missing_resources_rejected(self):
        (self.output / 'data-package/test.br').unlink()
        with self.assertRaises(ValueError):
            publish.validate_package(self.output, self.config)


if __name__ == '__main__':
    unittest.main()
