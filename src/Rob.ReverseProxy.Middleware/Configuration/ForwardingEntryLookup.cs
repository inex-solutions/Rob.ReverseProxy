#region Copyright & License
// The MIT License (MIT)
// 
// Copyright 2018 INEX Solutions Ltd
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rob.ReverseProxy.Middleware.Configuration
{
    public class ForwardingEntryLookup
    {
        private readonly List<Tuple<Regex, ForwardingEntry>> _forwardingEntries;

        public ForwardingEntryLookup(IEnumerable<ForwardingEntry> forwardingEntries)
        {
            _forwardingEntries = forwardingEntries
                .Select(fe => new Tuple<Regex, ForwardingEntry>(new Regex(fe.SourceUrlMatch), fe))
                .ToList();
        }

        public ForwardingEntry this[string sourceHost] => _forwardingEntries.FirstOrDefault(fe => fe.Item1.IsMatch(sourceHost))?.Item2;

        public bool TryGetForwardingEntry(string sourceHost, out ForwardingEntry targetHost)
        {
            targetHost = this[sourceHost];
            return targetHost != null;
        }
    }
}