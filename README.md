# MediaFileTranscriber

Консольная утилита на C# для расшифровки **MP3** и **MP4** в текст с помощью
[Whisper.net](https://github.com/sandrohanea/whisper.net) (обёртка над whisper.cpp).

Выбираешь формат → открывается проводник → выбираешь файлы → получаешь `.txt`.

---

## Возможности

- Поддержка **MP3** и **MP4**
- Выбор файлов через системный проводник (можно несколько сразу)
- Локальная расшифровка — **интернет не нужен**, ничего не уходит на сервера
- Русский язык из коробки
- Результат — обычный `.txt`

---

## Требования

- **Windows 10/11**
- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** — если ещё не установлен
- **FFmpeg** — для конвертации аудио/видео в WAV
- **Модель Whisper** — файл `ggml-*.bin`

---

## Установка

### 1. Склонировать репозиторий

```bash
git clone https://github.com/ТВОЙ_НИК/MediaFileTranscriber.git
cd MediaFileTranscriber
