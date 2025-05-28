[![en](https://img.shields.io/badge/lang-en-blue.svg)](https://github.com/borisbvf/mobile-recipes/README.md)
[![ru](https://img.shields.io/badge/lang-ru-green.svg)](https://github.com/borisbvf/mobile-recipes/README.ru.md)


# Кроссплатформенное приложение Рецепты
Приложение для хранения и упорядочивания рецептов.

## Реализованные функции:
Редактирование и поиск рецептов, списки ингридиентов и ярлыков, добавление изображений, экспорт в pdf. Данные хранятся на телефоне, есть возможность выгрузить архив базы данных.

## Платформы:
* Windows
* Android
* Предположительно iOS и macOS

## Используемые сторонние библиотеки:
* [sqlite-net-pcl](https://github.com/praeclarum/sqlite-net) – работа с базой данных SQLite.
* [PDFsharp-MigraDoc](https://github.com/empira/PDFsharp) – отличная библиотека для работы с pdf и текстовыми документами, с огромными возможностями, и при этом полностью бесплатная и без ограничений.
* Maui Community Toolkit – использован исходный код некоторых элементов (Toast, FolderPicker), сама библиотека не используется.
