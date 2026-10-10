# MediaFileTranscriber

Консольная утилита на C# для расшифровки **MP3** и **MP4** в текст с помощью
[Whisper.net](https://github.com/sandrohanea/whisper.net) — обёртки над whisper.cpp.

Выбираешь формат → открывается проводник → выбираешь файлы → получаешь `.txt`.
Расшифровка идёт **локально**, ничего не отправляется в интернет.

---

## Возможности

- Поддержка **MP3** и **MP4**
- Выбор файлов через системный проводник (можно несколько сразу)
- Локальная расшифровка — интернет не нужен
- Русский язык из коробки
- Результат — обычный `.txt`

---

## Требования

- **Windows 10/11**
- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
- **FFmpeg** (`ffmpeg.exe`)
- **Модель Whisper** (`ggml-*.bin`)

> ⚠️ FFmpeg и модель **не хранятся в репозитории** из-за размера.
> Скачай их отдельно по ссылкам ниже и положи в корень проекта.

---

## Где скачать нужные файлы

### FFmpeg

- **Прямая ссылка на архив (essentials, ~109 МБ):**  
  https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip

- **Страница со всеми сборками (если нужна другая версия):**  
  https://www.gyan.dev/ffmpeg/builds/

**Что делать с архивом:**
1. Распакуй
2. Найди внутри `bin\ffmpeg.exe`
3. Положи его **в корень проекта** (рядом с `Program.cs`)

### Модели Whisper

Выбери одну модель по размеру/качеству:

| Модель | Ссылка | Размер | Качество |
|---|---|---|---|
| **base** | https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-base.bin | ~142 МБ | базовое |
| **small** | https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-small.bin | ~466 МБ | хорошее |
| **medium** | https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-medium.bin | ~1.5 ГБ | очень хорошее |
| **large-v3** | https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-large-v3.bin | ~3 ГБ | лучшее |

**Начни с `ggml-base.bin`** — быстро и достаточно для теста. Скачай его **в корень проекта**.

Если HuggingFace не открывается — зеркало:  
https://hf-mirror.com/ggerganov/whisper.cpp/tree/main

### .NET 8 SDK

https://dotnet.microsoft.com/download/dotnet/8.0

