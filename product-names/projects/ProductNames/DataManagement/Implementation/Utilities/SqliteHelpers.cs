using System;
using System.Linq;

namespace ProductNames.DataManagement.Implementation.Utilities {
    public static class SqliteHelpers
    {
        public static string GetHexStringFromGuid(Guid id) {
            return HexStringFromByteArray(id.ToByteArray());
        }

        public static Guid GetGuidFromHexString(string hexString) {
            return new Guid(ByteArrayFromHexString(hexString));
        }

        private static byte[] ByteArrayFromHexString(string value) {
            return Enumerable.Range(0, value.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(value.Substring(x, 2), 16))
                         .ToArray();
        }

        private static string HexStringFromByteArray(byte[] byteArray) {
            return string.Join("", byteArray.Select(x => x.ToString("X2")).ToList());
        }
    }
}
