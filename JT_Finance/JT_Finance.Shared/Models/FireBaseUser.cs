namespace JT_Finance.Shared.Models
{
    /// <summary>
    /// Representa o usuário autenticado do Firebase.
    /// Compatível com autenticação via JSInterop (web) e REST API (desktop/mobile).
    /// </summary>
    public class FirebaseUser
    {
        /// <summary>
        /// ID único do usuário (UID do Firebase)
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// E-mail do usuário
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Nome exibido (pode ser nulo quando login por e-mail/senha)
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// ID token JWT retornado pelo Firebase (REST API)
        /// </summary>
        public string IdToken { get; set; }

        /// <summary>
        /// Identificador local do usuário (REST API)
        /// </summary>
        public string LocalId { get; set; }

        public FirebaseUser() { }

        public FirebaseUser(string uid, string email, string displayName = null)
        {
            Uid = uid;
            Email = email;
            DisplayName = displayName;
        }
    }

    /// <summary>
    /// Resultado genérico de autenticação (login)
    /// </summary>
    public class AuthResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public FirebaseUser User { get; set; }

        /// <summary>
        /// Indica se o login foi feito via redirect (usado no modo web)
        /// </summary>
        public bool Redirect { get; set; }
    }
}
