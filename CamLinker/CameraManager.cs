using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamLinker
{
    /// <summary>
    /// Менеджер для работы с виртуальными камерами в системе
    /// </summary>
    public class CameraManager
    {
        private readonly List<DsDevice> _videoDevices;

        public CameraManager()
        {
            _videoDevices = new List<DsDevice>();
            RefreshDeviceList();
        }

        /// <summary>
        /// Обновляет список доступных видеоустройств
        /// </summary>
        public void RefreshDeviceList()
        {
            _videoDevices.Clear();
            _videoDevices.AddRange(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));
        }

        /// <summary>
        /// Получает список всех доступных камер
        /// </summary>
        /// <returns>Список камер с их именами и идентификаторами</returns>
        public IEnumerable<(string Name, string Id)> GetAllCameras()
        {
            return _videoDevices.Select(device => (device.Name, device.DevicePath));
        }

        /// <summary>
        /// Находит виртуальную камеру по имени
        /// </summary>
        /// <param name="cameraName">Имя камеры для поиска</param>
        /// <returns>Информация о найденной камере или null</returns>
        public (string Name, string Id)? FindCameraByName(string cameraName)
        {
            var device = _videoDevices.FirstOrDefault(d => 
                d.Name.Contains(cameraName, StringComparison.OrdinalIgnoreCase));
            
            return device != null ? (device.Name, device.DevicePath) : null;
        }

        /// <summary>
        /// Назначает выбранную камеру как webcam0
        /// </summary>
        /// <param name="cameraId">Идентификатор камеры для назначения</param>
        /// <returns>true если операция успешна, иначе false</returns>
        public bool SetAsWebcam0(string cameraId)
        {
            try
            {
                var device = _videoDevices.FirstOrDefault(d => d.DevicePath == cameraId);
                if (device == null)
                {
                    Console.WriteLine("Камера не найдена");
                    return false;
                }

                // Путь в реестре для виртуальных камер
                string registryPath = @"SOFTWARE\Classes\CLSID\{860BB310-5D01-11d0-BD3B-00A0C911CE86}\Instance";

                using (var key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(registryPath))
                {
                    if (key == null)
                    {
                        Console.WriteLine("Не удалось получить доступ к реестру");
                        return false;
                    }

                    // Сохраняем идентификатор устройства
                    key.SetValue("DevicePath", device.DevicePath);
                    key.SetValue("FriendlyName", "webcam0");
                    key.SetValue("CLSID", "{860BB310-5D01-11d0-BD3B-00A0C911CE86}");
                }

                // Перезагружаем список устройств для применения изменений
                RefreshDeviceList();

                Console.WriteLine($"Камера {device.Name} успешно назначена как webcam0");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при назначении камеры: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Проверяет, является ли камера виртуальной
        /// </summary>
        /// <param name="cameraId">Идентификатор камеры</param>
        /// <returns>true если камера виртуальная, иначе false</returns>
        public bool IsVirtualCamera(string cameraId)
        {
            // Примечание: это упрощенная проверка, может потребоваться дополнительная логика
            // в зависимости от используемого ПО виртуальных камер
            var device = _videoDevices.FirstOrDefault(d => d.DevicePath == cameraId);
            if (device == null) return false;

            return device.Name.Contains("virtual", StringComparison.OrdinalIgnoreCase) ||
                   device.Name.Contains("obs", StringComparison.OrdinalIgnoreCase) ||
                   device.Name.Contains("droid", StringComparison.OrdinalIgnoreCase);
        }
    }
}
