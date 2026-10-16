#!/usr/bin/env node
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const root=path.resolve(__dirname,'../..');
const config=require('./release.config.json');
async function main(){
 const args=process.argv.slice(2); const preview=args.includes('--preview');
 const vi=args.indexOf('--version');const version=vi<0?'0.1.0':args[vi+1];
 if(!/^\d+\.\d+\.\d+$/.test(version||''))throw Error('Invalid version');
 const output=path.resolve(root,config.output);
 for(const file of ['game.js','game.json','project.config.json'])if(!fs.existsSync(path.join(output,file)))throw Error('尚未导出微信小游戏包：'+path.join(output,file));
 const projectConfig=JSON.parse(fs.readFileSync(path.join(output,'project.config.json')));
 if(projectConfig.appid!==config.appid||projectConfig.compileType!=='game')throw Error('AppID 或 compileType 不匹配，拒绝上传');
 const game=JSON.parse(fs.readFileSync(path.join(output,'game.json')));
 if(game.deviceOrientation!==config.orientation)throw Error('屏幕方向与发布配置不一致');
 if(!/loadDataPackageFromSubpackage:\s*true/.test(fs.readFileSync(path.join(output,'game.js'),'utf8')))throw Error('资源包未打入小游戏，拒绝上传缺少 CDN 配置的包');
 for(const dir of ['wasmcode','data-package']){
  const files=fs.readdirSync(path.join(output,dir));
  if(!files.some(name=>name.endsWith('.br')&&fs.statSync(path.join(output,dir,name)).size>0))throw Error('缺少构建内容：'+dir);
 }
 const local=fs.existsSync(path.join(__dirname,'.local.json'))?require('./.local.json'):{};
 const key=process.env.GOOSE_WECHAT_PRIVATE_KEY_PATH||local.privateKeyPath;
 if(!key||!fs.existsSync(key))throw Error('缺少代码上传私钥，设置 GOOSE_WECHAT_PRIVATE_KEY_PATH');
 const real=fs.realpathSync(key);if(real.startsWith(output+path.sep))throw Error('私钥不能放入构建目录');
 crypto.createPrivateKey(fs.readFileSync(real));
 if(args.includes('--check')){console.log('发布包、AppID 和上传私钥校验通过');return;}
 const ci=require('miniprogram-ci');
 const project=new ci.Project({appid:config.appid,type:'miniGame',projectPath:output,privateKeyPath:real,ignores:['**/*.key','**/*.pem','**/.env*','**/*.map','**/*.symbols.unityweb','**/*.symbols.unityweb.br','__context__/**/*','.plugincache/**/*','project.private.config.json','node_modules/**/*']});
 const options={project,version,desc:'抓鹅三消 '+version,robot:1,setting:{es6:true,es7:true,minify:true}};
 if(preview){await ci.preview({...options,qrcodeFormat:'image',qrcodeOutputDest:path.join(root,'Builds/Wechat/preview.png')});console.log('预览二维码已生成');}
 else {await ci.upload(options);console.log('开发版本上传成功，请在公众平台设为体验版；未提交审核。');}
}
main().catch(e=>{console.error(e.message);process.exitCode=1;});
