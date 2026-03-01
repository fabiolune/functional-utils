> **📖 Documentation**: This documentation reflects the active development version. For stable release documentation, visit [https://fabiolune.github.io/functional-utils/](https://fabiolune.github.io/functional-utils/)

# Fl.Functional.Utils

[![dotnet](https://github.com/fabiolune/functional-utils/actions/workflows/main.yml/badge.svg)](https://github.com/fabiolune/functional-utils/actions/workflows/main.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=fabiolune_functional-utils&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=fabiolune_functional-utils)
[![codecov](https://codecov.io/gh/fabiolune/functional-utils/graph/badge.svg?token=3VW1F0AG19)](https://codecov.io/gh/fabiolune/functional-utils)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Ffabiolune%2Ffunctional-utils%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/fabiolune/functional-utils/main)

A comprehensive set of functional programming extension methods for C# that enables a functional-first approach in .NET projects. Built on top of [LanguageExt](https://github.com/louthy/language-ext), this library provides fluent APIs for common functional patterns.

## 🚀 Quick Start

Install the library via NuGet:

```bash
dotnet add PROJECT package Fl.Functional.Utils --version <version>
```

## 🎯 Overview

Fl.Functional.Utils bridges the gap between C#'s object-oriented nature and functional programming paradigms. It provides:

- **Fluent transformation pipelines** with `Map`, `Bind`, and `Tee`
- **Safe null handling** with `Option<T>` and `Either<TLeft, TRight>` types
- **Resource management** with functional `Using` patterns
- **Pattern matching** utilities for cleaner conditional logic
- **Tail recursion** support to prevent stack overflows
- **Async-first design** with full async/await support

## ✨ Core Features

| Feature | Description |
| --------- | ------------- |
| **Map** | Transform values in fluent pipelines |
| **Bind** | Chain operations that return functional types |
| **Tee/TeeWhen** | Inject side effects without breaking chains |
| **MakeOption/MakeEither** | Convert values to functional types |
| **Match** | Pattern match on functional types |
| **Using** | Resource management in functional style |
| **Tail Recursion** | Stack-safe recursive algorithms |
| **ForEach** | Null-safe iteration over collections |

## 📚 Documentation

- **[API Reference](Fl.Functional.Utils/README.md)** - Detailed documentation with examples for all utilities
- **[Online Documentation](https://fabiolune.github.io/functional-utils/)** - Complete documentation site
- **[Getting Started](docs/index.md)** - Introduction and setup guide

## 🏗️ Project Structure

```text
├── Fl.Functional.Utils/          # Main library source code
│   ├── *.cs                      # Core utility functions
│   ├── Recursion/                # Tail recursion utilities
│   └── README.md                 # Detailed API documentation
├── tests/                        # Test suite
│   └── Fl.Functional.Utils.Tests/
├── docs/                         # Documentation source
└── README.md                     # This file
```
