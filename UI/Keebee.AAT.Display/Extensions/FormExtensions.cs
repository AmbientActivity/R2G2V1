using System.Linq;
using System.Windows.Forms;

namespace Keebee.AAT.Display.Extensions
{
    public static class FormExtensions
    {
        public static bool IsOpen(this Form form)
        {
            var fc = Application.OpenForms;

            return fc.Cast<Form>().Any(frm => frm.Name == form.Name);
        }
    }
}
