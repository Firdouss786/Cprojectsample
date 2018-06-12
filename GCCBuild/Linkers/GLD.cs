/**
 * CCTask
 * 
 * Copyright 2012 Konrad Kruczyński <konrad.kruczynski@gmail.com>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:

 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CCTask.Linkers
{
	public sealed class GLD : ILinker
	{
		public GLD(string pathToLd, string preLDApp)
		{
			this.pathToLd = pathToLd;
            this.preLDApp = preLDApp;

        }

		public bool Link(IEnumerable<string> objectFiles, string outputFile, string flags)
		{
            if (!string.IsNullOrEmpty(preLDApp))
            {
                objectFiles = objectFiles.Select(x => x = Utilities.ConvertWinPathToWSL(x));
                outputFile = Utilities.ConvertWinPathToWSL(outputFile);
            }

			var linkerArguments = string.Format("{0} {2} -o \"{1}\"", objectFiles.Select(x => "\"" + x + "\"").Aggregate((x, y) => x + " " + y), outputFile, flags);
			var runWrapper = new RunWrapper(pathToLd, linkerArguments, preLDApp);
			Logger.Instance.LogMessage("{0} {1}", pathToLd, linkerArguments);
            string outPutDir = Path.GetDirectoryName(outputFile);
            if (!Directory.Exists(outPutDir))
                Directory.CreateDirectory(outPutDir);

			#if DEBUG
			Logger.Instance.LogMessage(linkerArguments);
			#endif
			return runWrapper.Run();
		}

		private readonly string pathToLd;
        private readonly string preLDApp;

    }
}
