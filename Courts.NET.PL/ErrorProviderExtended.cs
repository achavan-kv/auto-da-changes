using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace STL.PL
{
    public class ErrorProviderExtended : ErrorProvider
    {
        private Dictionary<Control, string> _errors = new Dictionary<Control,string>();
        
        public ErrorProviderExtended()
            :base()
        { }

        public ErrorProviderExtended(ContainerControl parentControl)
            : base(parentControl)
        { }

        public ErrorProviderExtended(System.ComponentModel.IContainer container)
            : base(container)
        { }

        public new void SetError(Control control, string value)
        {
            base.SetError(control, value);
            
            if(value != null && value.Length > 0)
            {
                if(_errors.ContainsKey(control))
                    _errors[control] = value;
                else
                    _errors.Add(control, value);
            }
            else
            {
                _errors.Remove(control);
            }
        }
 
        public new void Clear()
        {
            base.Clear();
            _errors.Clear();
        }

        public bool HasErrors()
        {
            return _errors.Count > 0;
        }

        public IEnumerable<string> GetErrors()
        {
            foreach(var value in _errors.Values)
                yield return value;

        }
    }
}
