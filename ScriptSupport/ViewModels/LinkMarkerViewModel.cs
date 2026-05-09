using ScriptSupport.States;
using ScriptSupport.Commands;

namespace ScriptSupport.ViewModels
{
    public class LinkMarkerViewModel : BaseViewModel, IDisposable
    {
        public UIConfigState UIConfig { get; }

        #region Propertys
        private long _linkMakers = 0;
        public long LinkMakers
        {
            get => _linkMakers;
            set
            {
                if (SetProperty(ref _linkMakers, value))
                {
                    OnPropertyChanged(nameof(BottomLeft));
                    OnPropertyChanged(nameof(Bottom));
                    OnPropertyChanged(nameof(BottomRight));
                    OnPropertyChanged(nameof(Left));
                    OnPropertyChanged(nameof(Right));
                    OnPropertyChanged(nameof(TopLeft));
                    OnPropertyChanged(nameof(Top));
                    OnPropertyChanged(nameof(TopRight));

                    DebugLinkMakers(value);
                }
            }
        }
        private void DebugLinkMakers(long value)
        {
            // 1. Decimal
            System.Diagnostics.Debug.WriteLine($"[LinkMakers] DEC: {value}");

            // 2. Binary 32-bit (có padding)
            string binary = Convert.ToString(value & 0xFFFFFFFF, 2).PadLeft(32, '0');
            System.Diagnostics.Debug.WriteLine($"[LinkMakers] BIN : {binary}");

            // 3. Decode từng bit đang bật
            var directions = new Dictionary<int, string>
            {
                {0, "BottomLeft"},
                {1, "Bottom"},
                {2, "BottomRight"},
                {3, "Left"},
                {5, "Right"},
                {6, "TopLeft"},
                {7, "Top"},
                {8, "TopRight"},
            };

            var active = directions
                .Where(b => (value & (1L << b.Key)) != 0)
                .Select(b => $"{b.Value}(bit {b.Key})")
                .ToList();

            System.Diagnostics.Debug.WriteLine(
                "[LinkMakers] ACTIVE: " + (active.Count == 0 ? "None" : string.Join(", ", active))
            );
        }

        public bool BottomLeft
        {
            get => (_linkMakers & (1L << 0)) != 0;
            set => SetBit(0, value, nameof(BottomLeft));
        }
        public bool Bottom
        {
            get => (_linkMakers & (1L << 1)) != 0;
            set => SetBit(1, value, nameof(Bottom));
        }
        public bool BottomRight
        {
            get => (_linkMakers & (1L << 2)) != 0;
            set => SetBit(2, value, nameof(BottomRight));
        }
        public bool Left
        {
            get => (_linkMakers & (1L << 3)) != 0;
            set => SetBit(3, value, nameof(Left));
        }
        public bool Right
        {
            get => (_linkMakers & (1L << 5)) != 0;
            set => SetBit(5, value, nameof(Right));
        }
        public bool TopLeft
        {
            get => (_linkMakers & (1L << 6)) != 0;
            set => SetBit(6, value, nameof(TopLeft));
        }
        public bool Top
        {
            get => (_linkMakers & (1L << 7)) != 0;
            set => SetBit(7, value, nameof(Top));
        }
        public bool TopRight
        {
            get => (_linkMakers & (1L << 8)) != 0;
            set => SetBit(8, value, nameof(TopRight));
        }
        #endregion

        public RelayCommand ClearLinkMakerCommand { get; set; }

        public LinkMarkerViewModel(UIConfigState uiConfig)
        {
            UIConfig = uiConfig;
            ClearLinkMakerCommand = new ScriptSupport.Commands.RelayCommand(_ => ClearLinkMaker());
        }

        private void SetBit(int position, bool value, string propertyName)
        {
            long newValue = _linkMakers;
            if (value) newValue |= 1L << position;
            else newValue &= ~(1L << position);

            if (newValue != _linkMakers)
            {
                LinkMakers = newValue;
            }
        }
        private void ClearLinkMaker()
        {
            LinkMakers = 0;
        }

        public void Dispose()
        {

        }
    }
}
