# Changelog

## [1.3.2] - 2024-01-11

### Fixed
* Fixed `math.hash` crash when using IL2CPP builds on Arm 32 bit devices.
* Fixed obsolete method usage warnings for `MatrixDrawer.CanCacheInspectorGUI` and `PrimitiveVectorDrawer.CanCacheInspectorGUI` in UNITY_2023_2_OR_NEWER.
* Updated minimum editor version to 2021.3

## [1.3.1] - 2023-07-12

### Added
* Added `math.square` to compute the square (x * x).
* Added `math.orthonormal_basis` to compute an orthonormal basis from a single unit length vector.
* Added `math.sign` for int, int2, int3 and int4.
* Added `math.chgsign` for float, float2, float3, and float4.
* Added `math.Euler` to convert a quaternion to Euler angles.
* Added `math.angle` to compute the angle between two unit quaternions.
* Added `math.rotation` to extract a quaternion rotation from a float3x3 (that may have scale).
* Added `math.mulScale` to scale columns of a float3x3 with scaling coefficients in a float3.
* Added `math.scaleMul` to scale rows of a float3x3 with scaling coefficients in a float3.
* Added `AffineTransform` type.
* Added `PI2`, `PIHALF`, `TAU`, `TODEGREES` and `TORADIANS` constants.

### Changed
* `asfloat(uint)`, `asuint(float)`, `asint(float)` and other related methods are now faster in mono without Burst. Other methods which use these will see a performance improvement.
* Modified `quaternion.nlerp` to be branchless.
* More descriptive parameter names for many methods in `math` class.
* Made `Il2CppEagerStaticClassConstructionAttribute` internal to avoid conflicts with other definitions outside of the package.

## [1.2.6] - 2022-02-11

### Changed
* Made Il2CppEagerStaticClassConstructionAttribute internal to avoid conflicts with other definitions outside of the package. 

## [1.2.5] - 2021-11-01

### Fixed
* Fixed property drawing when manually drawing a property that was hidden with [HideInInspector].

## [1.2.4] - 2021-09-22

### Added
* Added `[Il2CppEagerStaticClassConstruction]` to Unity.Mathematics types to run static constructors at startup. This improves IL2CPP performance slightly for types that have static constructors.

### Changed
* License file updated to satisfy Unity's package validation tests.
* Changed noise documentation in comments to xmldoc comments.

### Fixed
* Fixed Equals(object) override which did not check type before casting. This could cause exceptions to be thrown when the object did not match the expected type.
* Fixed incorrect `math.tzcnt` documentation which mentioned leading zero counts instead of trailing zero counts.
* Fixed `float2x2.Rotate` documentation to mention radians instead of degrees.
* Fixed documentation for methods and properties that were previously undocumented.

## [1.2.1] - 2020-08-06

### Fixed
* Fixed warnings for meta files existing even though the files they represent did not exist.

### Internal (Not ready for production)

## [1.2.0] - 2020-08-03

### Added
* Added `[MethodImpl(MethodImplOptions.AggressiveInlining)]` to many static functions to improve IL2CPP performance.
* Added `compress()` that accepts a `float4` and `uint4`.
* Added `math.project()` and `math.projectsafe()` for vector projection.
* Added `math.EPSILON`, `math.INFINITY`, `math.NAN` and their double counterparts.
* Added `[Serializable]` to `RigidTransform`.
* Added `math.ceillog2()`.
* Added `math.floorlog2()`.
* Added `math.down()`, `math.forward()`, etc for Cartesian coordinate axes that match UnityEngine Vector3 equivalents.
* Added `math.ispow2()`.
* Added `half.MinValueAsHalf` and `half.MaxValueAsHalf` to avoid having to explicitly convert from float.
* Added a `float3x3` constructor which takes a `float4x4` as input.
* Added `[Serializable]` to half types.
* Added some performance tests which can be run from the Unity test project.
* Added `Random.CreateFromIndex()` to assist in creating Random instances from loop indices.

### Fixed
* Fixed documentation bug where `quaternion.RotateX/Y/Z` referred to a `float4x4` instead of quaternion.
* Fixed code generation bugs which could cause Windows and Mac to generate different test code.
* Fixed some test asserts which used NaNs and signed zeros which failed in IL2CPP builds.
* Updated documentation for `math.countbits()` to include equivalent names on Intel and ARM architectures to aid in discoverability.

### Internal (Not ready for production)
* Added `Unity.Mathematics.Geometry.Plane` to represent planes in 3D space.
* Added more `MinMaxAABB` functionality from `Unity.Physics.Aabb`.
* Added `Unity.Mathematics.Geometry.Math` to hold static functions like AABB transformations.
* Added `MinMaxAABB`.

## [1.1.0] - 2019-07-08

- Release stable version

## [1.1.0-preview.1] - 2019-06-27

- Add new math.bitmask to return a bit mask from a bool4

## [1.0.1] - 2019-04-15

- Release stable version
- Modify all math constants (e.g `math.PI`) to provide float constant by default instead of double. Use for example `math.PI_DBL` to get the previous double constant.

## [1.0.0-preview.1] - 2019-02-28

- Fixed bug where modifications on prefabs could not be reverted for vector properties when using context menu in Inspector.
- Fixed structure of the package for internal validation
