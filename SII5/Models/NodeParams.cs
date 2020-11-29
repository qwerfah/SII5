using SII5.Helpers;

namespace SII5.Models
{
    public class NodeParams : Observable
    {
        private double _maxStorageCapacity = 0;
        private int _releaseYear = 0;
        private double _maxSpeed = 0;
        private double _averageCost = 0.0f;
        private bool _isGeneralPurpose = true;
        private MemoryType[] _memoryTypes = new MemoryType[] { MemoryType.RAM };

        /// <summary> Максимальная емкость носителя, Мб. </summary>
        public double MaxStorageCapacity
        {
            get => _maxStorageCapacity;
            set => Set(ref _maxStorageCapacity, value);
        }

        /// <summary> Год выпуска. </summary>
        public int ReleaseYear
        {
            get => _releaseYear;
            set => Set(ref _releaseYear, value);
        }

        /// <summary> Максимальная скорость передачи, Мб/с. </summary>
        public double MaxSpeed
        {
            get => _maxSpeed;
            set => Set(ref _maxSpeed, value);
        }

        /// <summary> Средняя стоимость, Руб/Мб. </summary>
        public double AverageCost
        {
            get => _averageCost;
            set => Set(ref _averageCost, value);
        }

        /// <summary> Бинарный параметр, указывающий, 
        /// являются ли носители памятью общего назначения. </summary>
        public bool IsGeneralPurpose
        {
            get => _isGeneralPurpose;
            set => Set(ref _isGeneralPurpose, value);
        }

        /// <summary> Применение данного типа памяти. </summary>
        public MemoryType[] MemoryTypes
        {
            get => _memoryTypes;
            set => Set(ref _memoryTypes, value);
        }
    }
}
