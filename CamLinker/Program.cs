using System;
using System.Linq;
using System.Threading.Tasks;

namespace CamLinker
{
    /// <summary>
    /// Основной класс приложения для связывания виртуальных камер с эмулятором Android
    /// </summary>
    public class Program
    {
        private static readonly CameraManager _cameraManager = new();
        private static AndroidEmulatorManager? _emulatorManager;

        /// <summary>
        /// Точка входа в приложение
        /// </summary>
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("CamLinker - Утилита для подключения виртуальных камер к эмулятору Android");
            Console.WriteLine("==================================================================");

            try
            {
                _emulatorManager = new AndroidEmulatorManager();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка инициализации Android SDK: {ex.Message}");
                Console.WriteLine("Убедитесь, что Android SDK установлен и настроен корректно.");
                return;
            }

            while (true)
            {
                try
                {
                    await ShowMenuAndProcessSelectionAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nПроизошла ошибка: {ex.Message}");
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения или ESC для выхода...");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    break;
                
                Console.Clear();
            }
        }

        /// <summary>
        /// Отображает меню и обрабатывает выбор пользователя
        /// </summary>
        private static async Task ShowMenuAndProcessSelectionAsync()
        {
            Console.WriteLine("\nДоступные действия:");
            Console.WriteLine("1. Показать список всех камер");
            Console.WriteLine("2. Показать только виртуальные камеры");
            Console.WriteLine("3. Назначить камеру как webcam0");
            Console.WriteLine("4. Подключить webcam0 к эмулятору");
            Console.WriteLine("5. Обновить список устройств");
            Console.WriteLine("0. Выход");

            Console.Write("\nВыберите действие: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowAllCameras();
                    break;
                case "2":
                    ShowVirtualCameras();
                    break;
                case "3":
                    SelectAndSetWebcam0();
                    break;
                case "4":
                    await ConnectWebcam0ToEmulatorAsync();
                    break;
                case "5":
                    _cameraManager.RefreshDeviceList();
                    Console.WriteLine("Список устройств обновлен");
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
        }

        /// <summary>
        /// Отображает список всех доступных камер
        /// </summary>
        private static void ShowAllCameras()
        {
            var cameras = _cameraManager.GetAllCameras().ToList();
            if (!cameras.Any())
            {
                Console.WriteLine("Камеры не найдены");
                return;
            }

            Console.WriteLine("\nСписок всех камер:");
            foreach (var (name, id) in cameras)
            {
                var isVirtual = _cameraManager.IsVirtualCamera(id);
                Console.WriteLine($"- {name} [{(isVirtual ? "Виртуальная" : "Физическая")}]");
            }
        }

        /// <summary>
        /// Выбирает камеру и назначает её как webcam0
        /// </summary>
        private static void SelectAndSetWebcam0()
        {
            var cameras = _cameraManager.GetAllCameras().ToList();
            if (!cameras.Any())
            {
                Console.WriteLine("Камеры не найдены");
                return;
            }

            Console.WriteLine("\nДоступные камеры:");
            for (int i = 0; i < cameras.Count; i++)
            {
                var (name, id) = cameras[i];
                var isVirtual = _cameraManager.IsVirtualCamera(id);
                Console.WriteLine($"{i + 1}. {name} [{(isVirtual ? "Виртуальная" : "Физическая")}]");
            }

            Console.Write("\nВыберите номер камеры для назначения как webcam0: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > cameras.Count)
            {
                Console.WriteLine("Неверный выбор");
                return;
            }

            var selectedCamera = cameras[choice - 1];
            if (_cameraManager.SetAsWebcam0(selectedCamera.Id))
            {
                Console.WriteLine($"Камера {selectedCamera.Name} успешно назначена как webcam0");
            }
            else
            {
                Console.WriteLine("Не удалось назначить камеру как webcam0");
            }
        }

        /// <summary>
        /// Отображает список только виртуальных камер
        /// </summary>
        private static void ShowVirtualCameras()
        {
            var cameras = _cameraManager.GetAllCameras()
                .Where(c => _cameraManager.IsVirtualCamera(c.Id))
                .ToList();

            if (!cameras.Any())
            {
                Console.WriteLine("Виртуальные камеры не найдены");
                return;
            }

            Console.WriteLine("\nСписок виртуальных камер:");
            foreach (var (name, _) in cameras)
            {
                Console.WriteLine($"- {name}");
            }
        }

        /// <summary>
        /// Подключает webcam0 к эмулятору Android
        /// </summary>
        private static async Task ConnectWebcam0ToEmulatorAsync()
        {
            if (_emulatorManager == null)
            {
                Console.WriteLine("Менеджер эмулятора не инициализирован");
                return;
            }

            var camera = _cameraManager.FindCameraByName("webcam0");
            if (camera == null)
            {
                Console.WriteLine("Камера webcam0 не найдена");
                return;
            }

            if (!_cameraManager.IsVirtualCamera(camera.Value.Id))
            {
                Console.WriteLine("webcam0 не является виртуальной камерой");
                return;
            }

            Console.WriteLine("Подключение камеры к эмулятору...");
            var success = await _emulatorManager.ConnectCameraToEmulatorAsync(camera.Value.Id);
            
            if (success)
            {
                Console.WriteLine("Камера успешно подключена к эмулятору");
            }
            else
            {
                Console.WriteLine("Не удалось подключить камеру к эмулятору");
            }
        }
    }
}
