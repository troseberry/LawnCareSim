namespace Core.DataStructures
{
    public class HashString
    {
        public int Hash { get; private set; }
        public string String { get; private set; }

        public HashString(string str)
        {
            String = str;
            Hash = String.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = (HashString)obj;

            return String.Equals(other.String) && Hash == other.Hash;
        }

        public override string ToString()
        {
            return String;
        }

        public override int GetHashCode()
        {
            return Hash;
        }
    }
}
