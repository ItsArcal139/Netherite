﻿using System;
namespace Netherite.Utils
{
    public class PreconditionFailedException : Exception
    {
        public object Value { get; private set; }

        public PreconditionFailedException() : base() { }

        public PreconditionFailedException(string msg) : base(msg) { }

        public PreconditionFailedException(object val) : base()
        {
            Value = val;
        }

        public PreconditionFailedException(string msg, object val) : base(msg)
        {
            Value = val;
        }
    }
}
