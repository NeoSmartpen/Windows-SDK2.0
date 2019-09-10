using System;
using System.Numerics;

namespace Neosmartpen.Net.Support.Encryption
{
    /// <summary>
    /// Class representing private key information
    /// </summary>
    public class PrivateKey
    {
        // version, modulus, publicExponent, privateExponents, prime1, prime2, exponent1, exponent2, coefficient
        private byte[] version;
        private byte[] modulus;
        private byte[] publicExponent;
        private byte[] privateExponent;

        internal PrivateKey()
        {
        }

        internal byte[] GetVersion()
        {
            return version;
        }

        internal void SetVersion(byte[] version)
        {
            this.version = version;
        }

        internal BigInteger GetModulus()
        {
            byte[] newArr = new byte[modulus.Length];
            Array.Copy(modulus, newArr, newArr.Length);
            Array.Reverse(newArr);
            return new BigInteger(newArr);
        }

        internal void SetModulus(byte[] modulus)
        {
            this.modulus = modulus;
        }

        internal BigInteger GetPublicExponent()
        {
            byte[] newArr = new byte[publicExponent.Length];
            Array.Copy(publicExponent, newArr, newArr.Length);
            Array.Reverse(newArr);
            return new BigInteger(newArr);
        }

        internal void SetPublicExponent(byte[] publicExponent)
        {
            this.publicExponent = publicExponent;
        }

        internal BigInteger GetPrivateExponent()
        {
            byte[] newArr = new byte[privateExponent.Length];
            Array.Copy(privateExponent, newArr, newArr.Length);
            Array.Reverse(newArr);
            return new BigInteger(newArr);
        }

        internal void SetPrivateExponent(byte[] privateExponent)
        {
            this.privateExponent = privateExponent;
        }
    }
}
