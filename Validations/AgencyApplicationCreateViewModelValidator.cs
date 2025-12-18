using FluentValidation;
using UIStoreMVC.Models.Membership.Agency;

namespace UIStoreMVC.Validations
{
    public class AgencyApplicationCreateViewModelValidator
        : AbstractValidator<AgencyApplicationCreateViewModel>
    {
        public AgencyApplicationCreateViewModelValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Firma adı zorunludur.")
                .MinimumLength(3).WithMessage("Firma adı en az 3 karakter olmalıdır.");

            RuleFor(x => x.TaxNumber)
                .NotEmpty().WithMessage("Vergi numarası zorunludur.")
                .Length(10, 11).WithMessage("Vergi numarası 10–11 haneli olmalıdır.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Telefon zorunludur.");

            RuleFor(x => x.ContactPerson)
                .NotEmpty().WithMessage("Yetkili kişi zorunludur.");

            RuleFor(x => x.TaxDocument)
                .NotNull().WithMessage("Vergi levhası yüklenmelidir.");

            RuleFor(x => x.SignatureCircular)
                .NotNull().WithMessage("İmza sirküleri yüklenmelidir.");

            // Opsiyonel – istersen kapalı bırak
            RuleFor(x => x.TradeRegistry)
                .Must(file => file == null || file.Length > 0)
                .WithMessage("Ticaret sicil dosyası hatalı.");
        }
    }
}
