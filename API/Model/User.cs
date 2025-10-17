using API.Enums;
using API.ExportClasses;
using API.InternalClasses;
using System;
using System.Collections.Generic;

namespace API.Model;

public partial class User
{
    public int UId { get; set; }

    public string USurname { get; set; } = null!;

    public string UName { get; set; } = null!;

    public string? UPatronymic { get; set; }

    public string UEmail { get; set; } = null!;

    public string UPassword { get; set; } = null!;

    public Role URole { get; set; }

    public string UPhone { get; set; } = null!;

    public DateOnly UBirthdate { get; set; }

    public decimal UPassportSerial { get; set; }

    public decimal UPassportNumber { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public ExportUser ToExport()
    {
        return new()
        {
            UId = UId,
            USurname = USurname,
            UName = UName,
            UEmail = UEmail,
            UPassword = UPassword,
            UPhone = UPhone,
            UPassportSerial = UPassportSerial,
            UPassportNumber = UPassportNumber,
            UBirthdate = UBirthdate,
            UPatronymic = UPatronymic,
            URole = Convertation.ConvertEnumToString(URole),
        };
    }
}
