# Kongregate Unity Example Game

This repository contains the example app to exercise the Kongregate Mobile SDK.

## General Setup

* Install Unity. Ths SDK is currently build using [Unity 5.4.1](https://unity3d.com/get-unity/download/archive)
* Supported targates include: `iOS`, `Android`, `WebGL`, `OSX` and `Windows`.

## Topic Branches

* `Release` - This branch is updated by CI for tagged releases. If UCB where enabled, this triggers the Test Apps to automitically build.
* `Unity-2017-x1` - This branch contains a version of the project converted for Unity 2017. It's useful for testing the plugin using modern versions of Unity.
* `Gradle` - This branch contains a version of the project that supports the Gradle build pipeline.

## Build Dependencies

The way the [SDK Build Script](https://github.com/kongregate/mobile/tree/master/SDK) works, files are built and copied from the parent repository [UnitySDKWrapper](https://github.com/kongregate/mobile/tree/master/UnitySDKWrapper) into this test project. Scripts in the plugins [KongregateExport](https://github.com/kongregate/mobile/blob/master/UnitySDKWrapper/Assets/Editor/KongregateExport.cs) are executed to package the SDK. 

#### TODO

This is a sort of awkward cross-dependency. It may be useful to refactor the plugin so it lives in this repository and only depends on the Mobile SDK, similar to how the [AIR SDK Wrapper](https://github.com/kongregate/air-sdk-wrapper) is setup.