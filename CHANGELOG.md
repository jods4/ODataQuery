# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased

## 2.0.0 - 2024-06-19
### Breaking change
- Lowest .net target is 6.0
### Added
- Support DateOnly model properties

## 1.4.0 - 2023-08-15
### Fixed
- Parsing of decimal separator was dependent on culture.

## 1.3.0 - 2023-08-14
### Added
- `ODataFilter` and `ODataOrderBy` don't throw anymore on null strings (they return the source unchanged).

## 1.2.0 - 2023-04-04
### Fixed
- Target .net5.0 instead of netstandard and reference the framework in a way that doesn't cause problems in projects.

## 1.1.0 - 2022-07-27
### Fixed
- Look for `$top` instead of `$take` in query options. If `$top` is absent, `$take` is looked for as well for backward compatibility with 1.0.0.

## 1.0.0 - 2021-08-03
### Added
- Bind `$search` to a `search` action parameter
- `$select` to pick a subset of identifiers from full object, returns an `IDictionary<string, object>` meant for serialization.

## 0.5.0 - 2021-01-06
### Added
- `FlatSelect` utility that picks one property out of an object

## 0.4.0 - 2021-01-05
### Added
- Open up internal API for easier extensibility: `QueryableResultFilterAttribute` and `GetODataOption`

## 0.3.0 - 2020-12-02
### Fixed
- Support nullable properties

## 0.2.0 - 2019-09-18
### Fixed
- Better `DateTime` and `DateTimeOffset` support
- Many fixes!

## 0.1.0 - 2019-07-22
Initial release!