using Microsoft.AspNetCore.Identity;

namespace nexumApp.Models
{
    public class DuplicateUserDescriber: IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            var error = base.DuplicateUserName(userName);
            error.Description = "Esse Email já foi cadastrado.";
            return error;
        }
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            var error = base.PasswordRequiresNonAlphanumeric();
            error.Description = "A Senha deve conter pelo menos um caractere não alfanumérico.";
            return error;
        }
        public override IdentityError PasswordRequiresUpper()
        {
            var error = base.PasswordRequiresUpper();
            error.Description = "A Senha deve conter pelo menos um caractere maiúsculo.";
            return error;
        }
        public override IdentityError PasswordRequiresLower()
        {
            var error = base.PasswordRequiresLower();
            error.Description = "A Senha deve conter pelo menos um caractere minúsculo.";
            return error;
        }
        public override IdentityError PasswordRequiresDigit()
        {
            var error = base.PasswordRequiresDigit();
            error.Description = "A Senha deve conter pelo menos um número.";
            return error;
        }
        public override IdentityError PasswordMismatch()
        {
            var error = base.PasswordMismatch();
            error.Description = "Senha incorreta";
            return error;
        }
    }
}
