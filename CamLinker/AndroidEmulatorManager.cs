using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace CamLinker
{
    /// <summary>
    /// Менеджер для работы с эмулятором Android
    /// </summary>
    public class AndroidEmulatorManager
    {
        private readonly string _adbPath;
        private readonly string _emulatorPath;

        public AndroidEmulatorManager()
        {
            // Пути к Android SDK инструментам
            var androidHome = Environment.GetEnvironmentVariable("ANDROID_HOME") 
                ?? Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");

            if (string.IsNullOrEmpty(androidHome))
            {
                throw new InvalidOperationException(
                    "Не найдена переменная окружения ANDROID_HOME или ANDROID_SDK_ROOT. " +
                    "Убедитесь, что Android SDK установлен корректно.");
            }

            _adbPath = Path.Combine(androidHome, "platform-tools", "adb.exe");
            _emulatorPath = Path.Combine(androidHome, "emulator", "emulator.exe");

            if (!File.Exists(_adbPath) || !File.Exists(_emulatorPath))
            {
                throw new FileNotFoundException(
                    "Не найдены необходимые инструменты Android SDK (adb.exe или emulator.exe)");
            }
        }

        /// <summary>
        /// Проверяет, запущен ли эмулятор
        /// </summary>
        /// <returns>true если эмулятор запущен, иначе false</returns>
        public async Task<bool> IsEmulatorRunningAsync()
        {
            try
            {
                var result = await ExecuteAdbCommandAsync("devices");
                return result.Contains("emulator-");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке статуса эмулятора: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Подключает виртуальную камеру к эмулятору
        /// </summary>
        /// <param name="cameraId">Идентификатор виртуальной камеры</param>
        /// <returns>true если подключение успешно, иначе false</returns>
        public async Task<bool> ConnectCameraToEmulatorAsync(string cameraId)
        {
            try
            {
                if (!await IsEmulatorRunningAsync())
                {
                    Console.WriteLine("Эмулятор не запущен");
                    return false;
                }

                // Подключаем камеру к эмулятору через adb
                var command = $"emu webcam attach {cameraId}";
                var result = await ExecuteAdbCommandAsync(command);

                return !result.Contains("error", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при подключении камеры: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Выполняет ADB команду
        /// </summary>
        /// <param name="command">Команда для выполнения</param>
        /// <returns>Результат выполнения команды</returns>
        private async Task<string> ExecuteAdbCommandAsync(string command)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _adbPath,
                    Arguments = command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"ADB команда завершилась с ошибкой: {error}");
                }

                return output;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при выполнении ADB команды: {ex.Message}");
            }
        }
    }
}
