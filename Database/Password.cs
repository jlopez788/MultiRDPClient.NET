namespace Database
{
    public struct Password
    {
        public static readonly Password Empty = new Password();

        internal readonly string Encrypted;

        public Password(string value, bool isEncrypted)
        {
            Encrypted = null;
            if (!string.IsNullOrEmpty(value))
            {
                Encrypted = isEncrypted ? value : RijndaelSettings.Encrypt(value);
            }
        }

        public static implicit operator string(Password pwd) => pwd.Clear();

        public override bool Equals(object obj) => obj is Password pwd && string.Compare(Encrypted, pwd.Encrypted) == 0;

        public override int GetHashCode() => Encrypted?.GetHashCode() ?? 0;

        private string Clear() => !string.IsNullOrEmpty(Encrypted) ? RijndaelSettings.Decrypt(Encrypted) : "";
    }
}