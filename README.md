[![en](https://img.shields.io/badge/lang-en-blue.svg)](https://github.com/borisbvf/mobile-recipes/blob/main/README.md)
[![ru](https://img.shields.io/badge/lang-ru-green.svg)](https://github.com/borisbvf/mobile-recipes/blob/main/README.ru.md)


# Recipes cross-platform application
An application for storing and organizing recipes.

## Implemented features:
Recipe editing and search, ingredient and label lists, image attachments, and PDF export. Data is stored locally on the device, with an option to export a database archive.

## Platforms:
* Windows
* Android
* Possibly iOS and macOS

## Third-party libraries used:
* [sqlite-net-pcl](https://github.com/praeclarum/sqlite-net) – for working with the SQLite database.
* [PDFsharp-MigraDoc](https://github.com/empira/PDFsharp) – an excellent library for working with PDFs and text documents. It offers extensive features and is completely free with no limitations.
* Maui Community Toolkit – source code for certain components (Toast, FolderPicker) was used, though the library itself is not included.
