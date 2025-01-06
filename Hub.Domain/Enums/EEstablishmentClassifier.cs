using System;
using System.ComponentModel.DataAnnotations;

namespace Hub.Domain.Enums
{
    public enum EEstablishmentClassifier
    {
        [Display(Name = "Owned")]
        Owned = 1,  // Própria: Quando o estabelecimento é de propriedade da empresa.

        [Display(Name = "Leased")]
        Leased = 2,  // Alugada: Quando o estabelecimento é alugado pela empresa.

        [Display(Name = "Outsourced")]
        Outsourced = 3,  // Outsourced: Quando o estabelecimento é operado por uma empresa terceirizada.

        [Display(Name = "Franchised")]
        Franchised = 4,  // Franqueada: Quando o estabelecimento é parte de uma franquia.

        [Display(Name = "Partnered")]
        Partnered = 5,  // Parceira: Quando o estabelecimento é operado por meio de uma parceria entre empresas.

        [Display(Name = "Other")]
        Other = 6  // Outros: Qualquer outra classificação que não se encaixe nas opções anteriores.
    }
}
