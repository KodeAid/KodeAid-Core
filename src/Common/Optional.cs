// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;

namespace KodeAid
{
    public struct Optional<T>
    {
        public static readonly Optional<T> Undefined;

        private T _value;

        public Optional(T value)
        {
            _value = value;
            IsDefined = true;
        }

        public bool IsDefined { get; }

        public T Value
        {
            get
            {
                if (!IsDefined)
                {
                    throw new InvalidOperationException("Optional object must be defined.");
                }
                return _value;
            }
        }

        public T GetValueOrDefault()
        {
            return GetValueOrDefault(default);
        }

        public T GetValueOrDefault(T defaultValue)
        {
            return IsDefined ? _value : defaultValue;
        }

        public override bool Equals(object other)
        {
            if (!IsDefined)
            {
                return false;
            }

            if (other is Optional<T> optional)
            {
                if (!optional.IsDefined)
                {
                    return false;
                }
                other = optional.Value;
            }

            if (_value == null)
            {
                return other == null;
            }

            if (other == null)
            {
                return false;
            }

            return _value.Equals(other);
        }

        public override int GetHashCode()
        {
            return IsDefined ? _value.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return IsDefined ? _value.ToString() : "";
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public static explicit operator T(Optional<T> value)
        {
            return value.Value;
        }
    }
}
