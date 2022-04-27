
namespace UI.BaseClass
{
    public class BindableProperty<T>
    {
        public delegate void ValueChangedEvent(T oldValue, T newValue);
        public event ValueChangedEvent OnValueChanged;
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (object.Equals(_value, value)) return;
                T oldValue = _value;
                _value = value;
                ValueChanged(oldValue, value);
            }
        }
        private void ValueChanged(T oldValue, T newValue)
        {
            OnValueChanged?.Invoke(oldValue, newValue);
        }
    }
}