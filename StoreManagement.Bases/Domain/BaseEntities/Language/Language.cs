using StoreManagement.Bases.Domain.BaseEntities.localization;
using System;

namespace StoreManagement.Bases.Domain.BaseEntities.Language;

public class Language : BaseCommonEntity<Guid>, ILanguage
{
    public string Language_Code { get; set; }
    public string Language_Name { get; set; }

    public bool Is_Rtl { get; set; }

}

