﻿namespace Hub.Infrastructure.Architecture.Internacionalization.Enums
{
    public enum ESpecialDocumentValidation
    {
        None = 0,

        CPF = 1,
        CNPJ = 2,
        RUT = 3,

        // Colombia
        ColombianCC = 4,

        // Argentina
        ArgentineDNI = 5,
        ArgentineCUIT = 6,

        // Paraguay
        ParaguayanCI = 7,
        EUASSN = 8,
    }
}
