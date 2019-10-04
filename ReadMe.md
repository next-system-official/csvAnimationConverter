# CsvAnimationConverter
***
これは[ミチコンPlus](https://apps.apple.com/jp/app/ミチコンplus-スマホで全身モーキャプ/id1468862870)で出力したアニメーションデータ(.csv)を読み込んで、任意のキャラクターをUnityで動かすためのスクリプトです。
-  Unityバーション 2019.1.7f1で開発を行っています。

# SampleScene
***
実際にキャラクターを動かせるようにしてあるサンプルのシーンです。

# HowToUse
***
1.Hierarchyに空のEmptyを作成します。  
2.作成したEmptyのInspectorにMotionPlayer.csを付けます。  

**youtubeの[弊社のチャンネル](https://www.youtube.com/channel/UCSFOvi0cNXufDusurFMncYw)で公開している解説動画は、以下のあたりからの解説になります。**

3.任意のキャラクターとcsvをAssetsにインポートします。  
4.インポートしたキャラクターをHierarchyに配置します。  
5.キャラクターのInspectorに１Animationを付けます。  
6.空のEmptyに付けたMotionPlayer.csのTargetとAnimationにキャラクターをアタッチします。  
7.Csvには出力したcsvをアタッチします。  
8.再生を押します。    
9.キーボードのLキーでアニメーションを読み込みます。  
10.キーボードのPキーでアニメーションを再生します。  
11.吐き出されたAnimationはAssets直下に自動的にフォルダが作成され、その中に格納されます。

# Other
***
- 株式会社ネクストシステム
  -  https://www.next-system.com
  
-  UniVRM
  - https://github.com/vrm-c/UniVRM/releases
  - https://github.com/vrm-c/UniVRM/wiki



