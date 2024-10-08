# Сервис для обработки видео (backend)

Сервис для отправки данных между сервисом машинного обучения и браузерным клиентом.
* фреймворк: asp net core;
* база данных - postgres;
* менеджер задач - quartz;

## Конфигурация
Конфигурацию проекта следует осуществлять при помощи переменных окружения:

* `S3Options__ServiceUrl` - url к сервису s3;
* `S3Options__BucketName` - имя бакета;
* `S3Options__AccessKeyId` - id ключа доступа s3;
* `S3Options__SecretAccessKey` - секретный ключ доступа s3;
* `ConnectionStrings__PostgresDb` - строка подключения к postgres;
* `ConnectionStrings__MlService` - домен сервиса машинного обучения;
* `UrlGenerationOptions__LinkToVideoTemplate` - шаблон для генерации ссылки на скачивание видео с s3;

## Запуск
Есть два варианта для локального завпуска сервиса:
1. Склонировать репозиторий и запустить в режиме разработки:
```bash
git clone https://github.com/eiparfenov/cp-hack-backend.git
cd web-api
dotnet run
```
2. Использовать docker образ из публичного репозитория:
```bash
docker run eiparfenov/cp_backend
```
!!! При запуске из контенера не забыть сконфигурировать проект через переменные окружения (параметр `-e`)
