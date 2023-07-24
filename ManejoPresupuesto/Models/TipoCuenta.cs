using ManejoPresupuesto.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta //: IValidatableObject
    {
        public int ID { get; set; }

        [Required (ErrorMessage ="El campo Nombre es obligatorio")]
        //[StringLength(maximumLength:50, MinimumLength =3, ErrorMessage ="La longitud del campo {0} minima es {1} y maxima de {2}")]
        [Display(Name ="Nombre del tipo cuenta:")]
        [PrimeraLetraMayuscula]
        [Remote(action: "VerificarExisteTipoCuenta", controller:"TiposCuentas")]
        public string NOMBRE { get; set; }

        public int USUARIO_ID { get; set; }

        public int ORDEN { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (NOMBRE != null && NOMBRE.Length > 0)
        //    {
        //        var primeraLetra = NOMBRE[0].ToString();

        //        if(primeraLetra != primeraLetra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera letra debe ser mayuscula", new[] { nameof(NOMBRE) });
        //        }
                
                
        //    }  
        //}
    }
}
