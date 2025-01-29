# AndroidCamChanger

A Windows 11 console application for managing virtual cameras and connecting them to Android emulators. Designed for developers working with computer vision, AR/VR, and Android virtual device configurations.

---

## Requirements

- **OS**: Windows 11 (22H2 or later)
- **Runtime**: .NET 8.0+ SDK
- **Android Tools**: 
  - Android SDK with platform-tools
  - Android Emulator (API 30+ recommended)
- **Environment Variables**:
  - `ANDROID_HOME` or `ANDROID_SDK_ROOT` (must point to valid SDK location)

---

## Installation


# 1. Clone repository

git clone https://github.com/NeuroGhostOutput/CamLinker.git
```
# 2. Build solution
```bash
cd CamLinker
dotnet build --configuration Release
```
Command-Line Interface
Core Features:
bash
Copy

CamLinker.exe [command] [options]

Command	Description
list --all	Enumerate all camera devices
list --virtual	Filter virtual cameras (OBS/DroidCam/etc)
assign --name [NAME]	Bind camera as webcam0
connect --emulator [ID]	Route webcam0 to Android emulator
refresh	Reload device list
Usage Example:
bash
Copy

# 1. Identify virtual cameras
CamLinker.exe list --virtual

# 2. Assign camera
CamLinker.exe assign --name "OBS Virtual Camera"

# 3. Connect to running emulator
CamLinker.exe connect --emulator <vm name>

Technical Implementation
Key Components:

    Windows Camera Stack Integration

        Registry modifications under HKLM\SOFTWARE\Microsoft\Windows Media Foundation\Platform

        DirectShow filter enumeration via MFEnumDeviceSources()

    Android Toolchain Interaction

        ADB protocol implementation for emulator control

        Video4Linux (V4L2) emulation layer injection

Security Context:
powershell
Copy

# Requires elevated privileges for registry operations
Start-Process CamLinker.exe -Verb RunAs

Debugging & Diagnostics
Common Issues:
Symptom	Resolution
SDK path not detected	Verify ANDROID_HOME contains emulator/
Camera binding failure	Check driver signing (Requires WHQL)
ADB connection timeout	Ensure emulator is running with -writable-system
Logging:
bash
Copy

CamLinker.exe --verbose --log-level debug

Contribution Guidelines

    Follow Windows Driver Kit standards

    Use XML documentation for public APIs

    Maintain compatibility with:

        Multiple emulator instances (Genymotion, AVD)

        Popular virtual cameras (OBS, ManyCam, DroidCam)

License: MIT
Maintainer: NEuroGhost

P.S.
Dear customer if you like my software you can donate to me BTC address is [ bc1qag5mm2afhk40ydp6v4f2s0a37vp34rj069jnn5 ]

This version emphasizes:  
1. Professional CLI documentation format  
2. Clear technical implementation details  
3. Enterprise-grade troubleshooting table  
4. Security context requirements  
5. Contributor-focused guidelines  
6. Modern markdown formatting with console-aware styling



Консольное приложение для Windows 11, позволяющее находить все активные виртуальные камеры в системе и подключать указанную камеру (webcam0) к эмулятору Android.

## Требования

- Windows 11
- .NET 8.0 или выше
- Android SDK (с установленными platform-tools и эмулятором)
- Настроенные переменные окружения:
  - ANDROID_HOME или ANDROID_SDK_ROOT (путь к Android SDK)

## Установка

1. Убедитесь, что у вас установлен .NET 8.0 SDK
2. Склонируйте репозиторий
3. Перейдите в директорию проекта
4. Выполните сборку:
```bash
dotnet build
```

## Использование

1. Запустите приложение из директории проекта:
```bash
cd d:/software/camLinker/CamLinker
dotnet run
```

2. В главном меню доступны следующие опции:
   - Показать список всех камер
   - Показать только виртуальные камеры
   - Назначить камеру как webcam0
   - Подключить webcam0 к эмулятору
   - Обновить список устройств

3. Для подключения камеры к эмулятору:
   - Выберите пункт "Назначить камеру как webcam0"
   - Выберите нужную камеру из списка
   - После успешного назначения используйте пункт "Подключить webcam0 к эмулятору"

## Важные замечания

- Убедитесь, что эмулятор Android запущен перед попыткой подключения камеры
- Приложение должно быть запущено от имени администратора для доступа к реестру Windows
- Android SDK должен быть корректно установлен и настроен
- При назначении камеры как webcam0 происходит регистрация в системном реестре
- После назначения камеры может потребоваться перезапуск приложений, использующих камеру

## Решение проблем

1. Если приложение не может найти Android SDK:
   - Проверьте, что переменная ANDROID_HOME или ANDROID_SDK_ROOT установлена
   - Убедитесь, что путь содержит необходимые инструменты (adb.exe, emulator.exe)

2. Если камера не определяется как виртуальная:
   - Проверьте, что имя камеры содержит ключевые слова (virtual, obs, droid)
   - Убедитесь, что драйвер виртуальной камеры установлен корректно

3. Если не удается подключить камеру к эмулятору:
   - Проверьте, что эмулятор запущен
   - Убедитесь, что у приложения есть необходимые права доступа
   - Проверьте логи эмулятора на наличие ошибок
   - 
Разработчик: NEuroGhost

P.S.
Дорогой пользователь, если мои труды оказались для тебя полезными и ты хотел бы отблагодарить меня, ты можешь это сделать через BTC вот адрес [ bc1qag5mm2afhk40ydp6v4f2s0a37vp34rj069jnn5 ]  
