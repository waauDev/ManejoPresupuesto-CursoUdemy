namespace ManejoPresupuesto.Models
{
    public class IndiceCuentasViewModel
    {
        public string Tipo_Cuenta { get; set; }

        public IEnumerable<Cuenta> Cuentas  { get; set; }

        public decimal Balance => Cuentas.Sum(x => x.Balance);
    }
}
