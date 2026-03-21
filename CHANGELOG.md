# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0]

### Changed
- Renamed GameSystem concept to Singleton
- Renamed SystemsStart to SingletonMaster
- Renamed package to com.jonathonoh.unity.singleton

## [1.0.2]

### Added
- CHANGELOG.md
- LICENSE.md

### Changed
- More concise README.md

### Removed
- Verbose log option.
- Excessive debug logs. 

## [1.0.1] 2026-03-21

### Added
- Verbose log option.

### Fixed
- Other systems continue to initialize when one fails.

## [1.0.0] - 2024-08-27

### Added
- GameSystem as a singleton.
- Initialization of all GameSystems when one is accessed.
- Access through interfaces (undocumented :/)
