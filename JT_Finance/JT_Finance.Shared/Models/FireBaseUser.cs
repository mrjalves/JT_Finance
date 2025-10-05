using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Models
{
    public record FirebaseUser(string Uid, string Email, string DisplayName);
    public class AuthResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public FirebaseUser User { get; set; }
    }
}
