# UnityFx.Async changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/); this project adheres to [Semantic Versioning](http://semver.org/).

-----------------------
## [0.9.6] - unreleased

### Added
- Added `Play`/`Wait` extension methods for `Animation` and `Animator`.
- Added `AsyncResult.IsStarted` helper property.

### Changed
- Changed `AsyncResult` constructors argument order to avoid ambiguity in some cases.
- Moved the package content to Plugins folder and remove assembly definition file.
- Moved web request related helpers from `AsyncUtility` to `AsyncWww` class.
- Changed `AsyncUtility.SendToMainThread`, `AsyncUtility.PostToMainThread` and `AsyncUtility.InvokeOnMainThread` implementation to use `ConcurrentQueue` for net46+ to avoid unnesesary locks.
- Changed interface of `AsyncResultQueue`.

### Removed
- Removed `IAsyncOperationEvents.TryAddCompletionCallback` and `IAsyncOperationEvents.TryAddProgressCallback` methods. These methods are not needed in 99% of cases and may lead to logic errors in multi-threaded environment.

-----------------------
## [0.9.5] - 2018.07.31

### Added
- Added `IAsyncOperation.Id` property for easy operation identification.
- Added `AsyncUtility.GetText` and `AsyncUtility.GetBytes` helpers.
- Added `ToTask` extensions for `AsyncOperation`/`UnityWebRequest`/`WWW`.
- `AsyncResult` now implements `AsyncContinuation` to enable easy operation chaining.

### Fixed
- Fixed compile warnings for Unity 2018.2.

### Removed
- Removed `ToAsyncXxx` extension methods for `WWW` and `UnityWebRequest`.
- Removed `MovieTexture` related methods for Unity 2018.2 (the class is deprecated now).
- Removed `IAsyncSchedulable` interface (it was just another form of `IAsyncContinuation`).

-----------------------
## [0.9.4] - 2018.07.10

### Added
- Added assembly definition file for Unity 2017.3+.
- Added `AsyncResult.FaultedOperation` helper.
- Added `AsyncUtility.IsMainThread` method.
- Added `AsyncUtility.GetAssetBundle`, `AsyncUtility.GetTexture`, `AsyncUtility.AudioClip` and `AsyncUtility.GetMovieTexture` helper methods.
- Added `AssetBundleCreateRequest` wrapper operation.
- Added `ThrowIfNonSuccess` extension for `IAsyncOperation`.
- Added `ToEnum` extension for `IAsyncResult` that converts an asynchronous operation (`Task`, `AsyncResult` etc) to enumerator.
- Added `IAsyncSchedulable` interface - an abstraction of a schedulable entity.
- Added `AsyncLazy` helper for initialization operations.

### Fixed
- Fixed compile warnings for some older Unity versions.
- Fixed error handling for Unity operation wrappers.

### Removed
- Removed `AggregateException` class for net35.

-----------------------
## [0.9.3] - 2018.06.09

### Added
- Added push-based progress reporting support.
- Added `AsyncResult.Delay(float)` overloads.
- Added `AsyncCreationOptions.SuppressCancellation` option.
- Added update sources for `LateUpdate`, `FixedUpdate` and end-of-frame updates.
- Added `SynchronizationContext` for the main thread (if not set by Unity).
- Added methods `AsyncUtility.PostToMainThread`, `AsyncUtility.SendToMainThread` as `AsyncUtility.InvokeOnMainThread`.
- Added new `FromAction` overloads.

### Changed
- Significantly reduced number of memory allocations when adding continuations.
- Changed signature of the `IAsyncContinuation.Invoke` method.
- Changed `AsyncResult.OnCancel` implementation to do nothing (previously it threw `NotSupportedException`).

### Fixed
- Fixed exception when removing listeners while in `AsyncUpdateSource.OnError` / `AsyncUpdateSource.OnCompleted` / `AsyncUpdateSource.Dispose`.
- Fixed `AsyncResult.MoveNext` to always return `true` while the operation is not completed.
- Fixed `AsyncResult` construction code not working as intended when `AsyncCreationOptions` are specified.

### Removed
- Removed `AsyncOperationCallback` delegate type.

-----------------------
## [0.9.2] - 2018.05.25

### Added
- Added pull-based progress reporting support.
- Added new methods to `IAsyncUpdateSource`.
- Added `AsyncUpdateSource` class as default `IAsyncUpdateSource` implementation.
- `IAsyncOperation<T>` now inherits `IObservable<T>`.

### Changed
- Changed `IAsyncOperation.Exception` type to `Exception`.
- Changed `IAsyncOperationEvents.Completed` type to `AsyncCompletedEventHandler`.

### Removed
- Removed `ToObservable` extension of `IAsyncOperation`.

-----------------------
## [0.9.1] - 2018.05.07

