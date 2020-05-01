namespace Database
{
    public readonly struct Password
    {
        public static readonly Password Empty = new Password(null, false);

        internal readonly string Encrypted;
        public bool IsEmpty { get; }

        public Password(string value, bool isEncrypted)
        {
            IsEmpty = true;
            Encrypted = null;
            if (!string.IsNullOrEmpty(value))
            {
                IsEmpty = false;
                Encrypted = isEncrypted ? value : RijndaelSettings.Encrypt(value);
            }
        }

        public static implicit operator string(Password pwd) => pwd.Clear();

        public override bool Equals(object obj) => obj is Password pwd && (string.Compare(Encrypted, pwd.Encrypted) == 0 || pwd.IsEmpty == IsEmpty);

        public override int GetHashCode() => Encrypted?.GetHashCode() ?? 0;

        private string Clear() => !string.IsNullOrEmpty(Encrypted) ? RijndaelSettings.Decrypt(Encrypted) : "";
    }
}