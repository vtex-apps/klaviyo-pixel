# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.4.0] - 2021-03-29

### Added

- Send additional data to Klaviyo with events `vtex:pageView`, `vtex.removeFromCart`, `vtex:categoryView`, `vtex:addToWishlist`, and `vtex:removeFromWishlist`

## [0.3.0] - 2021-03-23

### Fixed

- Improve pixel data sent to Klaviyo in `vtex:productView`, `vtex:addToCart` and `vtex:cartChanged` events

## [0.2.0] - 2021-01-27

### Added

- Add Order Canceled Event

### Fixed

- Fixed json deserialization error

## [0.1.2] - 2021-01-26

### Fixed

- Fixed json deserialization error

## [0.1.1] - 2020-12-09

### Fixed

- Fixed json deserialization error

## [0.1.0] - 2020-10-29

### Added

- Add Event Broadcast listener
- Update docs with Checkout Integration instructions

### Fixed

- Eliminate Public API Key app settings field (alternate name for Company ID)

## [0.0.8] - 2020-07-07

### Fixed

- Items Event feed

## [0.0.7] - 2020-06-19

### Fixed

- PickupStoreInfo deserialization error

## [0.0.6] - 2020-06-18

### Fixed

- App setting path
- Deserialization error

## [0.0.5] - 2020-06-18

### Fixed

- Only use Rates and Benefits name for Discount Code field

## [0.0.4] - 2020-06-17

### Fixed

- Bug deserializing Rates and Benefits

## [0.0.3] - 2020-06-17

### Added

- Order logging

## [0.0.2] - 2020-06-17

### Added

- Logging

## [0.0.1] - 2020-06-04

### Added

- Initial release
