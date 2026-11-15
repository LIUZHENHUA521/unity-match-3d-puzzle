# Match3D 棋盘命名

CountrysideStove.prefab 改名为 Match3DBoard.prefab，保留 meta GUID；根节点、运行时实例、编辑器资源路径和托盘默认配置方法同步更新。编辑器“乡村”标签改为 Match3D。

环境键 CountrysideStove 保留兼容，通过 Match3DThemePresentation.SceneAssetName 映射到新预制体，既有配置、回放与业务数据不迁移。历史来源记录和旧资源导出路径保持原样。命名调整不改变资源来源。

用户已删除旧场景导出资源，因此发布不再要求 SourceScenes 目录。碰撞预处理改为遍历实际使用的当前主题，第一关同样由这些主题派生，不再处理已废弃的原版第一关模型列表。