### Fixed
- Fixed Unity3d script compile errors when targeting .NET 4.x ([Issue #1](https://github.com/Arvtesh/UnityFx.Async/issues/1)).

-----------------------
## [0.9.0] - 2018.04.24

### Added
- Added `AsyncContinuationOptions`.
- Added `AsyncCreationOptions`.
- Added `Promise`-like extensions `Then`, `ThenAll`, `ThenAny`, `Rebind`, `Catch`, `Finally` and `Done`.
- Added `Unwrap` extension methods.
- Added `FromTask`/`FromObservable` helpers.
- Added `FromAction` helpers.
- Added `ToAsync` extension method for `IObservable` interface.
- Added `TryAddContinuation`/`RemoveContinuation` methods to `IAsyncOperationEvents` for non-delegate continuations.
- Added `IAsyncUpdatable` and `IAsyncUpdateSource` interfaces.
- Added `Delay`/`Retry` overload that uses `IAsyncUpdateSource`-based service for time management.
- Added cancellation support (`IAsyncCancellable` interface, `WithCancellation` extension method and many implementation changes).
- Added `Wait`/`Join` overloads with `CancellationToken` argument.

### Changed
- Changed `ContinueWith` extension signatures to match corresponding `Task` methods.
- Changed `IAsyncOperation.Exception` to always return an exception instance if completed with non-success.
- Changed `AddCompletionCallback`/`AddContinuation` to instance methods (instead of extensions).

### Fixed
- Fixed exception not initialized properly for canceled operations sometimes.

### Removed
- Removed `GetAwaiter`/`ConfigureAwait` instance methods from `AsyncResult` to avoid code duplication (extension methods should be used).
- Removed all `AsyncCompletionSource` methods having `completedSynchronously` argument.

-----------------------
## [0.8.2] - 2018-03-28

### Added
- New `AsyncResult` constructors added.
- New overloads for `AsyncResult.FromResult`, `AsyncResult.FromCanceled` and `AsyncResult.FromException` methods added.

### Changed
- Made optimizations to `ToTask` extensions implementation for cases when target operation is completed.
- Marked all assembly classes CLS-compilant.

### Fixed
- `ToTask` extensions now throw inner exception instead of the `AggregateException` in case of an error.
- Fixed `AsyncResult.Delay` throwing exception when infinite delay (-1) specified.

-----------------------
## [0.8.1] - 2018-03-19

### Added
- Added `AsyncResultQueue.Empty` event.
- Added `Unity`-specific tools for Asset Store.

-----------------------
## [0.8.0] - 2018-03-10

### Added
- Added `IAsyncCompletionSource.Operation` property to match `TaskCompletionSource` interface.
- Added new constructor to `AsyncResult`.
- Added `AsyncResult.Start` method to match `Task` interface.
- Added `AsyncResult.OnStarted` virtual method.
- Added `WhenAll`/`WhenAny` static helpers for `AsyncResult`.
- Added `ConfigureAwait` extensions for `IAsyncOperation`.
- Added `Task` extension methods to that convert it to an `AsyncResult` instance.
- Added `AsyncResult.Retry` methods.
- Added `Wait` overloads to match `Task` interface.

### Removed
- Removed `AsyncResult.TryCreateAsyncWaitHandle` helper.

### Changed
- Modified `AsyncResultAwaiter` implementation to throw if the operation was canceled or faulted (to match `TaskAwaiter` behaviour).
- Implemented `AsyncCompletionSource` as a sealed analog of `TaskCompletionSource`.
- Removed public completion methods from `AsyncResult` (moved them to `AsyncCompletionSource`).
- Made `SpinUntilCompleted` an extension method (was `AsyncResult` instance method).
- Changed `IAsyncOperation.Exception` type to `AggregateException` to match `Task`.
- Changed `IAsyncOperationEvents.Completed` event signature & behaviour to always execute handler (event if it was registered after the comperation has copleted).
- Removed `AsyncResult` constructors that accepted exceptions.
- Changed `AsyncResult.Result` property to throw `AggregateException` when faulted or canceled to mathch `Task` behaviour.

### Fixed
- `AsyncResultQueue` now does not remove uncompleted operations from the queue.
- `AsyncResult.Exception` now only returns non-null value when the operation is faulted.

-----------------------
## [0.7.1] - 2018-02-14

### Added
- Added possibility to store multiple exceptions in `AsyncResult`.
- Added `AggregateException` class for `net35` target.
- Added `IsEmpty` property to `AsyncResultQueue`.

### Changed
- `AsyncResult` implemenation is changed to prevent returning null operation result when the operation is completed in some cases.

-----------------------
## [0.7.0] - 2018-02-10

### Added
- Added project documentation site (`docs` folder).
- Added unit tests project.
- Added continuations support.
- Added [AppVeyor CI](https://ci.appveyor.com/project/Arvtesh/unityfx-async) support.
- Added [NuGet](https://www.nuget.org/packages/UnityFx.Async/) deployment.
- Added [GitVersion](https://gitversion.readthedocs.io/en/latest/) support.

### Changed
- Renamed `AsyncOperationStatus` values to match [TastStatus](https://msdn.microsoft.com/ru-ru/library/system.threading.tasks.taskstatus(v=vs.110).aspx).

-----------------------
## [0.3.1] - 2017-08-01

### Added
- Initial release.

