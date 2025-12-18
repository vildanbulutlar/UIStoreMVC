using Domain.Enums;
using FluentValidation;
using UIStoreMVC.Models;

namespace UIStoreMVC.Validations
{
    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Ad Soyad zorunludur.")
                .MinimumLength(3).WithMessage("Ad Soyad en az 3 karakter olmalıdır.")
                .MaximumLength(100).WithMessage("Ad Soyad en fazla 100 karakter olabilir.")
                .Matches(@"^[a-zA-ZığüşöçİĞÜŞÖÇ\s]+$")
                .WithMessage("Ad Soyad yalnızca harf ve boşluk içerebilir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre zorunludur.")
                .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
                .Matches("[A-Z]").WithMessage("En az bir büyük harf olmalıdır.")
                .Matches("[a-z]").WithMessage("En az bir küçük harf olmalıdır.")
                .Matches("[0-9]").WithMessage("En az bir rakam olmalıdır.")
                .Matches("[^a-zA-Z0-9]").WithMessage("En az bir özel karakter olmalıdır.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Şifre tekrar zorunlu.")
                .Equal(x => x.Password).WithMessage("Şifreler eşleşmiyor.");

            When(x => x.MembershipType == MembershipType.Agency, () =>
            {
                RuleFor(x => x.CompanyName)
                    .NotEmpty().WithMessage("Ajans üyeliği için firma adı zorunlu.");

                RuleFor(x => x.TaxNumber)
                    .NotEmpty().WithMessage("Vergi numarası zorunlu.")
                    .Matches(@"^[0-9]{10}$").WithMessage("Vergi numarası 10 haneli olmalı.");
            });
        }
    }
}
