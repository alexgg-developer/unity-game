
using System;
using UnityEngine;

namespace MyUtils
{
	namespace Pair
	{
        [System.Serializable]
		public class Pair<T, U> {
			public Pair() {}

			public Pair(T first, U second) {
				this.First = first;
				this.Second = second;
			}

			public T First { get; set;}
			public U Second { get; set;}
		}

        [System.Serializable]
        public class uintPair {
			public uint First { get; set;}
			public uint Second { get; set;}

			public uintPair() {}

			public uintPair(uint first, uint second) {
				this.First = first;
				this.Second = second;
			}

			public uintPair(Pair<uint, uint> p) : this(p.First, p.Second) {}


			public override bool Equals(object obj)
			{
				if ((object)obj == null)
				{
					return false;
				}
				
                try {
                    uintPair uintPairObj = obj as uintPair;
                    if (uintPairObj != null) {
                        return (First == uintPairObj.First) && (Second == uintPairObj.Second);
                    }
                    else {
                        return false;
                    }
                }
                catch(Exception ex) {
                    Debug.Log(ex.Message);
                    return false;
                }
			}

			public override int GetHashCode()
			{
				return (int) (First ^ Second);
			}

			public static bool operator ==(uintPair a, uintPair b)
			{
				// If both are null, or both are same instance, return true.
				if (System.Object.ReferenceEquals(a, b))
				{
					return true;
				}

				// If one is null, but not both, return false.
				if (((object)a == null) || ((object)b == null))
				{
					return false;
				}

				// Return true if the fields match:
				return a.First == b.First && a.Second == b.Second;
			}

			public static bool operator !=(uintPair a, uintPair b)
			{
				return !(a == b);
			}

			public override string ToString ()
			{
				return string.Format ("[uintPair: First={0}, Second={1}]", First, Second);
			}
		}
		/*
		public sealed class uintPairEqualityComparer : IEqualityComparer<uintPair>
		{
			public bool Equals(uintPair a, uintPair b)
			{
				return a.First == b.First && a.Second == b.Second;
			}

			public int GetHashCode(uintPair obj)
			{
				unchecked
				{
					return (int) (obj.First ^ obj.Second);;
				}
			}
		}
		*/
	}
}